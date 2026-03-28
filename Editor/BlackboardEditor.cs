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
		private BlackboardVariableOption _selectedVariableOption;
		private string _variableName;
		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			
			_listDrawProvider.Draw();

			using (new EditorGUILayout.VerticalScope("box"))
			{
				EditorGUILayout.LabelField("Add Variable");
				bool hasOption = _selectedVariableOption != null;
				string caption = hasOption ? _selectedVariableOption.Name : "Select an option";
				if (GUILayout.Button(caption, EditorStyles.popup))
				{
					var provider = BlackboardTypeSearchProvider.Create(SetOption);
					var context = new SearchWindowContext(GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
					SearchWindow.Open(context, provider);
				}

				using (new EditorGUI.DisabledGroupScope(!hasOption))
				{
					_variableName = EditorGUILayout.TextField("Name", _variableName);
					bool isValidVariableName = IsValidVariableName(_variableName, out string normalizedVariableName);
					using (new EditorGUI.DisabledGroupScope(!hasOption || !isValidVariableName))
					{
						if (GUILayout.Button("Add"))
						{
							_listDrawProvider.Add(normalizedVariableName, _selectedVariableOption.Type.Type);
							_selectedVariableOption = null;
							_variableName = string.Empty;
						}
					}
				}
			}
			
			serializedObject.ApplyModifiedProperties();
		}

		private bool IsValidVariableName(string variableName, out string normalizedVariableName)
		{
			return BlackboardVariableNameValidator.TryValidate(
				_blackboardAsset.Variables,
				variableName,
				null,
				out normalizedVariableName,
				out _);
		}

		private void OnEnable()
		{
			_blackboardAsset = (BlackboardAsset)target;
			SerializedProperty listProperty = serializedObject.FindProperty(VariablesPropertyName);
			_listDrawProvider = new BlackboardVariableListDrawProvider(serializedObject, listProperty);
			_selectedVariableOption = null;
			_variableName = string.Empty;
		}

		private void OnDisable()
		{
			_blackboardAsset = null;
			_listDrawProvider?.Dispose();
			_listDrawProvider = null;
		}

		private void SetOption(BlackboardVariableOption value) => _selectedVariableOption = value;
	}
}
