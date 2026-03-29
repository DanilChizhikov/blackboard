using System;
using NUnit.Framework;
using Unity.PerformanceTesting;
using System.Collections.Generic;
using UnityEngine.TestTools;

namespace DTech.Blackboard.Tests.Performance
{
    [TestFixture]
    internal sealed class BlackboardMemoryAllocationTests
    {
        private Blackboard _blackboard;

        [SetUp]
        public void SetUp()
        {
            _blackboard = new Blackboard();
            LogAssert.ignoreFailingMessages = true;
        }

        [TearDown]
        public void TearDown()
        {
            LogAssert.ignoreFailingMessages = false;
            _blackboard?.Dispose();
        }

        [Test, Performance]
        public void AddVariable_Allocations()
        {
            Measure.Method(() =>
                {
                    var variable = new BlackboardVariable<int> { Name = Guid.NewGuid().ToString(), Value = 42 };
                    _blackboard.Add(variable);
                })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void TryGetVariable_ByName_NoAllocations()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            _blackboard.Add(variable);

            Measure.Method(() => { _ = _blackboard.TryGetVariable("TestVar", out _); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(10000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void TryGetVariable_ByGuid_NoAllocations()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            _blackboard.Add(variable);
            var guid = variable.Guid;

            Measure.Method(() => { _ = _blackboard.TryGetVariable(guid, out _); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(10000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void TryGetVariable_Generic_ByName_Allocations()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            _blackboard.Add(variable);

            Measure.Method(() => { _ = _blackboard.TryGetVariable<int>("TestVar", out _); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(10000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void SetValue_Allocations()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            _blackboard.Add(variable);

            Measure.Method(() => { _blackboard.SetValue("TestVar", 100); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void GetValue_NoAllocations()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            _blackboard.Add(variable);

            Measure.Method(() => { _ = _blackboard.GetValue<int>("TestVar"); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(10000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Remove_ByName_Allocations()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            _blackboard.Add(variable);

            Measure.Method(() =>
                {
                    _ = _blackboard.Remove("TestVar");
                    _blackboard.Add(variable);
                })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(100)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Remove_ByGuid_Allocations()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            _blackboard.Add(variable);
            var guid = variable.Guid;

            Measure.Method(() =>
                {
                    _ = _blackboard.Remove(guid);
                    _blackboard.Add(variable);
                })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(100)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Replace_ByName_Allocations()
        {
            var original = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            var replacement = new BlackboardVariable<int> { Name = "TestVar", Value = 100 };
            _blackboard.Add(original);

            Measure.Method(() =>
                {
                    _ = _blackboard.Replace("TestVar", replacement);
                })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(100)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void VariableCount_NoAllocations()
        {
            for (int i = 0; i < 100; i++)
            {
                _blackboard.Add(new BlackboardVariable<int> { Name = $"Var{i}", Value = i });
            }

            Measure.Method(() => { _ = _blackboard.VariableCount; })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(10000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void GetVariablesNonAlloc_Allocations()
        {
            for (int i = 0; i < 100; i++)
            {
                _blackboard.Add(new BlackboardVariable<int> { Name = $"Var{i}", Value = i });
            }

            var array = new BlackboardVariable[100];

            Measure.Method(() => { _ = _blackboard.GetVariablesNonAlloc(array); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Constructor_WithCapacity_Allocations()
        {
            Measure.Method(() =>
                {
                    var blackboard = new Blackboard(100);
                    blackboard.Dispose();
                })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(100)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Constructor_WithVariables_Allocations()
        {
            var variables = new List<BlackboardVariable>();
            for (int i = 0; i < 100; i++)
            {
                variables.Add(new BlackboardVariable<int> { Name = $"Var{i}", Value = i });
            }

            Measure.Method(() =>
                {
                    var blackboard = new Blackboard(variables);
                    blackboard.Dispose();
                })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Variable_ValueChange_WithNotification_Allocations()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            int receivedValue = 0;
            variable.OnValueChanged += (_, value) => receivedValue = value;

            Measure.Method(() => { variable.Value = 100; })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Variable_ValueChange_WithoutNotification_Allocations()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };

            Measure.Method(() => { variable.SetValueWithoutNotif(100); })
                .WarmupCount(10)
                .MeasurementCount(50)
                .IterationsPerMeasurement(1000)
                .GC()
                .Run();
        }
    }
}
