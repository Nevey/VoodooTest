using System;
using UnityEngine;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;

namespace VoodooTest.Player
{
    // TODO: Create more centralized place for input when needed
    public class PlayerInput : MonoBehaviour
    {
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
            if (!isActive)
            {
                return;
            }
            
            
            float mouseX = Input.GetAxis("Mouse X");
            mouseX /= Screen.dpi;

            if (!hadInputDownAfterGameStart)
            {
                mouseX = 0f;
            }

            if (Input.GetMouseButtonDown(0))
            {
                hadInputDownAfterGameStart = true;
                PointerDownEvent?.Invoke();
            }

            if (Input.GetMouseButtonUp(0) && hadInputDownAfterGameStart)
            {
                PointerUpEvent?.Invoke();
            }

            if (!Input.GetMouseButton(0))
            {
                return;
            }

            HorizontalDragEvent?.Invoke(mouseX);
        }
    }
}