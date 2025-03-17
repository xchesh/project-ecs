using Unity.Jobs;
using Unity.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using GameSdk.Core.Loggers;
using Features.Lockstep.States;

namespace Features.Lockstep
{
    // Оптимизированный LockstepModule с использованием Unity Jobs
    public class JobsLockstepModule : ILockstepModule, IDisposable
    {
        private const string TAG = "JobsLockstepModule";

        private readonly Dictionary<Type, ILockstepSimulation> _simulationsMap;
        private readonly List<ILockstepSimulation> _simulations;
        private readonly List<IJobLockstepSimulation> _jobSimulations;
        private readonly IReadOnlyList<IStatesStore> _statesStores;
        private readonly ILockstepLifecycle _lockstepLifecycle;

        public JobsLockstepModule(
            IEnumerable<ILockstepSimulation> simulations,
            IEnumerable<IStatesStore> statesStores,
            ILockstepLifecycle lockstepLifecycle)
        {
            _simulations = simulations.OrderBy(s => s.Order).ToList();
            _simulationsMap = _simulations.ToDictionary(s => s.GetType());
            _jobSimulations = _simulations.OfType<IJobLockstepSimulation>().ToList();
            _statesStores = statesStores.ToList();
            _lockstepLifecycle = lockstepLifecycle;
        }

        public void Initialize()
        {
            _lockstepLifecycle.Step += StepHandler;
            _lockstepLifecycle.Stop += StopHandler;
        }

        private void StepHandler(uint step)
        {
            // Выполняем обычные симуляции (не-job) в обычном порядке
            var regularSimulations = _simulations.Except(_jobSimulations.Cast<ILockstepSimulation>());
            foreach (var simulation in regularSimulations)
            {
                try
                {
                    simulation.Simulate(step);
                }
                catch (Exception ex)
                {
                    SystemLog.LogError(TAG, $"Error during simulation step {step}: {ex}");
                }
            }

            // Словарь для отслеживания JobHandles для каждой симуляции
            var simulationJobs = new Dictionary<Type, JobHandle>();

            // Для каждой job-симуляции
            foreach (var jobSimulation in _jobSimulations.OrderBy(js => js.Order))
            {
                try
                {
                    // Находим зависимости
                    JobHandle dependsOn = default;

                    foreach (var dependencyType in jobSimulation.JobDependencies)
                    {
                        if (simulationJobs.TryGetValue(dependencyType, out var depHandle))
                        {
                            // Объединяем зависимости
                            if (dependsOn == default)
                                dependsOn = depHandle;
                            else
                                dependsOn = JobHandle.CombineDependencies(dependsOn, depHandle);
                        }
                    }

                    // Создаем и запускаем job
                    var jobHandle = jobSimulation.CreateSimulationJob(step, dependsOn);

                    // Сохраняем JobHandle
                    simulationJobs[jobSimulation.GetType()] = jobHandle;
                }
                catch (Exception ex)
                {
                    SystemLog.LogError(TAG, $"Error creating job for simulation {jobSimulation.GetType().Name}: {ex}");
                }
            }

            // Собираем все JobHandles для ожидания их завершения
            foreach (var jobHandle in simulationJobs.Values)
            {
                jobHandle.Complete();
            }

            // Обновляем хранилища состояний
            foreach (var statesStore in _statesStores)
            {
                try
                {
                    statesStore.Update(step);
                }
                catch (Exception ex)
                {
                    SystemLog.LogError(TAG, $"Error during state store update step {step}: {ex}");
                }
            }
        }

        private void StopHandler()
        {
            foreach (var statesStore in _statesStores)
            {
                statesStore.Reset();
            }
        }

        public void Dispose()
        {
            _lockstepLifecycle.Step -= StepHandler;
            _lockstepLifecycle.Stop -= StopHandler;

            // Очищаем ресурсы Job-симуляций
            foreach (var jobSim in _jobSimulations)
            {
                if (jobSim is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}