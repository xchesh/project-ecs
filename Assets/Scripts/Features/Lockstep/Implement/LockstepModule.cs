using System;
using System.Collections.Generic;
using System.Linq;
using Features.Lockstep.States;
using GameSdk.Core.Loggers;
using UnityEngine;

namespace Features.Lockstep
{
    public class LockstepModule : ILockstepModule, IDisposable
    {
        private const string TAG = "LockstepModule";

        private readonly IReadOnlyList<ILockstepSimulation> _simulations;
        private readonly IReadOnlyList<IStatesStore> _statesStores;
        private readonly ILockstepLifecycle _lockstepLifecycle;

        public LockstepModule(IEnumerable<ILockstepSimulation> simulations, IEnumerable<IStatesStore> statesStores, ILockstepLifecycle lockstepLifecycle)
        {
            _simulations = simulations.OrderBy(s => s.Order).ToList();
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
            foreach (var simulation in _simulations)
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
            foreach (var tickable in _statesStores)
            {
                tickable.Reset();
            }
        }

        public void Dispose()
        {
            _lockstepLifecycle.Step -= StepHandler;
            _lockstepLifecycle.Stop -= StopHandler;
        }
    }
}
