using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Input
{
    /// <summary>
    /// System that processes movement input and updates MovementInput components.
    /// Handles input from the new Unity Input System and can be extended
    /// to support different input sources.
    /// </summary>
    [UpdateInGroup(typeof(InputSimalationSystemGroup))]
    [BurstCompile]
    public partial class MovementInputSystem : SystemBase
    {
        public const string TAG = "MovementInputSystem";

        private InputSystemActions _actions;
        private float2 _direction;
        private bool _hasInput;

        protected override void OnCreate()
        {
            RequireForUpdate<MovementInput>();
            RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();

            _actions = new InputSystemActions();
            _actions.Enable();

            _actions.Player.Move.performed += OnMovePerformed;
            _actions.Player.Move.canceled += OnMoveCanceled;
        }

        protected override void OnDestroy()
        {
            _actions.Disable();

            _actions.Player.Move.performed -= OnMovePerformed;
            _actions.Player.Move.canceled -= OnMoveCanceled;
        }

        protected override void OnUpdate()
        {
            if (_hasInput)
            {
                foreach (var (input, entity) in SystemAPI.Query<RefRW<MovementInput>>().WithPresent<MovementInput>().WithEntityAccess())
                {
                    input.ValueRW.Direction = _direction;
                    input.ValueRW.IsActive = true;
                    SystemAPI.SetComponentEnabled<MovementInput>(entity, true);
                }

                return;
            }

            // Schedule component disabling at the end of simulation
            var ecbSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            var ecb = ecbSystem.CreateCommandBuffer(World.Unmanaged);

            foreach (var (input, entity) in SystemAPI.Query<RefRW<MovementInput>>().WithEntityAccess())
            {
                input.ValueRW.Direction = float2.zero;
                input.ValueRW.IsActive = false;
                ecb.SetComponentEnabled<MovementInput>(entity, false);
            }
        }

        private void OnMovePerformed(InputAction.CallbackContext context)
        {
            var value = context.ReadValue<Vector2>();
            _direction = new float2(value.x, value.y);
            _hasInput = true;
        }

        private void OnMoveCanceled(InputAction.CallbackContext context)
        {
            _direction = float2.zero;
            _hasInput = false;
        }
    }
}
