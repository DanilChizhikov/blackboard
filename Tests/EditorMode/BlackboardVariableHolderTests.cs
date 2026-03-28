using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DTech.Blackboard.Tests.EditorMode
{
	[TestFixture]
	internal sealed class BlackboardVariableHolderTests
	{
		private BlackboardVariableHolder _holder;
		
		[SetUp]
		public void Setup()
		{
			_holder = new BlackboardVariableHolder();
		}
		
		[TearDown]
		public void TearDown()
		{
			_holder.Dispose();
		}

		[Test]
		public void Constructor_Default_CreatesEmptyHolder()
		{
			var holder = new BlackboardVariableHolder();
			
			Assert.That(holder.Contains("anyName"), Is.False);
			
			holder.Dispose();
		}

		[Test]
		public void Constructor_WithCapacity_CreatesHolder()
		{
			var holder = new BlackboardVariableHolder(10);
			
			Assert.That(holder.Contains("anyName"), Is.False);
			
			holder.Dispose();
		}

		[Test]
		public void Constructor_WithVariables_AddsAllVariables()
		{
			var var1 = new BlackboardVariable<int> { Name = "Var1", Value = 10 };
			var var2 = new BlackboardVariable<string> { Name = "Var2", Value = "test" };
			var variables = new List<BlackboardVariable> { var1, var2 };
			
			var holder = new BlackboardVariableHolder(variables);
			
			Assert.That(holder.Contains("Var1"), Is.True);
			Assert.That(holder.Contains("Var2"), Is.True);
			Assert.That(holder["Var1"], Is.SameAs(var1));
			Assert.That(holder["Var2"], Is.SameAs(var2));
			
			holder.Dispose();
		}

		[Test]
		public void Add_WhenVariableNotExists_AddsVariable()
		{
			var variable = new BlackboardVariable<float> { Name = "Health", Value = 100f };
			
			_holder.Add(variable);
			
			Assert.That(_holder.Contains("Health"), Is.True);
			Assert.That(_holder["Health"], Is.SameAs(variable));
		}

		[Test]
		public void Add_WhenVariableAlreadyExists_DoesNotAddDuplicate()
		{
			var var1 = new BlackboardVariable<int> { Name = "Score", Value = 100 };
			var var2 = new BlackboardVariable<int> { Name = "Score", Value = 200 };
			
			_holder.Add(var1);
			LogAssert.Expect(LogType.Error, "BlackboardVariableHolder.Add: Variable with name 'Score' already exists");
			_holder.Add(var2);
			
			Assert.That(_holder["Score"], Is.SameAs(var1));
		}

		[Test]
		public void Add_WhenVariableAlreadyExists_LogsError()
		{
			var var1 = new BlackboardVariable<int> { Name = "Score", Value = 100 };
			var var2 = new BlackboardVariable<int> { Name = "Score", Value = 200 };
			
			_holder.Add(var1);
			LogAssert.Expect(LogType.Error, "BlackboardVariableHolder.Add: Variable with name 'Score' already exists");
			_holder.Add(var2);
			
			Assert.That(_holder["Score"], Is.SameAs(var1));
		}

		[Test]
		public void Contains_WhenVariableExists_ReturnsTrue()
		{
			var variable = new BlackboardVariable<bool> { Name = "IsActive", Value = true };
			_holder.Add(variable);
			
			Assert.That(_holder.Contains("IsActive"), Is.True);
		}

		[Test]
		public void Contains_WhenVariableDoesNotExist_ReturnsFalse()
		{
			Assert.That(_holder.Contains("NonExistent"), Is.False);
		}

		[Test]
		public void Indexer_ByName_WhenVariableExists_ReturnsVariable()
		{
			var variable = new BlackboardVariable<string> { Name = "PlayerName", Value = "Hero" };
			_holder.Add(variable);
			
			var result = _holder["PlayerName"];
			
			Assert.That(result, Is.SameAs(variable));
		}

		[Test]
		public void Indexer_ByName_WhenVariableDoesNotExist_ThrowsKeyNotFoundException()
		{
			Assert.Throws<KeyNotFoundException>(() => { var _ = _holder["NonExistent"]; });
		}

		[Test]
		public void Indexer_ByGuid_WhenVariableExists_ReturnsVariable()
		{
			var variable = new BlackboardVariable<double> { Name = "Position", Value = 3.14 };
			_holder.Add(variable);
			
			var result = _holder[variable.Guid];
			
			Assert.That(result, Is.SameAs(variable));
		}

		[Test]
		public void Indexer_ByGuid_WhenVariableDoesNotExist_ThrowsKeyNotFoundException()
		{
			var nonExistentGuid = SerializableGuid.Generate();
			
			Assert.Throws<KeyNotFoundException>(() => { var _ = _holder[nonExistentGuid]; });
		}

		[Test]
		public void TryGetVariable_ByName_WhenVariableExists_ReturnsTrueAndVariable()
		{
			var variable = new BlackboardVariable<int> { Name = "Level", Value = 5 };
			_holder.Add(variable);
			
			bool result = _holder.TryGetVariable("Level", out BlackboardVariable found);
			
			Assert.That(result, Is.True);
			Assert.That(found, Is.SameAs(variable));
		}

		[Test]
		public void TryGetVariable_ByName_WhenVariableDoesNotExist_ReturnsFalseAndNull()
		{
			bool result = _holder.TryGetVariable("NonExistent", out BlackboardVariable found);
			
			Assert.That(result, Is.False);
			Assert.That(found, Is.Null);
		}

		[Test]
		public void TryGetVariable_ByGuid_WhenVariableExists_ReturnsTrueAndVariable()
		{
			var variable = new BlackboardVariable<Vector3>() { Name = "Position", Value = Vector3.zero };
			_holder.Add(variable);
			
			bool result = _holder.TryGetVariable(variable.Guid, out BlackboardVariable found);
			
			Assert.That(result, Is.True);
			Assert.That(found, Is.SameAs(variable));
		}

		[Test]
		public void TryGetVariable_ByGuid_WhenVariableDoesNotExist_ReturnsFalseAndNull()
		{
			var nonExistentGuid = SerializableGuid.Generate();
			
			bool result = _holder.TryGetVariable(nonExistentGuid, out BlackboardVariable found);
			
			Assert.That(result, Is.False);
			Assert.That(found, Is.Null);
		}

		[Test]
		public void Remove_ByName_WhenVariableExists_ReturnsTrue()
		{
			var variable = new BlackboardVariable<int> { Name = "ToRemove", Value = 42 };
			_holder.Add(variable);
			
			bool result = _holder.Remove("ToRemove");
			
			Assert.That(result, Is.True);
			Assert.That(_holder.Contains("ToRemove"), Is.False);
		}

		[Test]
		public void Remove_ByName_WhenVariableDoesNotExist_ReturnsFalse()
		{
			bool result = _holder.Remove("NonExistent");
			
			Assert.That(result, Is.False);
		}

		[Test]
		public void Remove_ByName_WithOut_WhenVariableExists_ReturnsTrueAndVariable()
		{
			var variable = new BlackboardVariable<string> { Name = "ToRemove", Value = "value" };
			_holder.Add(variable);
			
			bool result = _holder.Remove("ToRemove", out BlackboardVariable removed);
			
			Assert.That(result, Is.True);
			Assert.That(removed, Is.SameAs(variable));
			Assert.That(_holder.Contains("ToRemove"), Is.False);
		}

		[Test]
		public void Remove_ByName_WithOut_WhenVariableDoesNotExist_ReturnsFalseAndNull()
		{
			bool result = _holder.Remove("NonExistent", out BlackboardVariable removed);
			
			Assert.That(result, Is.False);
			Assert.That(removed, Is.Null);
		}

		[Test]
		public void Remove_ByGuid_WhenVariableExists_ReturnsTrue()
		{
			var variable = new BlackboardVariable<float> { Name = "ToRemove", Value = 1.5f };
			_holder.Add(variable);
			var guid = variable.Guid;
			
			bool result = _holder.Remove(guid);
			
			Assert.That(result, Is.True);
			Assert.That(_holder.Contains("ToRemove"), Is.False);
		}

		[Test]
		public void Remove_ByGuid_WhenVariableDoesNotExist_ReturnsFalse()
		{
			var nonExistentGuid = SerializableGuid.Generate();
			
			bool result = _holder.Remove(nonExistentGuid);
			
			Assert.That(result, Is.False);
		}

		[Test]
		public void Remove_ByGuid_WithOut_WhenVariableExists_ReturnsTrueAndVariable()
		{
			var variable = new BlackboardVariable<bool> { Name = "ToRemove", Value = true };
			_holder.Add(variable);
			var guid = variable.Guid;
			
			bool result = _holder.Remove(guid, out BlackboardVariable removed);
			
			Assert.That(result, Is.True);
			Assert.That(removed, Is.SameAs(variable));
			Assert.That(_holder.Contains("ToRemove"), Is.False);
		}

		[Test]
		public void Remove_ByGuid_WithOut_WhenVariableDoesNotExist_ReturnsFalseAndNull()
		{
			var nonExistentGuid = SerializableGuid.Generate();
			
			bool result = _holder.Remove(nonExistentGuid, out BlackboardVariable removed);
			
			Assert.That(result, Is.False);
			Assert.That(removed, Is.Null);
		}

		[Test]
		public void Replace_WhenOriginalVariableExists_ReplacesWithNewVariable()
		{
			var original = new BlackboardVariable<int> { Name = "Counter", Value = 10 };
			var replacement = new BlackboardVariable<int> { Name = "Counter", Value = 20 };
			_holder.Add(original);
			
			_holder.Replace("Counter", replacement);
			
			Assert.That(_holder["Counter"], Is.SameAs(replacement));
			Assert.That(_holder["Counter"].ObjectValue, Is.EqualTo(20));
		}

		[Test]
		public void Replace_WhenOriginalVariableDoesNotExist_DoesNotAddReplacement()
		{
			var replacement = new BlackboardVariable<int> { Name = "NonExistent", Value = 20 };
			
			LogAssert.Expect(LogType.Error, "BlackboardVariableHolder.Replace: Variable with name 'NonExistent' not found");
			
			Assert.IsFalse(_holder.Replace("NonExistent", replacement));
		}

		[Test]
		public void Replace_WhenVariableHasDifferentName_DoesNotReplace()
		{
			var origin = new BlackboardVariable<int> { Name = "NonExistent", Value = 20 };
			var replacement = new BlackboardVariable<int> { Name = "Replace", Value = 20 };
			
			_holder.Add(origin);
			
			LogAssert.Expect(LogType.Error, "BlackboardVariableHolder.Replace: Variable with name 'NonExistent' cannot be replaced with 'Replace'");
			Assert.IsFalse(_holder.Replace("NonExistent", replacement));
		}

		[Test]
		public void Dispose_ClearsAllVariables()
		{
			var var1 = new BlackboardVariable<int> { Name = "Var1", Value = 1 };
			var var2 = new BlackboardVariable<int> { Name = "Var2", Value = 2 };
			_holder.Add(var1);
			_holder.Add(var2);
			
			_holder.Dispose();
			
			Assert.That(_holder.Contains("Var1"), Is.False);
			Assert.That(_holder.Contains("Var2"), Is.False);
		}

		[Test]
		public void MultipleVariables_CanBeAddedAndRetrievedIndependently()
		{
			var intVar = new BlackboardVariable<int> { Name = "IntVar", Value = 42 };
			var floatVar = new BlackboardVariable<float> { Name = "FloatVar", Value = 3.14f };
			var stringVar = new BlackboardVariable<string> { Name = "StringVar", Value = "test" };
			var boolVar = new BlackboardVariable<bool> { Name = "BoolVar", Value = true };
			
			_holder.Add(intVar);
			_holder.Add(floatVar);
			_holder.Add(stringVar);
			_holder.Add(boolVar);
			
			Assert.That(_holder["IntVar"].ObjectValue, Is.EqualTo(42));
			Assert.That(_holder["FloatVar"].ObjectValue, Is.EqualTo(3.14f));
			Assert.That(_holder["StringVar"].ObjectValue, Is.EqualTo("test"));
			Assert.That(_holder["BoolVar"].ObjectValue, Is.EqualTo(true));
		}

		[Test]
		public void Variable_CanBeRetrievedByGuid_AfterBeingAdded()
		{
			var variable = new BlackboardVariable<int> { Name = "MyVar", Value = 100 };
			var originalGuid = variable.Guid;
			_holder.Add(variable);
			
			var retrievedByGuid = _holder[originalGuid];
			var retrievedByName = _holder["MyVar"];
			
			Assert.That(retrievedByGuid, Is.SameAs(variable));
			Assert.That(retrievedByName, Is.SameAs(variable));
			Assert.That(retrievedByGuid.Guid, Is.EqualTo(originalGuid));
		}
	}
}