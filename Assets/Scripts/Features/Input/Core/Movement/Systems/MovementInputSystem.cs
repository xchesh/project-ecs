using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Features.Movement;
using GameSdk.Core.Loggers;

namespace Features.Input
{
    /// <summary>
    /// System that processes movement input and updates MovementInput components.
    /// Handles input from the new Unity Input System and can be extended
    /// to support different input sources.
    /// </summary>
    [BurstCompile]
    public partial class MovementInputSystem : SystemBase
    {
        public const string TAG = "MovementInputSystem";

        private InputSystemActions _actions;
        private float2 _direction;

        protected override void OnCreate()
        {
            RequireForUpdate<MovementInput>();

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
            foreach (var input in SystemAPI.Query<RefRW<MovementInput>>())
            {
                input.ValueRW.Direction = _direction;
            }
        }

        private void OnMovePerformed(InputAction.CallbackContext ctx)
        {
            _direction = ctx.ReadValue<Vector2>();
        }

        private void OnMoveCanceled(InputAction.CallbackContext _)
        {
            _direction = float2.zero;
        }
    }
}
