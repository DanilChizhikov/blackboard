using UnityEditor;
using UnityEngine;

namespace DTech.Blackboard.Editor
{
    [CustomPropertyDrawer(typeof(BlackboardVariableReference))]
    internal sealed class BlackboardVariableReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
        }
    }
}