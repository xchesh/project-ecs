using GameSdk.Core.Loggers;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Input
{
    [UpdateInGroup(typeof(InputSimalationSystemGroup))]
    [UpdateAfter(typeof(ClickHoldInputSystem))]
    public partial class ClickCancelInputSystem : SystemBase
    {
        public const string TAG = "ClickCancelInputSystem";

        private Camera _mainCamera;
        private InputSystemActions _actions;
        private bool _hasCancelThisFrame;
        private float _holdStartTime;
        private Ray _currentRay;

        protected override void OnCreate()
        {
            RequireForUpdate<ClickInput>();
            RequireForUpdate<ClickCancelInput>();

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
            _holdStartTime = (float)SystemAPI.Time.ElapsedTime;

            SystemLog.Log(TAG, $"ClickInput: Performed - {_currentRay.origin}");
        }

        private void OnClickCanceled(InputAction.CallbackContext context)
        {
            if (_mainCamera == null) return;

            _hasCancelThisFrame = true;
            var mousePosition = Mouse.current.position.ReadValue();
            _currentRay = _mainCamera.ScreenPointToRay(mousePosition);

            SystemLog.Log(TAG, $"ClickInput: Canceled - {_currentRay.origin}");
        }

        protected override void OnUpdate()
        {
            if (_hasCancelThisFrame)
            {
                foreach (var (clickCancelInput, clickInput, entity) in SystemAPI.Query<RefRW<ClickCancelInput>, RefRW<ClickInput>>().WithPresent<ClickCancelInput, ClickInput>().WithEntityAccess())
                {
                    clickCancelInput.ValueRW.CancelRay = _currentRay;
                    clickCancelInput.ValueRW.CancelPosition = _currentRay.origin;
                    clickCancelInput.ValueRW.HoldDuration = (float)SystemAPI.Time.ElapsedTime - _holdStartTime;

                    clickInput.ValueRW.Ray = _currentRay;
                    clickInput.ValueRW.Position = _currentRay.origin;

                    SystemAPI.SetComponentEnabled<ClickCancelInput>(entity, true);
                    SystemAPI.SetComponentEnabled<ClickInput>(entity, true);
                }

                _hasCancelThisFrame = false;

                return;
            }

            foreach (var (_, entity) in SystemAPI.Query<RefRW<ClickCancelInput>>().WithAll<ClickCancelInput>().WithEntityAccess())
            {
                SystemAPI.SetComponentEnabled<ClickCancelInput>(entity, false);
                SystemAPI.SetComponentEnabled<ClickInput>(entity, false);
            }
        }
    }
}
