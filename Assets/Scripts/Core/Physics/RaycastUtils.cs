using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Physics;
using Unity.Entities;

namespace Core.Physics
{
    /// <summary>
    /// Utility class for performing efficient raycasts in Unity Physics.
    /// Provides methods for both single and batch raycast operations using Unity's Job System.
    /// </summary>
    public static class RaycastUtils
    {
        /// <summary>
        /// Burst-compiled job for performing multiple raycasts in parallel.
        /// </summary>
        [BurstCompile]
        private struct RaycastJob : IJobParallelFor
        {
            [ReadOnly] public CollisionWorld World;
            [ReadOnly] public NativeArray<RaycastInput> Inputs;
            public NativeArray<RaycastHit> Results;

            public void Execute(int index)
            {
                var input = Inputs[index];
                World.CastRay(input, out var hit);
                Results[index] = hit;
            }
        }

        /// <summary>
        /// Schedules a batch raycast operation to be executed in parallel.
        /// </summary>
        /// <param name="world">The physics collision world to perform raycasts in.</param>
        /// <param name="inputs">Array of raycast inputs. Each input defines start point, end point, and filter.</param>
        /// <param name="results">Array to store raycast results. Must be the same length as inputs.</param>
        /// <param name="batchSize">Number of raycasts to process in each parallel batch. Default is 4.</param>
        /// <returns>JobHandle that can be used to chain operations or ensure completion.</returns>
        /// <remarks>
        /// The caller is responsible for:
        /// - Ensuring inputs and results arrays have the same length
        /// - Properly allocating and disposing of the native arrays
        /// - Completing the returned JobHandle before accessing results
        /// </remarks>
        public static JobHandle ScheduleBatchRayCast(CollisionWorld world, NativeArray<RaycastInput> inputs, NativeArray<RaycastHit> results, JobHandle dependency, int batchSize = 4)
        {
            var rcj = new RaycastJob
            {
                Inputs = inputs,
                Results = results,
                World = world
            }.Schedule(inputs.Length, batchSize, dependency);

            return rcj;
        }

        /// <summary>
        /// Schedules a batch raycast operation to be executed in parallel.
        /// </summary>
        /// <param name="physicsWorld">The physics world to perform raycasts in.</param>
        /// <param name="inputs">Array of raycast inputs. Each input defines start point, end point, and filter.</param>
        /// <param name="results">Array to store raycast results. Must be the same length as inputs.</param>
        /// <param name="dependency">JobHandle to depend on. Optional, can be null.</param>
        /// <param name="batchSize">Number of raycasts to process in each parallel batch. Default is 4.</param>
        /// <returns>JobHandle that can be used to chain operations or ensure completion.</returns>
        public static JobHandle ScheduleBatchRayCast(PhysicsWorldSingleton physicsWorld, NativeArray<RaycastInput> inputs, NativeArray<RaycastHit> results, JobHandle dependency, int batchSize = 4)
        {
            return ScheduleBatchRayCast(physicsWorld.CollisionWorld, inputs, results, dependency, batchSize);
        }

        /// <summary>
        /// Performs a single raycast operation immediately.
        /// This is a convenience method that internally uses the batch raycast system.
        /// </summary>
        /// <param name="world">The physics collision world to perform the raycast in.</param>
        /// <param name="input">The raycast input defining start point, end point, and filter.</param>
        /// <param name="result">Reference to store the raycast result.</param>
        /// <remarks>
        /// This method:
        /// - Creates temporary arrays for a single raycast
        /// - Schedules and immediately completes the raycast job
        /// - Handles all memory management internally
        /// - Is less efficient for multiple raycasts compared to ScheduleBatchRayCast
        /// </remarks>
        public static void SingleRayCast(CollisionWorld world, RaycastInput input, JobHandle dependency, ref RaycastHit result)
        {
            // Create arrays of size 1 to store the single raycast input and result
            var rayCommands = new NativeArray<RaycastInput>(1, Allocator.TempJob);
            var rayResults = new NativeArray<RaycastHit>(1, Allocator.TempJob);
            // Store the input
            rayCommands[0] = input;
            // Schedule the raycast job
            var handle = ScheduleBatchRayCast(world, rayCommands, rayResults, dependency);
            // Wait for the job to complete
            handle.Complete();
            // Get the result
            result = rayResults[0];
            // Dispose of the arrays
            rayCommands.Dispose();
            rayResults.Dispose();
        }
    }
}
