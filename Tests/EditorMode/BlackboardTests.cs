using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace DTech.Blackboard.Tests.EditorMode
{
	[TestFixture]
	internal sealed class BlackboardTests
	{
		private Blackboard _blackboard;
		
		[SetUp]
		public void Setup()
		{
			_blackboard = new Blackboard();
		}
		
		[TearDown]
		public void TearDown()
		{
			_blackboard?.Dispose();
		}

		[Test]
		public void Constructor_Default_CreatesEmptyBlackboard()
		{
			var blackboard = new Blackboard();
			
			Assert.That(blackboard.VariableCount, Is.EqualTo(0));
			
			blackboard.Dispose();
		}

		[Test]
		public void Constructor_WithCapacity_CreatesEmptyBlackboard()
		{
			var blackboard = new Blackboard(10);
			
			Assert.That(blackboard.VariableCount, Is.EqualTo(0));
			
			blackboard.Dispose();
		}

		[Test]
		public void Constructor_WithNullVariables_CreatesEmptyBlackboard()
		{
			var blackboard = new Blackboard((IEnumerable<BlackboardVariable>)null);
			
			Assert.That(blackboard.VariableCount, Is.EqualTo(0));
			
			blackboard.Dispose();
		}

		[Test]
		public void Constructor_WithVariables_AddsAllVariables()
		{
			var var1 = new BlackboardVariable<int> { Name = "Health", Value = 100 };
			var var2 = new BlackboardVariable<string> { Name = "Name", Value = "Player" };
			var variables = new List<BlackboardVariable> { var1, var2 };
			
			var blackboard = new Blackboard(variables);
			
			Assert.That(blackboard.VariableCount, Is.EqualTo(2));
			Assert.That(blackboard.TryGetVariable("Health", out BlackboardVariable found1), Is.True);
			Assert.That(found1, Is.SameAs(var1));
			Assert.That(blackboard.TryGetVariable("Name", out BlackboardVariable found2), Is.True);
			Assert.That(found2, Is.SameAs(var2));
			
			blackboard.Dispose();
		}

		[Test]
		public void VariableCount_WhenNoVariables_ReturnsZero()
		{
			Assert.That(_blackboard.VariableCount, Is.EqualTo(0));
		}

		[Test]
		public void VariableCount_AfterAddingVariables_ReturnsCorrectCount()
		{
			var var1 = new BlackboardVariable<int> { Name = "Var1", Value = 1 };
			var var2 = new BlackboardVariable<float> { Name = "Var2", Value = 2f };
			
			_blackboard = new Blackboard(new List<BlackboardVariable> { var1, var2 });
			
			Assert.That(_blackboard.VariableCount, Is.EqualTo(2));
		}

		[Test]
		public void TryGetVariable_ByName_WhenVariableExists_ReturnsTrueAndVariable()
		{
			var variable = new BlackboardVariable<int> { Name = "Score", Value = 50 };
			_blackboard = new Blackboard(new List<BlackboardVariable> { variable });
			
			bool result = _blackboard.TryGetVariable("Score", out BlackboardVariable found);
			
			Assert.That(result, Is.True);
			Assert.That(found, Is.SameAs(variable));
		}

		[Test]
		public void TryGetVariable_ByName_WhenVariableDoesNotExist_ReturnsFalseAndNull()
		{
			bool result = _blackboard.TryGetVariable("NonExistent", out BlackboardVariable found);
			
			Assert.That(result, Is.False);
			Assert.That(found, Is.Null);
		}

		[Test]
		public void TryGetVariable_Generic_ByName_WhenVariableExistsAndTypeMatches_ReturnsTrueAndTypedVariable()
		{
			var variable = new BlackboardVariable<int> { Name = "Count", Value = 42 };
			_blackboard = new Blackboard(new List<BlackboardVariable> { variable });
			
			bool result = _blackboard.TryGetVariable<int>("Count", out BlackboardVariable<int> found);
			
			Assert.That(result, Is.True);
			Assert.That(found, Is.SameAs(variable));
			Assert.That(found.Value, Is.EqualTo(42));
		}

		[Test]
		public void TryGetVariable_Generic_ByName_WhenVariableExistsButTypeDoesNotMatch_ReturnsFalseAndNull()
		{
			var variable = new BlackboardVariable<int> { Name = "Number", Value = 10 };
			_blackboard = new Blackboard(new List<BlackboardVariable> { variable });
			
			bool result = _blackboard.TryGetVariable<string>("Number", out BlackboardVariable<string> found);
			
			Assert.That(result, Is.False);
			Assert.That(found, Is.Null);
		}

		[Test]
		public void TryGetVariable_Generic_ByName_WhenVariableDoesNotExist_ReturnsFalseAndNull()
		{
			bool result = _blackboard.TryGetVariable<float>("Missing", out BlackboardVariable<float> found);
			
			Assert.That(result, Is.False);
			Assert.That(found, Is.Null);
		}

		[Test]
		public void TryGetVariable_ByGuid_WhenVariableExists_ReturnsTrueAndVariable()
		{
			var variable = new BlackboardVariable<Vector3> { Name = "Position", Value = Vector3.one };
			_blackboard = new Blackboard(new List<BlackboardVariable> { variable });
			var guid = variable.Guid;
			
			bool result = _blackboard.TryGetVariable(guid, out BlackboardVariable found);
			
			Assert.That(result, Is.True);
			Assert.That(found, Is.SameAs(variable));
		}

		[Test]
		public void TryGetVariable_ByGuid_WhenVariableDoesNotExist_ReturnsFalseAndNull()
		{
			var nonExistentGuid = SerializableGuid.Generate();
			
			bool result = _blackboard.TryGetVariable(nonExistentGuid, out BlackboardVariable found);
			
			Assert.That(result, Is.False);
			Assert.That(found, Is.Null);
		}

		[Test]
		public void TryGetVariable_Generic_ByGuid_WhenVariableExistsAndTypeMatches_ReturnsTrueAndTypedVariable()
		{
			var variable = new BlackboardVariable<bool> { Name = "IsActive", Value = true };
			_blackboard = new Blackboard(new List<BlackboardVariable> { variable });
			var guid = variable.Guid;
			
			bool result = _blackboard.TryGetVariable<bool>(guid, out BlackboardVariable<bool> found);
			
			Assert.That(result, Is.True);
			Assert.That(found, Is.SameAs(variable));
		}

		[Test]
		public void TryGetVariable_Generic_ByGuid_WhenVariableExistsButTypeDoesNotMatch_ReturnsFalseAndNull()
		{
			var variable = new BlackboardVariable<float> { Name = "Speed", Value = 5.5f };
			_blackboard = new Blackboard(new List<BlackboardVariable> { variable });
			var guid = variable.Guid;
			
			bool result = _blackboard.TryGetVariable<int>(guid, out BlackboardVariable<int> found);
			
			Assert.That(result, Is.False);
			Assert.That(found, Is.Null);
		}

		[Test]
		public void TryGetVariable_Generic_ByGuid_WhenVariableDoesNotExist_ReturnsFalseAndNull()
		{
			var nonExistentGuid = SerializableGuid.Generate();
			
			bool result = _blackboard.TryGetVariable<double>(nonExistentGuid, out BlackboardVariable<double> found);
			
			Assert.That(result, Is.False);
			Assert.That(found, Is.Null);
		}

		[Test]
		public void GetVariablesNonAlloc_WhenArraySizeMatches_CopiesAllVariables()
		{
			var var1 = new BlackboardVariable<int> { Name = "Var1", Value = 1 };
			var var2 = new BlackboardVariable<int> { Name = "Var2", Value = 2 };
			var var3 = new BlackboardVariable<int> { Name = "Var3", Value = 3 };
			_blackboard = new Blackboard(new List<BlackboardVariable> { var1, var2, var3 });
			
			var array = new BlackboardVariable[3];
			int count = _blackboard.GetVariablesNonAlloc(array);
			
			Assert.That(count, Is.EqualTo(3));
			Assert.That(array[0], Is.Not.Null);
			Assert.That(array[1], Is.Not.Null);
			Assert.That(array[2], Is.Not.Null);
		}

		[Test]
		public void GetVariablesNonAlloc_WhenArraySmaller_CopiesOnlyWhatFits()
		{
			var var1 = new BlackboardVariable<int> { Name = "Var1", Value = 1 };
			var var2 = new BlackboardVariable<int> { Name = "Var2", Value = 2 };
			var var3 = new BlackboardVariable<int> { Name = "Var3", Value = 3 };
			_blackboard = new Blackboard(new List<BlackboardVariable> { var1, var2, var3 });
			
			var array = new BlackboardVariable[2];
			int count = _blackboard.GetVariablesNonAlloc(array);
			
			Assert.That(count, Is.EqualTo(2));
			Assert.That(array[0], Is.Not.Null);
			Assert.That(array[1], Is.Not.Null);
		}

		[Test]
		public void GetVariablesNonAlloc_WhenNoVariables_ReturnsZero()
		{
			var array = new BlackboardVariable[5];
			
			int count = _blackboard.GetVariablesNonAlloc(array);
			
			Assert.That(count, Is.EqualTo(0));
		}

		[Test]
		public void GetVariablesNonAlloc_WhenEmptyArray_ReturnsZero()
		{
			var var1 = new BlackboardVariable<int> { Name = "Var1", Value = 1 };
			_blackboard = new Blackboard(new List<BlackboardVariable> { var1 });
			
			var array = new BlackboardVariable[0];
			int count = _blackboard.GetVariablesNonAlloc(array);
			
			Assert.That(count, Is.EqualTo(0));
		}

		[Test]
		public void Dispose_ClearsAllVariables()
		{
			var var1 = new BlackboardVariable<int> { Name = "Var1", Value = 1 };
			var var2 = new BlackboardVariable<string> { Name = "Var2", Value = "test" };
			_blackboard = new Blackboard(new List<BlackboardVariable> { var1, var2 });
			
			_blackboard.Dispose();
			
			Assert.That(_blackboard.VariableCount, Is.EqualTo(0));
			_blackboard = null;
		}

		[Test]
		public void MultipleVariables_CanBeRetrievedIndependently()
		{
			var intVar = new BlackboardVariable<int> { Name = "IntVar", Value = 42 };
			var floatVar = new BlackboardVariable<float> { Name = "FloatVar", Value = 3.14f };
			var stringVar = new BlackboardVariable<string> { Name = "StringVar", Value = "hello" };
			var boolVar = new BlackboardVariable<bool> { Name = "BoolVar", Value = true };
			
			_blackboard = new Blackboard(new List<BlackboardVariable> { intVar, floatVar, stringVar, boolVar });
			
			Assert.That(_blackboard.TryGetVariable<int>("IntVar", out var foundInt), Is.True);
			Assert.That(foundInt.Value, Is.EqualTo(42));
			
			Assert.That(_blackboard.TryGetVariable<float>("FloatVar", out var foundFloat), Is.True);
			Assert.That(foundFloat.Value, Is.EqualTo(3.14f));
			
			Assert.That(_blackboard.TryGetVariable<string>("StringVar", out var foundString), Is.True);
			Assert.That(foundString.Value, Is.EqualTo("hello"));
			
			Assert.That(_blackboard.TryGetVariable<bool>("BoolVar", out var foundBool), Is.True);
			Assert.That(foundBool.Value, Is.EqualTo(true));
		}

		[Test]
		public void Variable_CanBeRetrievedByGuid_AfterBeingAdded()
		{
			var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 100 };
			var originalGuid = variable.Guid;
			_blackboard = new Blackboard(new List<BlackboardVariable> { variable });
			
			var retrievedByGuid = _blackboard.TryGetVariable(originalGuid, out BlackboardVariable byGuidResult);
			var retrievedByName = _blackboard.TryGetVariable("TestVar", out BlackboardVariable byNameResult);
			
			Assert.That(retrievedByGuid, Is.True);
			Assert.That(retrievedByName, Is.True);
			Assert.That(byGuidResult, Is.SameAs(variable));
			Assert.That(byNameResult, Is.SameAs(variable));
			Assert.That(byGuidResult.Guid, Is.EqualTo(originalGuid));
		}
	}
}