using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace DTech.Blackboard.Editor
{
	internal static class BlackboardRegistry
	{
		private static readonly List<BlackboardOption> _customVariableOptions = new();
		private static readonly string[] _excludedNamespaces =
		{
			"Editor",
			"Muse",
			"AppUI",
		};
        
		public static List<BlackboardOption> GetDefaultBlackboardOptions()
		{
			return new List<BlackboardOption>
			{
				new BlackboardOption(typeof(GameObject), "GameObject", "GameObject On Icon", priority: 2),
				new BlackboardOption(typeof(Transform), "Transform", "Transform Icon", priority: 1),

				new BlackboardOption(typeof(string), "Basic Types/String"),
				new BlackboardOption(typeof(float), "Basic Types/Float"),
				new BlackboardOption(typeof(int), "Basic Types/Integer"),
				new BlackboardOption(typeof(double), "Basic Types/Double"),
				new BlackboardOption(typeof(bool), "Basic Types/Boolean"),
				new BlackboardOption(typeof(Vector2), "Vector Types/Vector2"),
				new BlackboardOption(typeof(Vector3), "Vector Types/Vector3"),
				new BlackboardOption(typeof(Vector4), "Vector Types/Vector4"),
				new BlackboardOption(typeof(Vector2Int), "Vector Types/Vector2 Int"),
				new BlackboardOption(typeof(Vector3Int), "Vector Types/Vector3 Int"),
				new BlackboardOption(typeof(Color), "Basic Types/Color"),

				// Resource Types
				new BlackboardOption(typeof(ScriptableObject), "Resources/Scriptable Object", "ScriptableObject Icon"),
				new BlackboardOption(typeof(Texture2D), "Resources/Texture2D"),
				new BlackboardOption(typeof(Sprite), "Resources/Sprite"),
				new BlackboardOption(typeof(Material), "Resources/Material"),
				new BlackboardOption(typeof(AudioClip), "Resources/Audio Clip"),
				new BlackboardOption(typeof(AnimationClip), "Resources/Animation Clip"),
				new BlackboardOption(typeof(AudioMixer), "Resources/Audio Mixer"),
				new BlackboardOption(typeof(TextAsset), "Resources/Text Asset"),
				new BlackboardOption(typeof(ParticleSystem), "Resources/Particle System"),

				// List Types
				new BlackboardOption(typeof(List<GameObject>), "List/Game Object List"),
				new BlackboardOption(typeof(List<string>), "List/String List"),
				new BlackboardOption(typeof(List<float>), "List/Float List"),
				new BlackboardOption(typeof(List<int>), "List/Integer List"),
				new BlackboardOption(typeof(List<double>), "List/Double List"),
				new BlackboardOption(typeof(List<bool>), "List/Boolean List"),
				new BlackboardOption(typeof(List<Vector2>), "List/Vector2 List"),
				new BlackboardOption(typeof(List<Vector3>), "List/Vector3 List"),
				new BlackboardOption(typeof(List<Vector4>), "List/Vector4 List"),
				new BlackboardOption(typeof(List<Vector2Int>), "List/Vector2 Int List"),
				new BlackboardOption(typeof(List<Vector3Int>), "List/Vector3 Int List"),
				new BlackboardOption(typeof(List<Color>), "List/Color List"),
			};
		}

		public static List<BlackboardOption> GetEnumVariableTypes()
		{
			var enumOptions = new List<BlackboardOption>();
			IEnumerable<Type> enumTypes = TypeCache.GetTypesWithAttribute<BlackboardEnumAttribute>()
				.Where(type => type.IsEnum && Enum.GetValues(type).Length > 0);

			foreach (var type in enumTypes)
			{
				enumOptions.Add(new BlackboardOption(type, "Enumeration/" + BlackboardUtilities.NicifyVariableName(type.Name)));
			}

			return enumOptions;
		}

		public static List<BlackboardOption> GetStoryVariableTypes()
		{
			List<BlackboardOption> options = GetDefaultBlackboardOptions();
			List<BlackboardOption> enums = GetEnumVariableTypes();

			AddCustomTypes<Component>(options, "Other/Components", "cs Script Icon");
			AddCustomTypes<ScriptableObject>(options, "Other/ScriptableObjects", "ScriptableObject Icon");

			options.AddRange(enums);

			return options;
		}

		public static List<BlackboardOption> GetStoryVariableTypesWithOperators()
		{
			List<BlackboardOption> options = GetStoryVariableTypes();
			return options;
		}
        
		public static List<BlackboardOption> GetCustomTypes()
		{
			if (_customVariableOptions.Count == 0)
			{
				AddCustomTypes<Component>(_customVariableOptions, "Components", "cs Script Icon");
				AddCustomTypes<ScriptableObject>(_customVariableOptions, "ScriptableObjects", "ScriptableObject Icon");
			}

			return _customVariableOptions;
		}

		private static List<BlackboardOption> AddCustomTypes<TypeName>(List<BlackboardOption> options, string path, string icon = null)
		{
			if (string.IsNullOrEmpty(path))
			{
				path = "";
			}
			else
			{
				path += "/";
			}

			TypeCache.TypeCollection monoBehaviourTypes = TypeCache.GetTypesDerivedFrom<TypeName>();
			foreach (var type in monoBehaviourTypes)
			{
				if (type.IsNotPublic ||
					!type.IsVisible ||
					IsExcludedNamespaceOrType(type))
				{
					continue;
				}

				string namespacePath = type.Namespace?.Replace('.', '/');
				if (!string.IsNullOrEmpty(namespacePath))
				{
					namespacePath += "/";
				}

				string fullPath = $"{path}{namespacePath}{BlackboardUtilities.NicifyVariableName(type.Name, true)}";
				var option = new BlackboardOption(type, fullPath, icon);
				options.Add(option);
			}

			return options;
		}

		private static bool IsExcludedNamespaceOrType(Type type)
		{
			if (string.IsNullOrEmpty(type.Namespace))
			{
				return false;
			}

			foreach (string excludedNamespace in _excludedNamespaces)
			{
				if (type.Namespace.Contains(excludedNamespace))
				{
					return true;
				}
			}
            
			return false;
		}
	}
}