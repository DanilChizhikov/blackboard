using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace DTech.Blackboard.Tests.EditorMode
{
	[TestFixture]
	internal sealed class BlackboardTests
	{
		private RuntimeBlackboard _blackboard;
		
		[SetUp]
		public void Setup()
		{
			_blackboard = new RuntimeBlackboard();
		}
		
		[TearDown]
		public void TearDown()
		{
			_blackboard?.Dispose();
		}

		[Test]
		public void Constructor_Default_CreatesEmptyBlackboard()
		{
			var blackboard = new RuntimeBlackboard();
			
			Assert.That(blackboard.VariableCount, Is.EqualTo(0));
			
			blackboard.Dispose();
		}

		[Test]
		public void Constructor_WithCapacity_CreatesEmptyBlackboard()
		{
			var blackboard = new RuntimeBlackboard(10);
			
			Assert.That(blackboard.VariableCount, Is.EqualTo(0));
			
			blackboard.Dispose();
		}

		[Test]
		public void Constructor_WithNullVariables_CreatesEmptyBlackboard()
		{
			var blackboard = new RuntimeBlackboard((IEnumerable<BlackboardVariable>)null);
			
			Assert.That(blackboard.VariableCount, Is.EqualTo(0));
			
			blackboard.Dispose();
		}

		[Test]
		public void Constructor_WithVariables_AddsAllVariables()
		{
			var var1 = new BlackboardVariable<int> { Name = "Health", Value = 100 };
			var var2 = new BlackboardVariable<string> { Name = "Name", Value = "Player" };
			var variables = new List<BlackboardVariable> { var1, var2 };
			
			var blackboard = new RuntimeBlackboard(variables);
			
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
			
			_blackboard.Add(var1);
			_blackboard.Add(var2);
			
			Assert.That(_blackboard.VariableCount, Is.EqualTo(2));
		}

		[Test]
		public void Add_WhenVariableNotExists_AddsVariable()
		{
			var variable = new BlackboardVariable<int> { Name = "Score", Value = 100 };
			
			_blackboard.Add(variable);
			
			Assert.That(_blackboard.VariableCount, Is.EqualTo(1));
			Assert.That(_blackboard.TryGetVariable("Score", out BlackboardVariable found), Is.True);
			Assert.That(found, Is.SameAs(variable));
		}

		[Test]
		public void Add_WhenVariableAlreadyExists_DoesNotAddDuplicate()
		{
			var var1 = new BlackboardVariable<int> { Name = "Score", Value = 100 };
			var var2 = new BlackboardVariable<int> { Name = "Score", Value = 200 };
			
			_blackboard.Add(var1);
			LogAssert.Expect(LogType.Error, "BlackboardVariableHolder.Add: Variable name 'Score' is already used by 'Score'.");
			_blackboard.Add(var2);
			
			Assert.That(_blackboard.VariableCount, Is.EqualTo(1));
			Assert.That(_blackboard.TryGetVariable("Score", out BlackboardVariable found), Is.True);
			Assert.That(found, Is.SameAs(var1));
		}

		[Test]
		public void Remove_ByName_WhenVariableExists_ReturnsTrue()
		{
			var variable = new BlackboardVariable<int> { Name = "ToRemove", Value = 42 };
			_blackboard.Add(variable);
			
			bool result = _blackboard.Remove("ToRemove");
			
			Assert.That(result, Is.True);
			Assert.That(_blackboard.VariableCount, Is.EqualTo(0));
		}

		[Test]
		public void Remove_ByName_WhenVariableDoesNotExist_ReturnsFalse()
		{
			bool result = _blackboard.Remove("NonExistent");
			
			Assert.That(result, Is.False);
		}

		[Test]
		public void Remove_ByName_WithOut_WhenVariableExists_ReturnsTrueAndVariable()
		{
			var variable = new BlackboardVariable<string> { Name = "ToRemove", Value = "value" };
			_blackboard.Add(variable);
			
			bool result = _blackboard.Remove("ToRemove", out BlackboardVariable removed);
			
			Assert.That(result, Is.True);
			Assert.That(removed, Is.SameAs(variable));
			Assert.That(_blackboard.VariableCount, Is.EqualTo(0));
		}

		[Test]
		public void Remove_ByName_WithOut_WhenVariableDoesNotExist_ReturnsFalseAndNull()
		{
			bool result = _blackboard.Remove("NonExistent", out BlackboardVariable removed);
			
			Assert.That(result, Is.False);
			Assert.That(removed, Is.Null);
		}

		[Test]
		public void Remove_ByGuid_WhenVariableExists_ReturnsTrue()
		{
			var variable = new BlackboardVariable<float> { Name = "ToRemove", Value = 1.5f };
			_blackboard.Add(variable);
			var guid = variable.Guid;
			
			bool result = _blackboard.Remove(guid);
			
			Assert.That(result, Is.True);
			Assert.That(_blackboard.VariableCount, Is.EqualTo(0));
		}

		[Test]
		public void Remove_ByGuid_WhenVariableDoesNotExist_ReturnsFalse()
		{
			var nonExistentGuid = SerializableGuid.Generate();
			
			bool result = _blackboard.Remove(nonExistentGuid);
			
			Assert.That(result, Is.False);
		}

		[Test]
		public void Remove_ByGuid_WithOut_WhenVariableExists_ReturnsTrueAndVariable()
		{
			var variable = new BlackboardVariable<bool> { Name = "ToRemove", Value = true };
			_blackboard.Add(variable);
			var guid = variable.Guid;
			
			bool result = _blackboard.Remove(guid, out BlackboardVariable removed);
			
			Assert.That(result, Is.True);
			Assert.That(removed, Is.SameAs(variable));
			Assert.That(_blackboard.VariableCount, Is.EqualTo(0));
		}

		[Test]
		public void Remove_ByGuid_WithOut_WhenVariableDoesNotExist_ReturnsFalseAndNull()
		{
			var nonExistentGuid = SerializableGuid.Generate();
			
			bool result = _blackboard.Remove(nonExistentGuid, out BlackboardVariable removed);
			
			Assert.That(result, Is.False);
			Assert.That(removed, Is.Null);
		}

		[Test]
		public void Replace_ByName_WhenVariableExists_ReplacesVariable()
		{
			var original = new BlackboardVariable<int> { Name = "Counter", Value = 10 };
			var replacement = new BlackboardVariable<int> { Name = "Counter", Value = 20 };
			_blackboard.Add(original);
			
			bool result = _blackboard.Replace("Counter", replacement);
			
			Assert.That(result, Is.True);
			Assert.That(_blackboard.TryGetVariable("Counter", out BlackboardVariable found), Is.True);
			Assert.That(found, Is.SameAs(replacement));
			Assert.That(found.ObjectValue, Is.EqualTo(20));
		}

		[Test]
		public void Replace_ByName_WhenVariableDoesNotExist_ReturnsFalse()
		{
			var replacement = new BlackboardVariable<int> { Name = "NonExistent", Value = 20 };
			
			LogAssert.Expect(LogType.Error, "BlackboardVariableHolder.Replace: Variable with name 'NonExistent' not found");
			bool result = _blackboard.Replace("NonExistent", replacement);
			
			Assert.That(result, Is.False);
			Assert.That(_blackboard.VariableCount, Is.EqualTo(0));
		}

		[Test]
		public void Replace_ByGuid_WhenVariableExists_ReplacesVariable()
		{
			var original = new BlackboardVariable<float> { Name = "Speed", Value = 5f };
			var replacement = new BlackboardVariable<float> { Name = "Speed", Value = 10f };
			_blackboard.Add(original);
			var originalGuid = original.Guid;
			
			bool result = _blackboard.Replace(originalGuid, replacement);
			
			Assert.That(result, Is.True);
			Assert.That(_blackboard.TryGetVariable("Speed", out BlackboardVariable found), Is.True);
			Assert.That(found, Is.SameAs(replacement));
			Assert.That(found.Guid, Is.EqualTo(originalGuid));
		}

		[Test]
		public void Replace_ByGuid_WhenVariableDoesNotExist_ReturnsFalse()
		{
			var nonExistentGuid = SerializableGuid.Generate();
			var replacement = new BlackboardVariable<int> { Name = "Test", Value = 20 };
			
			LogAssert.Expect(LogType.Error, $"BlackboardVariableHolder.Replace: Variable with guid '{nonExistentGuid}' not found");
			bool result = _blackboard.Replace(nonExistentGuid, replacement);
			
			Assert.That(result, Is.False);
		}

		[Test]
		public void Replace_WhenVariableHasDifferentName_ReturnsFalse()
		{
			var original = new BlackboardVariable<int> { Name = "Original", Value = 10 };
			var replacement = new BlackboardVariable<int> { Name = "Different", Value = 20 };
			_blackboard.Add(original);
			
			LogAssert.Expect(LogType.Error, "BlackboardVariableHolder.Replace: Variable with name 'Original' cannot be replaced with 'Different'");
			bool result = _blackboard.Replace("Original", replacement);
			
			Assert.That(result, Is.False);
			Assert.That(_blackboard.TryGetVariable("Original", out BlackboardVariable found), Is.True);
			Assert.That(found, Is.SameAs(original));
		}

		[Test]
		public void TryGetVariable_ByName_WhenVariableExists_ReturnsTrueAndVariable()
		{
			var variable = new BlackboardVariable<int> { Name = "Score", Value = 50 };
			_blackboard.Add(variable);
			
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
			_blackboard.Add(variable);
			
			bool result = _blackboard.TryGetVariable<int>("Count", out BlackboardVariable<int> found);
			
			Assert.That(result, Is.True);
			Assert.That(found, Is.SameAs(variable));
			Assert.That(found.Value, Is.EqualTo(42));
		}

		[Test]
		public void TryGetVariable_Generic_ByName_WhenVariableExistsButTypeDoesNotMatch_ReturnsFalseAndNull()
		{
			var variable = new BlackboardVariable<int> { Name = "Number", Value = 10 };
			_blackboard.Add(variable);
			
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
			_blackboard.Add(variable);
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
			_blackboard.Add(variable);
			var guid = variable.Guid;
			
			bool result = _blackboard.TryGetVariable<bool>(guid, out BlackboardVariable<bool> found);
			
			Assert.That(result, Is.True);
			Assert.That(found, Is.SameAs(variable));
		}

		[Test]
		public void TryGetVariable_Generic_ByGuid_WhenVariableExistsButTypeDoesNotMatch_ReturnsFalseAndNull()
		{
			var variable = new BlackboardVariable<float> { Name = "Speed", Value = 5.5f };
			_blackboard.Add(variable);
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
			_blackboard.Add(var1);
			_blackboard.Add(var2);
			_blackboard.Add(var3);
			
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
			_blackboard.Add(var1);
			_blackboard.Add(var2);
			_blackboard.Add(var3);
			
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
			_blackboard.Add(var1);
			
			var array = new BlackboardVariable[0];
			int count = _blackboard.GetVariablesNonAlloc(array);
			
			Assert.That(count, Is.EqualTo(0));
		}

		[Test]
		public void Dispose_ClearsAllVariables()
		{
			var var1 = new BlackboardVariable<int> { Name = "Var1", Value = 1 };
			var var2 = new BlackboardVariable<string> { Name = "Var2", Value = "test" };
			_blackboard.Add(var1);
			_blackboard.Add(var2);
			
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
			
			_blackboard.Add(intVar);
			_blackboard.Add(floatVar);
			_blackboard.Add(stringVar);
			_blackboard.Add(boolVar);
			
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
			_blackboard.Add(variable);
			
			var retrievedByGuid = _blackboard.TryGetVariable(originalGuid, out BlackboardVariable byGuidResult);
			var retrievedByName = _blackboard.TryGetVariable("TestVar", out BlackboardVariable byNameResult);
			
			Assert.That(retrievedByGuid, Is.True);
			Assert.That(retrievedByName, Is.True);
			Assert.That(byGuidResult, Is.SameAs(variable));
			Assert.That(byNameResult, Is.SameAs(variable));
			Assert.That(byGuidResult.Guid, Is.EqualTo(originalGuid));
		}

		[Test]
		public void BlackboardVariable_Value_WhenChanged_InvokesOnValueChangedWithGuidAndValue()
		{
			var variable = new BlackboardVariable<int> { Name = "Observed", Value = 1 };
			int callCount = 0;
			SerializableGuid receivedGuid = default;
			int receivedValue = default;

			variable.OnValueChanged += (guid, value) =>
			{
				callCount++;
				receivedGuid = guid;
				receivedValue = value;
			};

			variable.Value = 5;

			Assert.That(callCount, Is.EqualTo(1));
			Assert.That(receivedGuid, Is.EqualTo(variable.Guid));
			Assert.That(receivedValue, Is.EqualTo(5));
		}

		[Test]
		public void BlackboardVariable_Value_WhenSetToSameValue_DoesNotInvokeOnValueChanged()
		{
			var variable = new BlackboardVariable<int> { Name = "Observed", Value = 10 };
			int callCount = 0;
			variable.OnValueChanged += (_, _) => callCount++;

			variable.Value = 10;

			Assert.That(callCount, Is.EqualTo(0));
		}

		[Test]
		public void BlackboardVariable_ObjectValueSetter_UsesSameNotificationRulesAsValueSetter()
		{
			var variable = new BlackboardVariable<int> { Name = "Observed", Value = 1 };
			int callCount = 0;
			int receivedValue = default;

			variable.OnValueChanged += (_, value) =>
			{
				callCount++;
				receivedValue = value;
			};

			variable.ObjectValue = 4;
			variable.ObjectValue = 4;

			Assert.That(callCount, Is.EqualTo(1));
			Assert.That(receivedValue, Is.EqualTo(4));
			Assert.That(variable.Value, Is.EqualTo(4));
		}

		[Test]
		public void BlackboardVariable_SetValueWithoutNotif_UpdatesValueWithoutInvokingOnValueChanged()
		{
			var variable = new BlackboardVariable<int> { Name = "Observed", Value = 1 };
			int callCount = 0;
			variable.OnValueChanged += (_, _) => callCount++;

			variable.SetValueWithoutNotif(9);

			Assert.That(variable.Value, Is.EqualTo(9));
			Assert.That(callCount, Is.EqualTo(0));
		}

		[Test]
		public void BlackboardVariable_SetObjectValueWithoutNotif_UpdatesValueWithoutInvokingOnValueChanged()
		{
			var variable = new BlackboardVariable<int> { Name = "Observed", Value = 1 };
			int callCount = 0;
			variable.OnValueChanged += (_, _) => callCount++;

			variable.SetObjectValueWithoutNotif(7);

			Assert.That(variable.Value, Is.EqualTo(7));
			Assert.That(callCount, Is.EqualTo(0));
		}

		[Test]
		public void BlackboardExtensions_SetValue_ByName_InvokesOnValueChanged()
		{
			var variable = new BlackboardVariable<int> { Name = "Health", Value = 10 };
			_blackboard.Add(variable);
			int callCount = 0;
			int receivedValue = default;
			variable.OnValueChanged += (_, value) =>
			{
				callCount++;
				receivedValue = value;
			};

			_blackboard.SetValue("Health", 15);

			Assert.That(variable.Value, Is.EqualTo(15));
			Assert.That(callCount, Is.EqualTo(1));
			Assert.That(receivedValue, Is.EqualTo(15));
		}

		[Test]
		public void BlackboardExtensions_SetValueWithoutNotif_ByName_UpdatesValueWithoutNotification()
		{
			var variable = new BlackboardVariable<int> { Name = "Health", Value = 10 };
			_blackboard.Add(variable);
			int callCount = 0;
			variable.OnValueChanged += (_, _) => callCount++;

			_blackboard.SetValueWithoutNotify("Health", 20);

			Assert.That(variable.Value, Is.EqualTo(20));
			Assert.That(callCount, Is.EqualTo(0));
		}

		[Test]
		public void BlackboardExtensions_SetValue_ByGuid_InvokesOnValueChanged()
		{
			var variable = new BlackboardVariable<int> { Name = "Health", Value = 10 };
			_blackboard.Add(variable);
			int callCount = 0;
			int receivedValue = default;
			variable.OnValueChanged += (_, value) =>
			{
				callCount++;
				receivedValue = value;
			};

			_blackboard.SetValue(variable.Guid, 30);

			Assert.That(variable.Value, Is.EqualTo(30));
			Assert.That(callCount, Is.EqualTo(1));
			Assert.That(receivedValue, Is.EqualTo(30));
		}

		[Test]
		public void BlackboardExtensions_SetValueWithoutNotif_ByGuid_UpdatesValueWithoutNotification()
		{
			var variable = new BlackboardVariable<int> { Name = "Health", Value = 10 };
			_blackboard.Add(variable);
			int callCount = 0;
			variable.OnValueChanged += (_, _) => callCount++;

			_blackboard.SetValueWithoutNotify(variable.Guid, 40);

			Assert.That(variable.Value, Is.EqualTo(40));
			Assert.That(callCount, Is.EqualTo(0));
		}
	}
}
