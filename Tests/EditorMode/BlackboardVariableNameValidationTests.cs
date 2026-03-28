using System.Collections.Generic;
using NUnit.Framework;

namespace DTech.Blackboard.Tests.EditorMode
{
	[TestFixture]
	internal sealed class BlackboardVariableNameValidationTests
	{
		[Test]
		public void TryValidate_Add_WithTrimAndIgnoreCase_DetectsDuplicate()
		{
			var variables = new List<BlackboardVariable>
			{
				new BlackboardVariable<int> { Name = "Health", Value = 100 }
			};

			bool result = BlackboardVariableNameValidator.TryValidate(
				variables,
				" health ",
				null,
				out string normalizedName,
				out string errorMessage);

			Assert.That(result, Is.False);
			Assert.That(normalizedName, Is.EqualTo("health"));
			Assert.That(errorMessage, Does.Contain("already used"));
		}

		[Test]
		public void TryValidate_Add_WithUpperCaseDuplicate_DetectsDuplicate()
		{
			var variables = new List<BlackboardVariable>
			{
				new BlackboardVariable<int> { Name = "Health", Value = 100 }
			};

			bool result = BlackboardVariableNameValidator.TryValidate(
				variables,
				"HEALTH",
				null,
				out _,
				out _);

			Assert.That(result, Is.False);
		}

		[Test]
		public void TryValidate_Rename_ExcludesCurrentVariableGuid()
		{
			var currentVariable = new BlackboardVariable<int> { Name = "Health", Value = 100 };
			var variables = new List<BlackboardVariable>
			{
				currentVariable
			};

			bool result = BlackboardVariableNameValidator.TryValidate(
				variables,
				" health ",
				currentVariable.Guid,
				out string normalizedName,
				out string errorMessage);

			Assert.That(result, Is.True);
			Assert.That(normalizedName, Is.EqualTo("health"));
			Assert.That(errorMessage, Is.Null);
		}

		[Test]
		public void TryValidate_Add_WithUniqueName_ReturnsTrue()
		{
			var variables = new List<BlackboardVariable>
			{
				new BlackboardVariable<int> { Name = "Health", Value = 100 }
			};

			bool result = BlackboardVariableNameValidator.TryValidate(
				variables,
				" speed ",
				null,
				out string normalizedName,
				out string errorMessage);

			Assert.That(result, Is.True);
			Assert.That(normalizedName, Is.EqualTo("speed"));
			Assert.That(errorMessage, Is.Null);
		}
	}
}
