namespace DTech.Blackboard.Editor
{
	internal sealed class BlackboardVariableOption
	{
		public string Name => System.IO.Path.GetFileName(Path);
		public string Path { get; }
		public SerializableType Type { get; }
		public string IconName { get; }
		public int Priority { get; }

		public BlackboardVariableOption(SerializableType type, string path = null, string iconName = null, int priority = 0)
		{
			Path = path;
			Type = type;
			IconName = iconName;
			Priority = priority;

			if (string.IsNullOrEmpty(path))
			{
				Path = BlackboardUtilities.NicifyVariableName(type.Type.Name);
			}
		}
	}
}