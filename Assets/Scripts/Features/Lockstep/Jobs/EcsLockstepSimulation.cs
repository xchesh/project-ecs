using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Features.Lockstep
{
    // Пример симуляции с использованием ECS
    public partial class EcsLockstepSimulation : JobLockstepSimulation
    {
        private EntityManager _entityManager;
        private EntityQuery _movableEntitiesQuery;

        public override int Order => 3;

        public EcsLockstepSimulation(World world)
        {
            _entityManager = world.EntityManager;

            // Создаем запрос для выборки сущностей с определенными компонентами
            _movableEntitiesQuery = _entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Position>(),
                ComponentType.ReadWrite<Velocity>()
            );
        }

        public override JobHandle CreateSimulationJob(uint step, JobHandle dependsOn = default)
        {
            var world = World.DefaultGameObjectInjectionWorld;
            // Создаем или получаем синглтон для передачи данных
            var stepDataQuery = world.EntityManager.CreateEntityQuery(typeof(StepData));
            if (!stepDataQuery.HasSingleton<StepData>())
            {
                var entity = world.EntityManager.CreateEntity();
                world.EntityManager.AddComponentData(entity, new StepData { Value = step });
            }
            else
            {
                stepDataQuery.SetSingleton(new StepData { Value = step });
            }

            var job = new MoveSystem { Step = step, DeltaTime = ILockstepLifecycle.STEP_DURATION };
            return job.Schedule(dependsOn);
        }

        // Определение системы перемещения
        [UpdateInGroup(typeof(SimulationSystemGroup))]
        public partial struct MoveSystem : IJobEntity
        {
            public uint Step;
            public float DeltaTime;

            public void Execute(ref Position position, in Velocity velocity)
            {
                position.Value += velocity.Value * DeltaTime;
            }
        }

        // Компоненты ECS
        public struct Position : IComponentData
        {
            public float3 Value;
        }

        public struct Velocity : IComponentData
        {
            public float3 Value;
        }

        // Добавьте синглтон для передачи данных
        public struct StepData : IComponentData
        {
            public uint Value;
        }
    }
}
