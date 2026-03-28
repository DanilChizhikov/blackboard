using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DTech.Blackboard.Editor
{
	public abstract class BlackboardVariableDrawer
	{
		public abstract Type ServicedValueType { get; }
		
		public abstract void Draw(Rect position, SerializedProperty property);
		public abstract void Draw(SerializedProperty property);
		
		public abstract float GetPropertyHeight(SerializedProperty property);
	}

	public class BlackboardVariableDrawer<T> : BlackboardVariableDrawer
	{
		private const string GuidPropertyName = "_guid";
		private const string NamePropertyName = "_name";
		private const string ValuePropertyName = "_value";
		private const string NameLabel = "Name";
		
		private static readonly Color _nameErrorOutlineColor = new (0.87f, 0.26f, 0.26f);
		private static readonly Dictionary<string, NameValidationState> _nameValidationStates = new();

		public override Type ServicedValueType => typeof(T);

		public override void Draw(Rect position, SerializedProperty property)
		{
			SerializedProperty nameProperty = property.FindPropertyRelative(NamePropertyName);
			GUIContent label = new GUIContent(nameProperty.stringValue);
			EditorGUI.BeginProperty(position, label, property);
			
			property.isExpanded = EditorGUI.Foldout(
				new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
				property.isExpanded,
				label
			);
			
			if (!property.isExpanded)
			{
				EditorGUI.EndProperty();
				return;
			}
			
			float currentY = position.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			
			EditorGUI.indentLevel++;
			
			SerializedProperty guidProperty = property.FindPropertyRelative(GuidPropertyName);
			SerializedProperty valueProperty = property.FindPropertyRelative(ValuePropertyName);
			
			float guidHeight = EditorGUI.GetPropertyHeight(guidProperty);
			Rect guidPosition = new Rect(position.x, currentY, position.width, guidHeight);
			EditorGUI.PropertyField(guidPosition, guidProperty);
			currentY += guidHeight + EditorGUIUtility.standardVerticalSpacing;
			
			float nameHeight = EditorGUI.GetPropertyHeight(nameProperty);
			Rect namePosition = new Rect(position.x, currentY, position.width, nameHeight);
			BlackboardVariable variable = property.managedReferenceValue as BlackboardVariable;
			SerializableGuid variableGuid = variable?.Guid ?? default;
			IReadOnlyList<BlackboardVariable> variables = GetVariables(property);
			string key = GetStateKey(property, variableGuid);
			string displayedName = GetDisplayedName(nameProperty, key);
			bool hasError = !TryValidateName(variables, displayedName, variableGuid, out _, out string errorMessage);
			DrawNameField(namePosition, displayedName, hasError, out string editedName);
			if (!string.Equals(editedName, displayedName, StringComparison.Ordinal))
			{
				HandleNameEdit(nameProperty, variables, variableGuid, key, editedName);
				hasError = !TryValidateName(variables, GetDisplayedName(nameProperty, key), variableGuid, out _, out errorMessage);
			}
			currentY += nameHeight + EditorGUIUtility.standardVerticalSpacing;

			if (hasError)
			{
				Rect helpBoxPosition = new Rect(position.x, currentY, position.width, GetHelpBoxHeight());
				EditorGUI.HelpBox(helpBoxPosition, errorMessage, MessageType.Error);
				currentY += GetHelpBoxHeight() + EditorGUIUtility.standardVerticalSpacing;
			}
			
			float valueHeight = EditorGUI.GetPropertyHeight(valueProperty);
			Rect valuePosition = new Rect(position.x, currentY, position.width, valueHeight);
			EditorGUI.PropertyField(valuePosition, valueProperty);
			
			EditorGUI.indentLevel--;
			EditorGUI.EndProperty();
		}

		public override void Draw(SerializedProperty property)
		{
			BlackboardVariable variable = property.managedReferenceValue as BlackboardVariable;
			IReadOnlyList<BlackboardVariable> variables = GetVariables(property);
			SerializedProperty guidProperty = property.FindPropertyRelative(GuidPropertyName);
			SerializedProperty nameProperty = property.FindPropertyRelative(NamePropertyName);
			SerializedProperty valueProperty = property.FindPropertyRelative(ValuePropertyName);
			
			EditorGUILayout.PropertyField(guidProperty);
			EditorGUILayout.PropertyField(nameProperty);

			SerializableGuid variableGuid = variable?.Guid ?? default;
			string key = GetStateKey(property, variableGuid);
			string displayedName = GetDisplayedName(nameProperty, key);
			bool hasError = !TryValidateName(variables, displayedName, variableGuid, out _, out string errorMessage);
			if (hasError)
			{
				EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
			}
			
			EditorGUILayout.PropertyField(valueProperty);
		}

		public override float GetPropertyHeight(SerializedProperty property)
		{
			if (!property.isExpanded)
			{
				return EditorGUIUtility.singleLineHeight;
			}
			
			SerializedProperty guidProperty = property.FindPropertyRelative(GuidPropertyName);
			SerializedProperty nameProperty = property.FindPropertyRelative(NamePropertyName);
			SerializedProperty valueProperty = property.FindPropertyRelative(ValuePropertyName);
			BlackboardVariable variable = property.managedReferenceValue as BlackboardVariable;
			SerializableGuid variableGuid = variable?.Guid ?? default;
			IReadOnlyList<BlackboardVariable> variables = GetVariables(property);
			string key = GetStateKey(property, variableGuid);
			bool hasError = !TryValidateName(variables, GetDisplayedName(nameProperty, key), variableGuid, out _, out _);
			
			float height = EditorGUIUtility.singleLineHeight;
			height += EditorGUIUtility.standardVerticalSpacing;
			height += EditorGUI.GetPropertyHeight(guidProperty);
			height += EditorGUIUtility.standardVerticalSpacing;
			height += EditorGUI.GetPropertyHeight(nameProperty);
			height += EditorGUIUtility.standardVerticalSpacing;
			if (hasError)
			{
				height += GetHelpBoxHeight();
				height += EditorGUIUtility.standardVerticalSpacing;
			}
			height += EditorGUI.GetPropertyHeight(valueProperty);
			
			return height;
		}

		private static void HandleNameEdit(
			SerializedProperty nameProperty,
			IReadOnlyList<BlackboardVariable> variables,
			SerializableGuid variableGuid,
			string key,
			string editedName)
		{
			if (TryValidateName(variables, editedName, variableGuid, out string normalizedName, out _))
			{
				nameProperty.stringValue = normalizedName;
				_nameValidationStates.Remove(key);
				return;
			}

			_nameValidationStates[key] = new NameValidationState(editedName);
		}

		private static void DrawNameField(Rect position, string value, bool hasError, out string editedName)
		{
			editedName = EditorGUI.TextField(position, NameLabel, value);
			if (!hasError)
			{
				return;
			}

			Rect outlineRect = new Rect(position.x, position.yMax - 1f, position.width, 1f);
			EditorGUI.DrawRect(outlineRect, _nameErrorOutlineColor);
		}

		private static bool TryValidateName(
			IReadOnlyList<BlackboardVariable> variables,
			string variableName,
			SerializableGuid variableGuid,
			out string normalizedName,
			out string errorMessage)
		{
			return BlackboardVariableNameValidator.TryValidate(variables, variableName, variableGuid, out normalizedName, out errorMessage);
		}

		private static string GetDisplayedName(SerializedProperty nameProperty, string key)
		{
			if (_nameValidationStates.TryGetValue(key, out NameValidationState state))
			{
				return state.PendingName;
			}

			return nameProperty.stringValue;
		}

		private static string GetStateKey(SerializedProperty property, SerializableGuid guid)
		{
			int targetId = property.serializedObject.targetObject.GetInstanceID();
			return $"{targetId}:{property.propertyPath}:{guid}";
		}

		private static IReadOnlyList<BlackboardVariable> GetVariables(SerializedProperty property)
		{
			if (property?.serializedObject?.targetObject is BlackboardAsset blackboardAsset)
			{
				return blackboardAsset.Variables;
			}

			return Array.Empty<BlackboardVariable>();
		}

		private static float GetHelpBoxHeight()
		{
			return EditorGUIUtility.singleLineHeight * 2f;
		}

		private readonly struct NameValidationState
		{
			public readonly string PendingName;

			public NameValidationState(string pendingName)
			{
				PendingName = pendingName;
			}
		}
	}
}
