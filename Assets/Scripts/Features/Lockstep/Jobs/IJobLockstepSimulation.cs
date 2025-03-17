using Unity.Jobs;
using Unity.Collections;
using System.Collections.Generic;
using System;

namespace Features.Lockstep
{
    // Extending interface to support Jobs
    public interface IJobLockstepSimulation : ILockstepSimulation
    {
        // Creates a job for execution in the Jobs System
        JobHandle CreateSimulationJob(uint step, JobHandle dependsOn = default);

        // Gets the list of dependencies - which simulations does this depend on
        IReadOnlyList<Type> JobDependencies { get; }


    }

    // Base implementation with support for Jobs
    public abstract class JobLockstepSimulation : IJobLockstepSimulation
    {
        public abstract int Order { get; }

        public virtual IReadOnlyList<Type> JobDependencies => Array.Empty<Type>();

        // Standard execution (not in job)
        public virtual void Simulate(uint step)
        {
            // Create a job
            var jobHandle = CreateSimulationJob(step);

            // Wait for completion and process results
            jobHandle.Complete();
        }

        // Method for creating a Job - must be implemented in descendants
        public abstract JobHandle CreateSimulationJob(uint step, JobHandle dependsOn = default);
    }
}