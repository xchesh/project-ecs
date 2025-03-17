using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Input
{
    /// <summary>
    /// System that processes mouse clicks and updates click state.
    /// Only responsible for handling input and creating click events.
    /// </summary>
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class ClickInputSystem : SystemBase
    {
        private Camera _mainCamera;
        private InputSystemActions _actions;
        private Entity _clickEntity;
        private bool _hasClickThisFrame;
        private Ray _currentRay;

        protected override void OnCreate()
        {
            RequireForUpdate<ClickInput>();

            _actions = new InputSystemActions();
            _actions.Enable();

            _actions.Player.Attack.performed += OnClickPerformed;

            // Create entity for click state
            _clickEntity = EntityManager.CreateEntity();
            EntityManager.AddComponent<ClickInput>(_clickEntity);
        }

        protected override void OnDestroy()
        {
            if (_actions != null)
            {
                _actions.Player.Attack.performed -= OnClickPerformed;
                _actions.Disable();
            }

            if (EntityManager.Exists(_clickEntity))
            {
                EntityManager.DestroyEntity(_clickEntity);
            }
        }

        protected override void OnStartRunning()
        {
            _mainCamera = Camera.main;
        }

        private void OnClickPerformed(InputAction.CallbackContext context)
        {
            if (_mainCamera == null) return;

            var mousePosition = Mouse.current.position.ReadValue();
            _currentRay = _mainCamera.ScreenPointToRay(mousePosition);
            _hasClickThisFrame = true;
        }

        protected override void OnUpdate()
        {
            foreach (var clickInput in SystemAPI.Query<RefRW<ClickInput>>())
            {
                clickInput.ValueRW.HasClick = false;

                if (_hasClickThisFrame)
                {
                    clickInput.ValueRW.ClickPosition = _currentRay.origin;
                    clickInput.ValueRW.ClickRay = _currentRay;
                    clickInput.ValueRW.HasClick = true;

                    _hasClickThisFrame = false;
                }
            }
        }
    }
}
