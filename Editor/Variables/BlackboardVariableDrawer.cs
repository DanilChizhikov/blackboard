using System;
using UnityEditor;
using UnityEngine;

namespace DTech.Blackboard.Editor
{
	public abstract class BlackboardVariableDrawer
	{
		public abstract Type ServicedValueType { get; }
		
		public abstract void Draw(Rect position, SerializedProperty property);
		
		public abstract float GetPropertyHeight(SerializedProperty property);
	}

	public sealed class BlackboardVariableDrawer<T> : BlackboardVariableDrawer
	{
		private const string GuidPropertyName = "_guid";
		private const string NamePropertyName = "_name";
		private const string ValuePropertyName = "_value";

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
			EditorGUI.PropertyField(namePosition, nameProperty);
			currentY += nameHeight + EditorGUIUtility.standardVerticalSpacing;
			
			float valueHeight = EditorGUI.GetPropertyHeight(valueProperty);
			Rect valuePosition = new Rect(position.x, currentY, position.width, valueHeight);
			EditorGUI.PropertyField(valuePosition, valueProperty);
			
			EditorGUI.indentLevel--;
			EditorGUI.EndProperty();
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
			
			float height = EditorGUIUtility.singleLineHeight;
			height += EditorGUIUtility.standardVerticalSpacing;
			height += EditorGUI.GetPropertyHeight(guidProperty);
			height += EditorGUIUtility.standardVerticalSpacing;
			height += EditorGUI.GetPropertyHeight(nameProperty);
			height += EditorGUIUtility.standardVerticalSpacing;
			height += EditorGUI.GetPropertyHeight(valueProperty);
			
			return height;
		}
	}
}