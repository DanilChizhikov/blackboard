using UnityEditor;
using UnityEngine;

namespace DTech.Blackboard.Editor
{
    [CustomPropertyDrawer(typeof(SerializableGuid))]
    internal sealed class SerializableGuidDrawer : PropertyDrawer
    {
        private const float RegenerateButtonWidth = 35f;
        private const float ButtonOffset = 5f;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Rect propertyRect = position;
            propertyRect.xMax -= RegenerateButtonWidth + ButtonOffset;

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.TextField(propertyRect, label, property.boxedValue.ToString());
            }

            Rect buttonRect = new Rect(position);
            buttonRect.width = RegenerateButtonWidth;
            buttonRect.x = propertyRect.xMax + ButtonOffset;

            GUIContent copyIcon = EditorGUIUtility.IconContent("d__Menu@2x");
            if (GUI.Button(buttonRect, copyIcon))
            {
                var contextMenu = new GenericMenu();
                contextMenu.AddItem(new GUIContent("Regenerate"), false,
                    () =>
                    {
                        property.boxedValue = SerializableGuid.Generate();
                        property.serializedObject.ApplyModifiedProperties();
                    });
				
                contextMenu.AddItem(new GUIContent("Copy"), false,
                    () =>
                    {
                        EditorGUIUtility.systemCopyBuffer = property.boxedValue.ToString();
                    });
				
                contextMenu.ShowAsContext();
            }
			
            EditorGUI.EndProperty();
        }
    }
}