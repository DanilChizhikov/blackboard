using System;
using NUnit.Framework;
using Unity.PerformanceTesting;
using System.Collections.Generic;
using UnityEngine;

namespace DTech.Blackboard.Tests.Performance
{
    [TestFixture]
    internal sealed class BlackboardPerformanceTests
    {
        private Blackboard _blackboard;

        [SetUp]
        public void SetUp()
        {
            _blackboard = new Blackboard();
        }

        [TearDown]
        public void TearDown()
        {
            _blackboard?.Dispose();
        }

        [Test, Performance]
        public void AddVariable_Performance()
        {
            Measure.Method(() =>
                {
                    var variable = new BlackboardVariable<int> { Name = Guid.NewGuid().ToString(), Value = 42 };
                    _blackboard.Add(variable);
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void TryGetVariable_ByName_Performance()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            _blackboard.Add(variable);

            Measure.Method(() => { _ = _blackboard.TryGetVariable("TestVar", out _); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(1000)
                .Run();
        }

        [Test, Performance]
        public void TryGetVariable_ByGuid_Performance()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            _blackboard.Add(variable);
            var guid = variable.Guid;

            Measure.Method(() => { _ = _blackboard.TryGetVariable(guid, out _); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(1000)
                .Run();
        }

        [Test, Performance]
        public void TryGetVariable_Generic_ByName_Performance()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            _blackboard.Add(variable);

            Measure.Method(() => { _ = _blackboard.TryGetVariable<int>("TestVar", out _); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(1000)
                .Run();
        }

        [Test, Performance]
        public void SetValue_Performance()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            _blackboard.Add(variable);

            Measure.Method(() => { _blackboard.SetValue("TestVar", 100); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void SetValue_ByGuid_Performance()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            _blackboard.Add(variable);
            var guid = variable.Guid;

            Measure.Method(() => { _blackboard.SetValue(guid, 100); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void GetValue_Performance()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            _blackboard.Add(variable);

            Measure.Method(() => { _ = _blackboard.GetValue<int>("TestVar"); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(1000)
                .Run();
        }

        [Test, Performance]
        public void Remove_ByName_Performance()
        {
            var variable = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            _blackboard.Add(variable);

            Measure.Method(() =>
                {
                    _ = _blackboard.Remove("TestVar");
                    _blackboard.Add(variable);
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(10)
                .Run();
        }

        [Test, Performance]
        public void Remove_ByGuid_Performance()
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
                .MeasurementCount(100)
                .IterationsPerMeasurement(10)
                .Run();
        }

        [Test, Performance]
        public void Replace_ByName_Performance()
        {
            var original = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            var replacement = new BlackboardVariable<int> { Name = "TestVar", Value = 100 };
            _blackboard.Add(original);

            Measure.Method(() =>
                {
                    _ = _blackboard.Replace("TestVar", replacement);
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(10)
                .Run();
        }

        [Test, Performance]
        public void Replace_ByGuid_Performance()
        {
            var original = new BlackboardVariable<int> { Name = "TestVar", Value = 42 };
            var replacement = new BlackboardVariable<int> { Name = "TestVar", Value = 100 };
            _blackboard.Add(original);
            var guid = original.Guid;

            Measure.Method(() =>
                {
                    _ = _blackboard.Replace(guid, replacement);
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(10)
                .Run();
        }

        [Test, Performance]
        public void VariableCount_Performance()
        {
            for (int i = 0; i < 100; i++)
            {
                _blackboard.Add(new BlackboardVariable<int> { Name = $"Var{i}", Value = i });
            }

            Measure.Method(() => { _ = _blackboard.VariableCount; })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(1000)
                .Run();
        }

        [Test, Performance]
        public void GetVariablesNonAlloc_Performance()
        {
            for (int i = 0; i < 100; i++)
            {
                _blackboard.Add(new BlackboardVariable<int> { Name = $"Var{i}", Value = i });
            }

            var array = new BlackboardVariable[100];

            Measure.Method(() => { _ = _blackboard.GetVariablesNonAlloc(array); })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .Run();
        }

        [Test, Performance]
        public void Constructor_WithCapacity_Performance()
        {
            Measure.Method(() =>
                {
                    var blackboard = new Blackboard(100);
                    blackboard.Dispose();
                })
                .WarmupCount(10)
                .MeasurementCount(100)
                .IterationsPerMeasurement(100)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void Constructor_WithVariables_Performance()
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
                .MeasurementCount(100)
                .IterationsPerMeasurement(10)
                .GC()
                .Run();
        }
    }
}
