using System;
using NUnit.Framework;
using Unity.PerformanceTesting;
using System.Collections.Generic;
using UnityEngine.TestTools;
using UnityEngine;

namespace DTech.Blackboard.Tests.Performance
{
    [TestFixture]
    internal sealed class BlackboardStressTests
    {
        private RuntimeBlackboard _blackboard;

        [SetUp]
        public void SetUp()
        {
            _blackboard = new RuntimeBlackboard();
            LogAssert.ignoreFailingMessages = true;
        }

        [TearDown]
        public void TearDown()
        {
            LogAssert.ignoreFailingMessages = false;
            _blackboard?.Dispose();
        }

        [Test, Performance]
        public void StressTest_RapidVariableAddition_Performance()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        _blackboard.Add(new BlackboardVariable<int> { Name = Guid.NewGuid().ToString(), Value = i });
                    }
                })
                .WarmupCount(3)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void StressTest_RapidVariableAccess_ByName_Performance()
        {
            for (int i = 0; i < 1000; i++)
            {
                _blackboard.Add(new BlackboardVariable<int> { Name = $"Var{i}", Value = i });
            }

            Measure.Method(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        _ = _blackboard.TryGetVariable($"Var{i}", out _);
                    }
                })
                .WarmupCount(3)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .Run();
        }

        [Test, Performance]
        public void StressTest_RapidVariableAccess_ByGuid_Performance()
        {
            var guids = new List<SerializableGuid>(1000);
            for (int i = 0; i < 1000; i++)
            {
                var variable = new BlackboardVariable<int> { Name = $"Var{i}", Value = i };
                _blackboard.Add(variable);
                guids.Add(variable.Guid);
            }

            Measure.Method(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        _ = _blackboard.TryGetVariable(guids[i], out _);
                    }
                })
                .WarmupCount(3)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .Run();
        }

        [Test, Performance]
        public void StressTest_RapidValueSetting_Performance()
        {
            for (int i = 0; i < 1000; i++)
            {
                _blackboard.Add(new BlackboardVariable<int> { Name = $"Var{i}", Value = i });
            }

            Measure.Method(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        _blackboard.SetValue($"Var{i}", i + 1);
                    }
                })
                .WarmupCount(3)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .Run();
        }

        [Test, Performance]
        public void StressTest_MultipleVariableTypes_Performance()
        {
            Measure.Method(() =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        _blackboard.Add(new BlackboardVariable<int> { Name = Guid.NewGuid().ToString(), Value = i });
                        _blackboard.Add(new BlackboardVariable<float> { Name = Guid.NewGuid().ToString(), Value = i * 1.5f });
                        _blackboard.Add(new BlackboardVariable<string> { Name = Guid.NewGuid().ToString(), Value = $"test{i}" });
                        _blackboard.Add(new BlackboardVariable<bool> { Name = Guid.NewGuid().ToString(), Value = i % 2 == 0 });
                        _blackboard.Add(new BlackboardVariable<Vector3> { Name = Guid.NewGuid().ToString(), Value = new Vector3(i, i, i) });
                    }
                })
                .WarmupCount(3)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void StressTest_BlackboardCreation_Performance()
        {
            var blackboards = new List<RuntimeBlackboard>(100);

            Measure.Method(() =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        blackboards.Add(new RuntimeBlackboard(50));
                    }

                    foreach (var blackboard in blackboards)
                    {
                        blackboard.Dispose();
                    }

                    blackboards.Clear();
                })
                .WarmupCount(5)
                .MeasurementCount(20)
                .IterationsPerMeasurement(1)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void StressTest_ComplexScenario_Performance()
        {
            Measure.Method(() =>
                {
                    using (var blackboard = new RuntimeBlackboard())
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            blackboard.Add(new BlackboardVariable<int> { Name = $"Counter{i}", Value = i });
                            blackboard.Add(new BlackboardVariable<string> { Name = $"Name{i}", Value = $"Entity{i}" });

                            for (int j = 0; j < 10; j++)
                            {
                                blackboard.SetValue($"Counter{i}", i + j);
                                _ = blackboard.GetValue<int>($"Counter{i}");
                                _ = blackboard.TryGetVariable<string>($"Name{i}", out _);
                            }

                            if (i % 10 == 0)
                            {
                                _ = blackboard.Remove($"Counter{i}");
                            }
                        }
                    }
                })
                .WarmupCount(3)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .GC()
                .Run();
        }

        [Test, Performance]
        public void StressTest_ConcurrentAccessSimulation_Performance()
        {
            for (int i = 0; i < 500; i++)
            {
                _blackboard.Add(new BlackboardVariable<int> { Name = $"SharedVar{i}", Value = i });
            }

            Measure.Method(() =>
                {
                    for (int i = 0; i < 500; i++)
                    {
                        _ = _blackboard.TryGetVariable<int>($"SharedVar{i}", out var var);
                        if (var != null)
                        {
                            var.Value = i * 2;
                        }
                    }
                })
                .WarmupCount(3)
                .MeasurementCount(10)
                .IterationsPerMeasurement(1)
                .Run();
        }
    }
}
