using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

namespace DTech.Blackboard.Editor
{
	internal sealed class BlackboardEditorWindow : EditorWindow
	{
		private const string VariablesPropertyName = "_variables";

		private BlackboardAsset _blackboardAsset;
		private SerializedProperty _listProperty;
		private SerializedObject _serializedObject;
		private BlackboardVariableOption _selectedVariableOption;
		private string _variableName = string.Empty;
		private Vector2 _scrollPosition;
		
		public static void ShowWindow(BlackboardAsset blackboardAsset)
		{
			BlackboardEditorWindow[] windows = Resources.FindObjectsOfTypeAll<BlackboardEditorWindow>();
			BlackboardEditorWindow window = windows.Length > 0 ? windows[0] : GetWindow<BlackboardEditorWindow>();
			window.Initialize(blackboardAsset);
			window.Focus();
		}
		
		private void Initialize(BlackboardAsset blackboardAsset)
		{
			_blackboardAsset = blackboardAsset;
			BindToAsset();
			_selectedVariableOption = null;
			titleContent = new GUIContent($"Blackboard [{blackboardAsset.name}]");
			minSize = new Vector2(700f, 350f);
			Repaint();
		}

		private void OnGUI()
		{
			if (_blackboardAsset == null)
			{
				EditorGUILayout.HelpBox("Select a BlackboardAsset to edit.", MessageType.Info);
				return;
			}

			if (_serializedObject == null || _serializedObject.targetObject != _blackboardAsset)
			{
				BindToAsset();
			}

			_serializedObject.Update();

			using (new EditorGUILayout.VerticalScope())
			{
				DrawVariableListBlock();
				DrawAddVariableBlock();
			}

			_serializedObject.ApplyModifiedProperties();
		}

		private void DrawVariableListBlock()
		{
			using var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition, "box");
			for (int i = 0; i < _blackboardAsset.Variables.Count; i++)
			{
				using (new EditorGUILayout.VerticalScope("box"))
				{
					SerializedProperty element = _listProperty.GetArrayElementAtIndex(i);
					var variable = (BlackboardVariable)element.managedReferenceValue;
					BlackboardVariableDrawer drawer = BlackboardVariableDrawerProvider.GetDrawerForType(variable.ValueType);
					drawer.Draw(element);
					using (new EditorGUILayout.HorizontalScope("box"))
					{
						if (GUILayout.Button("Remove"))
						{
							_listProperty.DeleteArrayElementAtIndex(i);
							break;
						}
					}
				}
			}
			
			_scrollPosition = scrollView.scrollPosition;
		}

		private void DrawAddVariableBlock()
		{
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
					bool isValidVariableName = IsValidVariableName(_variableName, out string normalizedVariableName, out string errorMessage);
					if (!isValidVariableName && !string.IsNullOrEmpty(_variableName))
					{
						EditorGUILayout.HelpBox(errorMessage, MessageType.Error);
					}

					using (new EditorGUI.DisabledGroupScope(!hasOption || !isValidVariableName))
					{
						if (GUILayout.Button("Add"))
						{
							Undo.RecordObject(_blackboardAsset, "Add Blackboard Variable");
							int elementIndex = _listProperty.arraySize;
							_listProperty.arraySize++;
							SerializedProperty element = _listProperty.GetArrayElementAtIndex(elementIndex);
							element.managedReferenceValue = BlackboardVariable.CreateForType(_selectedVariableOption.Type.NativeType, normalizedVariableName);
							_selectedVariableOption = null;
							_variableName = string.Empty;
							GUI.FocusControl(null);
						}
					}
				}
			}
		}
		
		private bool IsValidVariableName(string variableName, out string normalizedVariableName, out string errorMessage)
		{
			return BlackboardVariableNameValidator.TryValidate(
				_blackboardAsset.Variables,
				variableName,
				null,
				out normalizedVariableName,
				out errorMessage);
		}
		
		private void OnEnable()
		{
			Undo.undoRedoPerformed += UndoRedoPerformed;
			BindToAsset();
		}

		private void OnDisable()
		{
			Undo.undoRedoPerformed -= UndoRedoPerformed;
		}

		private void BindToAsset()
		{
			if (_blackboardAsset == null)
			{
				_serializedObject = null;
				return;
			}

			_serializedObject = new SerializedObject(_blackboardAsset);
			_listProperty = _serializedObject.FindProperty(VariablesPropertyName);
		}

		private void UndoRedoPerformed()
		{
			if (_serializedObject?.targetObject != null)
			{
				_serializedObject.Update();
			}

			Repaint();
		}

		private void SetOption(BlackboardVariableOption value) => _selectedVariableOption = value;
	}
}
