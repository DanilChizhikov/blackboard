using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;

namespace DTech.Blackboard.Editor
{
	internal static class BlackboardVariableDrawerProvider
	{
		private static readonly Dictionary<Type, BlackboardVariableDrawer> _drawersMap = new();
		
		public static BlackboardVariableDrawer GetDrawerForType(Type valueType)
		{
			if (!_drawersMap.TryGetValue(valueType, out BlackboardVariableDrawer drawer))
			{
				drawer = CreateGenericDrawer(valueType);
				_drawersMap.Add(valueType, drawer);
			}

			return drawer;
		}

		[InitializeOnLoadMethod, DidReloadScripts]
		private static void Initialize()
		{
			TypeCache.TypeCollection collection = TypeCache.GetTypesDerivedFrom<BlackboardVariableDrawer>();
			collection.Where(type => !type.IsAbstract && !type.IsGenericType && !_drawersMap.ContainsKey(type))
				.ToList()
				.ForEach(type =>
				{
					var drawer = (BlackboardVariableDrawer)Activator.CreateInstance(type);
					_drawersMap.Add(drawer.ServicedValueType, drawer);
				});
		}

		private static BlackboardVariableDrawer CreateGenericDrawer(Type valueType)
		{
			Type drawerType = typeof(BlackboardVariableDrawer<>).MakeGenericType(valueType);
			var drawer = (BlackboardVariableDrawer)Activator.CreateInstance(drawerType);
			return drawer;
		}
	}
}