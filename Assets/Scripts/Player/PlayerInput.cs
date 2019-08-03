using System;
using UnityEngine;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;

namespace VoodooTest.Player
{
    // TODO: Create more centralized place for input when needed
    public class PlayerInput : MonoBehaviour
    {
        private float previousMouseInputX;
        private float delta;
        private float rawDelta;
        
        private GameplayState gameplayState;
        private bool isActive;
        private bool hadInputDownAfterGameStart;

        public event Action<float> HorizontalDragEvent;
        public event Action PointerDownEvent;
        public event Action PointerUpEvent;

        private void Awake()
        {
            gameplayState = ApplicationManager.GetState<GameplayState>();
            gameplayState.EnterStateEvent += OnEnterGameplayState;
            gameplayState.ExitStateEvent += OnExitGameplayState;
        }

        private void OnDestroy()
        {
            gameplayState.EnterStateEvent -= OnEnterGameplayState;
            gameplayState.ExitStateEvent -= OnExitGameplayState;
        }
        
        private void OnEnterGameplayState()
        {
            isActive = true;
            hadInputDownAfterGameStart = false;
        }
        
        private void OnExitGameplayState()
        {
            isActive = false;
        }

        private void Update()
        {
            // Update the delta always, this helps working around initialization issues :D
            UpdateDelta();

            if (!isActive)
            {
                return;
            }

            if (!hadInputDownAfterGameStart)
            {
                delta = 0f;
            }

            // This also works on touch devices
            if (Input.GetMouseButtonDown(0))
            {
                hadInputDownAfterGameStart = true;
                PointerDownEvent?.Invoke();
            }

            // This also works on touch devices
            if (Input.GetMouseButtonUp(0) && hadInputDownAfterGameStart)
            {
                PointerUpEvent?.Invoke();
            }

            // This also works on touch devices
            if (!Input.GetMouseButton(0))
            {
                return;
            }

            HorizontalDragEvent?.Invoke(delta);
        }

        private void UpdateDelta()
        {
            if (Input.touchSupported)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.touches[0];
                    rawDelta = touch.deltaPosition.x;
                }
            }
            else
            {
                rawDelta = Input.mousePosition.x - previousMouseInputX;
                previousMouseInputX = Input.mousePosition.x;
            }

            // Make sure the delta input is based on screen width, as the controls require the user to
            // swipe horizontally
            delta = rawDelta / Screen.width;
        }
    }
}