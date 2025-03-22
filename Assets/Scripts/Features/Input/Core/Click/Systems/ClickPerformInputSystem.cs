using GameSdk.Core.Loggers;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Features.Input
{
    [UpdateInGroup(typeof(InputSimalationSystemGroup), OrderFirst = true)]
    public partial class ClickPerformInputSystem : SystemBase
    {
        public const string TAG = "ClickPerformInputSystem";

        private Camera _mainCamera;
        private InputSystemActions _actions;
        private bool _hasClickThisFrame;
        private Ray _currentRay;

        protected override void OnCreate()
        {
            RequireForUpdate<ClickInput>();
            RequireForUpdate<ClickPerformInput>();

            _actions = new InputSystemActions();
            _actions.Enable();

            _actions.Player.Attack.performed += OnClickPerformed;
        }

        protected override void OnDestroy()
        {
            if (_actions != null)
            {
                _actions.Player.Attack.performed -= OnClickPerformed;
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
            _hasClickThisFrame = true;
        }

        protected override void OnUpdate()
        {

            if (_hasClickThisFrame)
            {

                foreach (var (clickPerformInput, clickInput, entity) in SystemAPI.Query<RefRW<ClickPerformInput>, RefRW<ClickInput>>().WithPresent<ClickPerformInput, ClickInput>().WithEntityAccess())
                {
                    SystemLog.Log(TAG, $"Click Performed - {_currentRay.origin}");

                    clickPerformInput.ValueRW.ClickPosition = _currentRay.origin;
                    clickPerformInput.ValueRW.ClickRay = _currentRay;

                    clickInput.ValueRW.Position = _currentRay.origin;
                    clickInput.ValueRW.Ray = _currentRay;

                    SystemAPI.SetComponentEnabled<ClickPerformInput>(entity, true);
                    SystemAPI.SetComponentEnabled<ClickInput>(entity, true);
                }

                _hasClickThisFrame = false;

                return;
            }


            foreach (var (_, entity) in SystemAPI.Query<RefRW<ClickPerformInput>>().WithAll<ClickPerformInput>().WithEntityAccess())
            {
                SystemAPI.SetComponentEnabled<ClickPerformInput>(entity, false);
                SystemAPI.SetComponentEnabled<ClickInput>(entity, false);
            }
        }
    }
}
