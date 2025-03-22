using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Input
{
    [UpdateInGroup(typeof(InputSimalationSystemGroup))]
    [UpdateAfter(typeof(ClickPerformInputSystem))]
    public partial class ClickHoldInputSystem : SystemBase
    {
        private Camera _mainCamera;
        private InputSystemActions _actions;
        private bool _isHoldingClick;
        private float _holdStartTime;
        private Ray _currentRay;

        protected override void OnCreate()
        {
            RequireForUpdate<ClickInput>();
            RequireForUpdate<ClickHoldInput>();

            _actions = new InputSystemActions();
            _actions.Enable();

            _actions.Player.Attack.performed += OnClickPerformed;
            _actions.Player.Attack.canceled += OnClickCanceled;
        }

        protected override void OnDestroy()
        {
            if (_actions != null)
            {
                _actions.Player.Attack.performed -= OnClickPerformed;
                _actions.Player.Attack.canceled -= OnClickCanceled;
                _actions.Disable();
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
            _isHoldingClick = true;
            _holdStartTime = (float)SystemAPI.Time.ElapsedTime;
        }

        private void OnClickCanceled(InputAction.CallbackContext context)
        {
            _isHoldingClick = false;
        }

        protected override void OnUpdate()
        {
            if (_isHoldingClick)
            {
                var mousePosition = Mouse.current.position.ReadValue();
                _currentRay = _mainCamera.ScreenPointToRay(mousePosition);

                foreach (var (clickHoldInput, clickInput, entity) in SystemAPI.Query<RefRW<ClickHoldInput>, RefRW<ClickInput>>().WithPresent<ClickHoldInput, ClickInput>().WithEntityAccess())
                {
                    clickHoldInput.ValueRW.CurrentRay = _currentRay;
                    clickHoldInput.ValueRW.CurrentPosition = _currentRay.origin;
                    clickHoldInput.ValueRW.HoldDuration = (float)SystemAPI.Time.ElapsedTime - _holdStartTime;

                    clickInput.ValueRW.Ray = _currentRay;
                    clickInput.ValueRW.Position = _currentRay.origin;

                    SystemAPI.SetComponentEnabled<ClickHoldInput>(entity, true);
                    SystemAPI.SetComponentEnabled<ClickInput>(entity, true);
                }

                return;
            }

            foreach (var (_, entity) in SystemAPI.Query<RefRW<ClickHoldInput>>().WithAll<ClickHoldInput>().WithEntityAccess())
            {
                SystemAPI.SetComponentEnabled<ClickHoldInput>(entity, false);
                SystemAPI.SetComponentEnabled<ClickInput>(entity, false);
            }
        }
    }
}
