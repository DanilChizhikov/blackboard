using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace DTech.Blackboard.Editor
{
	internal sealed class BlackboardVariableListDrawProvider : IDisposable
	{
		private readonly ReorderableList _list;

		public BlackboardVariableListDrawProvider(SerializedObject serializedObject, SerializedProperty listProperty)
		{
			_list = new ReorderableList(serializedObject, listProperty);
			_list.drawHeaderCallback += DrawHeaderCallback;
			_list.drawElementCallback += DrawElementCallback;
			_list.elementHeightCallback += ElementHeightCallback;
			_list.onRemoveCallback += RemoveCallback;
			_list.displayAdd = false;
			_list.displayRemove = true;
			_list.draggable = false;
		}

		public void Draw()
		{
			_list.DoLayoutList();
		}

		public void Add(string name, Type valueType)
		{
			int elementIndex = _list.serializedProperty.arraySize;
			_list.serializedProperty.arraySize++;
			SerializedProperty element = _list.serializedProperty.GetArrayElementAtIndex(elementIndex);
			element.managedReferenceValue = BlackboardVariable.CreateForType(valueType, name);
			_list.serializedProperty.serializedObject.ApplyModifiedProperties();
		}

		public void Dispose()
		{
			_list.drawHeaderCallback -= DrawHeaderCallback;
			_list.drawElementCallback -= DrawElementCallback;
			_list.elementHeightCallback -= ElementHeightCallback;
			_list.onRemoveCallback -= RemoveCallback;
		}
		
		private void DrawHeaderCallback(Rect rect)
		{
			EditorGUI.LabelField(rect, new GUIContent("Variables"));
		}
		
		private void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
		{
			EditorGUI.indentLevel++;
			SerializedProperty element = _list.serializedProperty.GetArrayElementAtIndex(index);
			var variable = (BlackboardVariable)element.managedReferenceValue;
			BlackboardVariableDrawer drawer = BlackboardVariableDrawerProvider.GetDrawerForType(variable.ValueType);
			drawer.Draw(rect, element);
			EditorGUI.indentLevel--;
		}
		
		private float ElementHeightCallback(int index)
		{
			SerializedProperty element = _list.serializedProperty.GetArrayElementAtIndex(index);
			var variable = (BlackboardVariable)element.managedReferenceValue;
			BlackboardVariableDrawer drawer = BlackboardVariableDrawerProvider.GetDrawerForType(variable.ValueType);
			return drawer.GetPropertyHeight(element);
		}
		
		private void RemoveCallback(ReorderableList list)
		{
			int removeIndex = list.index;
			list.serializedProperty.DeleteArrayElementAtIndex(removeIndex);
			list.serializedProperty.serializedObject.ApplyModifiedProperties();
		}
	}
}