using System;
using VoodooTest.ApplicationManagement.States;

namespace VoodooTest.ApplicationManagement
{
    public static class ApplicationManager
    {
        // TODO: Make this list automated
        private static readonly ApplicationState[] applicationStates = {
            new MainMenuState(),
            new GameplayState(),
            new GameOverState(),
            new StoreMenuState()
        };

        private static ApplicationState currentState;

        public static ApplicationState CurrentState => currentState;

        public static event Action<ApplicationState> NewStateEnteredEvent;

        public static void GoToState<T>() where T : ApplicationState
        {
            ApplicationState newState = null;
            
            for (int i = 0; i < applicationStates.Length; i++)
            {
                if (applicationStates[i].GetType() == typeof(T))
                {
                    newState = applicationStates[i];
                    break;
                }
            }

            if (newState == null)
            {
                throw new Exception($"No ApplicationState found of type {typeof(T).Name}");
            }

            currentState?.Exit();
            newState.Enter();
            currentState = newState;
            
            NewStateEnteredEvent?.Invoke(currentState);
        }

        public static T GetState<T>() where T : ApplicationState
        {
            for (int i = 0; i < applicationStates.Length; i++)
            {
                if (applicationStates[i].GetType() == typeof(T))
                {
                    return applicationStates[i] as T;
                }
            }
            
            throw new Exception($"No ApplicationState found of type {typeof(T).Name}");
        }

        public static bool IsCurrentState<T>() where T : ApplicationState
        {
            if (currentState == null)
            {
                return false;
            }

            return currentState.GetType() == typeof(T);
        }
    }
}