using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace DTech.Blackboard.Editor
{
	internal static class BlackboardRegistry
	{
		private static readonly List<BlackboardVariableOption> _customVariableOptions = new();
		private static readonly string[] _excludedNamespaces =
		{
			"Editor",
			"Muse",
			"AppUI",
		};
        
		public static List<BlackboardVariableOption> GetDefaultBlackboardOptions()
		{
			return new List<BlackboardVariableOption>
			{
				new BlackboardVariableOption(typeof(GameObject), "GameObject", "GameObject On Icon", priority: 2),
				new BlackboardVariableOption(typeof(Transform), "Transform", "Transform Icon", priority: 1),

				new BlackboardVariableOption(typeof(string), "Basic Types/String"),
				new BlackboardVariableOption(typeof(float), "Basic Types/Float"),
				new BlackboardVariableOption(typeof(int), "Basic Types/Integer"),
				new BlackboardVariableOption(typeof(double), "Basic Types/Double"),
				new BlackboardVariableOption(typeof(bool), "Basic Types/Boolean"),
				new BlackboardVariableOption(typeof(Vector2), "Vector Types/Vector2"),
				new BlackboardVariableOption(typeof(Vector3), "Vector Types/Vector3"),
				new BlackboardVariableOption(typeof(Vector4), "Vector Types/Vector4"),
				new BlackboardVariableOption(typeof(Vector2Int), "Vector Types/Vector2 Int"),
				new BlackboardVariableOption(typeof(Vector3Int), "Vector Types/Vector3 Int"),
				new BlackboardVariableOption(typeof(Color), "Basic Types/Color"),

				// Resource Types
				new BlackboardVariableOption(typeof(ScriptableObject), "Resources/Scriptable Object", "ScriptableObject Icon"),
				new BlackboardVariableOption(typeof(Texture2D), "Resources/Texture2D"),
				new BlackboardVariableOption(typeof(Sprite), "Resources/Sprite"),
				new BlackboardVariableOption(typeof(Material), "Resources/Material"),
				new BlackboardVariableOption(typeof(AudioClip), "Resources/Audio Clip"),
				new BlackboardVariableOption(typeof(AnimationClip), "Resources/Animation Clip"),
				new BlackboardVariableOption(typeof(AudioMixer), "Resources/Audio Mixer"),
				new BlackboardVariableOption(typeof(TextAsset), "Resources/Text Asset"),
				new BlackboardVariableOption(typeof(ParticleSystem), "Resources/Particle System"),

				// List Types
				new BlackboardVariableOption(typeof(List<GameObject>), "List/Game Object List"),
				new BlackboardVariableOption(typeof(List<string>), "List/String List"),
				new BlackboardVariableOption(typeof(List<float>), "List/Float List"),
				new BlackboardVariableOption(typeof(List<int>), "List/Integer List"),
				new BlackboardVariableOption(typeof(List<double>), "List/Double List"),
				new BlackboardVariableOption(typeof(List<bool>), "List/Boolean List"),
				new BlackboardVariableOption(typeof(List<Vector2>), "List/Vector2 List"),
				new BlackboardVariableOption(typeof(List<Vector3>), "List/Vector3 List"),
				new BlackboardVariableOption(typeof(List<Vector4>), "List/Vector4 List"),
				new BlackboardVariableOption(typeof(List<Vector2Int>), "List/Vector2 Int List"),
				new BlackboardVariableOption(typeof(List<Vector3Int>), "List/Vector3 Int List"),
				new BlackboardVariableOption(typeof(List<Color>), "List/Color List"),
			};
		}

		public static List<BlackboardVariableOption> GetEnumVariableTypes()
		{
			var enumOptions = new List<BlackboardVariableOption>();
			IEnumerable<Type> enumTypes = TypeCache.GetTypesWithAttribute<BlackboardEnumAttribute>()
				.Where(type => type.IsEnum && Enum.GetValues(type).Length > 0);

			foreach (var type in enumTypes)
			{
				enumOptions.Add(new BlackboardVariableOption(type, "Enumeration/" + BlackboardUtilities.NicifyVariableName(type.Name)));
			}

			return enumOptions;
		}

		public static List<BlackboardVariableOption> GetBlackboardCategoryTypes()
		{
			var options = new List<BlackboardVariableOption>();
			IEnumerable<Type> types = TypeCache.GetTypesWithAttribute<BlackboardCategoryAttribute>()
				.Where(type => !type.IsAbstract && !type.IsInterface && type.GetCustomAttribute<SerializableAttribute>() != null);
			
			foreach (Type type in types)
			{
				var attribute = type.GetCustomAttribute<BlackboardCategoryAttribute>();
				string path = string.IsNullOrEmpty(attribute.Path) ? $"Other/{type.FullName.Replace(".", "/")}" : $"{attribute.Path}/{type.Name}";
				options.Add(new BlackboardVariableOption(type, path, "cs Script Icon"));
			}
			
			return options;
		}

		public static List<BlackboardVariableOption> GetStoryVariableTypes()
		{
			List<BlackboardVariableOption> options = GetDefaultBlackboardOptions();
			List<BlackboardVariableOption> enums = GetEnumVariableTypes();
			List<BlackboardVariableOption> categories = GetBlackboardCategoryTypes();

			AddCustomTypes<Component>(options, "Other/Components", "cs Script Icon");
			AddCustomTypes<ScriptableObject>(options, "Other/ScriptableObjects", "ScriptableObject Icon");

			options.AddRange(enums);
			options.AddRange(categories);

			return options;
		}
        
		public static List<BlackboardVariableOption> GetCustomTypes()
		{
			if (_customVariableOptions.Count == 0)
			{
				AddCustomTypes<Component>(_customVariableOptions, "Components", "cs Script Icon");
				AddCustomTypes<ScriptableObject>(_customVariableOptions, "ScriptableObjects", "ScriptableObject Icon");
			}

			return _customVariableOptions;
		}

		private static List<BlackboardVariableOption> AddCustomTypes<TypeName>(List<BlackboardVariableOption> options, string path, string icon = null)
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
				var option = new BlackboardVariableOption(type, fullPath, icon);
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