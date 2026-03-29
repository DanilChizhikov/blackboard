using System;

namespace DTech.Blackboard
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class BlackboardCategoryAttribute : Attribute
	{
		public string Path { get; }

		public BlackboardCategoryAttribute()
		{
			Path = string.Empty;
		}

		public BlackboardCategoryAttribute(string path)
		{
			Path = path;
		}
	}
}