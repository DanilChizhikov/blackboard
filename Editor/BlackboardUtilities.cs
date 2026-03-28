using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace DTech.Blackboard.Editor
{
	internal static class BlackboardUtilities
	{
		private static readonly List<Type> _staticallySupportedTypes = new()
		{
			typeof(GameObject),
			typeof(string),
			typeof(int),
			typeof(float),
			typeof(double),
			typeof(bool),
			typeof(Vector2),
			typeof(Vector3),
			typeof(Vector4),
			typeof(Vector2Int),
			typeof(Vector3Int),
			typeof(Color),
			typeof(List<int>),
			typeof(List<float>),
			typeof(List<double>),
			typeof(List<bool>),
			typeof(List<string>),
			typeof(List<GameObject>),
			typeof(List<Vector2>),
			typeof(List<Vector3>),
			typeof(List<Vector4>),
			typeof(List<Vector2Int>),
			typeof(List<Vector3Int>),
			typeof(List<Color>)
		};

		public static IEnumerable<Type> GetSupportedTypes() => _staticallySupportedTypes.Concat(GetEnumVariableTypes());

		private static bool IsCapitalCharacter(char character)
		{
			return character >= 'A' && character <= 'Z';
		}

		private static bool IsNumberCharacter(char character)
		{
			return character >= '0' && character <= '9';
		}

		public static string NicifyVariableName(string input, bool detectAbbreviation = false)
		{
			System.Text.StringBuilder output = new System.Text.StringBuilder();
			char[] inputArray = input.ToCharArray();
			int startIndex = 0;

			if (inputArray.Length > 1 &&
				inputArray[0] == 'm' &&
				input[1] == '_')
			{
				startIndex += 2;
			}

			if (inputArray.Length > 1 &&
				inputArray[0] == 'k' &&
				inputArray[1] >= 'A' &&
				inputArray[1] <= 'Z')
			{
				startIndex += 1;
			}

			if (inputArray.Length > 0 &&
				inputArray[0] >= 'a' &&
				inputArray[0] <= 'z')
			{
				inputArray[0] -= (char)('a' - 'A');
			}

			for (int i = startIndex; i < inputArray.Length; ++i)
			{
				if (inputArray[i] == '_')
				{
					output.Append(' ');
					continue;
				}

				if (IsCapitalCharacter(inputArray[i]))
				{
					bool IsAbbreviation()
					{
						return i > 0 &&
							output[output.Length - 1] != ' ' &&
							!(IsCapitalCharacter(inputArray[i - 1]) || IsNumberCharacter(inputArray[i - 1]));
					}

					if (!detectAbbreviation ||
						IsAbbreviation())
					{
						output.Append(' ');
					}
				}

				output.Append(inputArray[i]);
			}

			return output.ToString().TrimStart(' ');
		}

		public static IEnumerable<Type> GetEnumVariableTypes() =>
			TypeCache.GetTypesWithAttribute<BlackboardEnumAttribute>()
				.Where(type => type.IsEnum && Enum.GetValues(type).Length > 0);
	}
}