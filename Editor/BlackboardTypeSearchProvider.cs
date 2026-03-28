using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DTech.Blackboard.Editor
{
	internal sealed class BlackboardTypeSearchProvider : ScriptableObject, ISearchWindowProvider
	{
		private Action<BlackboardVariableOption> _onTypeSelected;
		
		public static BlackboardTypeSearchProvider Create(Action<BlackboardVariableOption> onTypeSelected)
		{
			var provider = CreateInstance<BlackboardTypeSearchProvider>();
			provider._onTypeSelected = onTypeSelected;
			return provider;
		}
		
		private static GUIContent GetEntryContent(BlackboardVariableOption variableOption)
		{
			bool hasIcon = !string.IsNullOrEmpty(variableOption.IconName);
			Texture icon = hasIcon ? EditorGUIUtility.IconContent(variableOption.IconName).image : null;
			var content = hasIcon ? new GUIContent(variableOption.Name, icon) : new GUIContent(variableOption.Name);
			return content;
		}
		
		List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
		{
			var tree = new List<SearchTreeEntry>
			{
				new SearchTreeGroupEntry(new GUIContent("Variables"), 0)
			};
			
			List<BlackboardVariableOption> options = BlackboardRegistry.GetStoryVariableTypes();
			var pathSet = new HashSet<string>();
			for (int i = 0; i < options.Count; i++)
			{
				BlackboardVariableOption variableOption = options[i];
				if (string.IsNullOrEmpty(variableOption.Path))
				{
					continue;
				}

				GUIContent entryContent = GetEntryContent(variableOption);
				string[] pathParts = variableOption.Path.Split('/');
				if (pathParts.Length > 1)
				{
					string currentPath = string.Empty;
					for (int j = 0; j < pathParts.Length; j++)
					{
						string pathPart = pathParts[j];
						currentPath = string.IsNullOrEmpty(currentPath)
							? pathPart
							: currentPath + "/" + pathPart;

						if (pathSet.Add(currentPath))
						{
							int level = j + 1;
							bool isLast = level >= pathParts.Length;
							SearchTreeEntry entry;
							if (isLast)
							{
								entry = new SearchTreeEntry(entryContent)
								{
									level = level,
									userData = variableOption
								};
							}
							else
							{
								entry = new SearchTreeGroupEntry(new GUIContent(pathPart), level);
							}
							
							tree.Add(entry);
						}
					}
				}
				else
				{
					var entry = new SearchTreeEntry(entryContent)
					{
						level = 1,
						userData = variableOption
					};
					tree.Add(entry);
				}
			}

			return tree;
		}

		bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
		{
			var option = searchTreeEntry.userData as BlackboardVariableOption;
			_onTypeSelected?.Invoke(option);
			return true;
		}
	}
}