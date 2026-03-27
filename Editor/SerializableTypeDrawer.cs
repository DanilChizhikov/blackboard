using UnityEditor;
using UnityEngine;

namespace DTech.Blackboard.Editor
{
    [CustomPropertyDrawer(typeof(SerializableType))]
    internal sealed class SerializableTypeDrawer : PropertyDrawer
    {
        private const string SerializableTypeFieldName = "_serializableType";
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, label);

            using (new EditorGUI.DisabledGroupScope(true))
            {
                SerializedProperty serializedTypeProperty = property.FindPropertyRelative(SerializableTypeFieldName);
                if (string.IsNullOrEmpty(serializedTypeProperty.stringValue))
                {
                    EditorGUI.TextField(position, label, string.Empty);
                }
                else
                {
                    var serializableType = (SerializableType)property.boxedValue;
                    EditorGUI.TextField(position, label, serializableType.Type.FullName);
                }
            }
            
            EditorGUI.EndProperty();
        }
    }
}