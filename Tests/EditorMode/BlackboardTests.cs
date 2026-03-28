using NUnit.Framework;

namespace DTech.Blackboard.Tests.EditorMode
{
	[TestFixture]
	internal sealed class BlackboardTests
	{
		private Blackboard _blackboard;
		
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			_blackboard = new Blackboard();
		}
		
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			_blackboard.Dispose();
		}
	}
}