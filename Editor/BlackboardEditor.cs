using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DTech.Blackboard.Editor
{
	[CustomEditor(typeof(BlackboardAsset))]
	internal sealed class BlackboardEditor : UnityEditor.Editor
	{
		private const string VariablesPropertyName = "_variables";

		private BlackboardAsset _blackboardAsset;
		private BlackboardVariableListDrawProvider _listDrawProvider;
		private BlackboardOption _selectedOption;
		private string _variableName;
		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			_listDrawProvider.Draw();

			using (new EditorGUILayout.VerticalScope("box"))
			{
				EditorGUILayout.LabelField("Add Variable");
				bool hasOption = _selectedOption != null;
				string caption = hasOption ? _selectedOption.Name : "Select an option";
				if (GUILayout.Button(caption, EditorStyles.popup))
				{
					var provider = BlackboardTypeSearchProvider.Create(SetOption);
					var context = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
					SearchWindow.Open(context, provider);
				}

				using (new EditorGUI.DisabledGroupScope(!hasOption))
				{
					_variableName = EditorGUILayout.TextField("Name", _variableName);
					using (new EditorGUI.DisabledGroupScope(!hasOption || !IsValidVariableName(_variableName)))
					{
						if (GUILayout.Button("Add"))
						{
							_listDrawProvider.Add(_variableName, _selectedOption.Type.Type);
							_selectedOption = null;
							_variableName = string.Empty;
						}
					}
				}
			}
			
			serializedObject.ApplyModifiedProperties();
		}

		private bool IsValidVariableName(string variableName)
		{
			if (string.IsNullOrEmpty(variableName))
			{
				return false;
			}

			foreach (BlackboardVariable variable in _blackboardAsset.Variables)
			{
				if (variable.Name == _variableName)
				{
					return false;
				}
			}

			return true;
		}

		private void OnEnable()
		{
			_blackboardAsset = (BlackboardAsset)target;
			SerializedProperty listProperty = serializedObject.FindProperty(VariablesPropertyName);
			_listDrawProvider = new BlackboardVariableListDrawProvider(serializedObject, listProperty);
			_selectedOption = null;
			_variableName = string.Empty;
		}

		private void OnDisable()
		{
			_blackboardAsset = null;
			_listDrawProvider?.Dispose();
			_listDrawProvider = null;
		}

		private void SetOption(BlackboardOption value) => _selectedOption = value;
	}
}