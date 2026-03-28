using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DTech.Blackboard.Editor
{
	internal sealed class BlackboardTypeSearchProvider : ScriptableObject, ISearchWindowProvider
	{
		private Action<BlackboardOption> _onTypeSelected;
		
		public static BlackboardTypeSearchProvider Create(Action<BlackboardOption> onTypeSelected)
		{
			var provider = CreateInstance<BlackboardTypeSearchProvider>();
			provider._onTypeSelected = onTypeSelected;
			return provider;
		}
		
		private static GUIContent GetEntryContent(BlackboardOption option)
		{
			bool hasIcon = !string.IsNullOrEmpty(option.IconName);
			Texture icon = hasIcon ? EditorGUIUtility.IconContent(option.IconName).image : null;
			var content = hasIcon ? new GUIContent(option.Name, icon) : new GUIContent(option.Name);
			return content;
		}
		
		List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
		{
			var tree = new List<SearchTreeEntry>
			{
				new SearchTreeGroupEntry(new GUIContent("Variables"), 0)
			};
			
			List<BlackboardOption> options = BlackboardRegistry.GetStoryVariableTypes();
			var pathSet = new HashSet<string>();
			for (int i = 0; i < options.Count; i++)
			{
				BlackboardOption option = options[i];
				if (string.IsNullOrEmpty(option.Path))
				{
					continue;
				}

				GUIContent entryContent = GetEntryContent(option);
				string[] pathParts = option.Path.Split('/');
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
									userData = option
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
						userData = option
					};
					tree.Add(entry);
				}
			}

			return tree;
		}

		bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
		{
			var option = searchTreeEntry.userData as BlackboardOption;
			_onTypeSelected?.Invoke(option);
			return true;
		}
	}
}