using System;
using UnityEngine;
using VoodooTest.ApplicationManagement;

namespace VoodooTest.UI
{
    public class UIManager : MonoBehaviour
    {
        private UIScreen[] uiScreens;

        private UIScreen currentScreen;

        private void Awake()
        {
            ApplicationManager.NewStateEnteredEvent += OnNewStateEnteredEvent;
            
            // Automatically find all ui screens attached to children 
            uiScreens = GetComponentsInChildren<UIScreen>(true);
            
            for (int i = 0; i < uiScreens.Length; i++)
            {
                uiScreens[i].Initialize();
            }
        }

        private void OnDestroy()
        {
            ApplicationManager.NewStateEnteredEvent -= OnNewStateEnteredEvent;
        }

        private void OnNewStateEnteredEvent(ApplicationState applicationState)
        {
            for (int i = 0; i < uiScreens.Length; i++)
            {
                UIScreen uiScreen = uiScreens[i];

                if (uiScreen.ApplicationStateType != applicationState.GetType())
                {
                    continue;
                }

                // Don't do anything if current ui screen is the same as new ui screen 
                if (currentScreen == uiScreen)
                {
                    return;
                }
                
                // Hide the current screen, if any, and show the new screen
                currentScreen?.Hide();
                uiScreen.Show();
                currentScreen = uiScreen;

                return;
            }
            
            // It's ok if there's no ui screen for a specific state, but at least warn us about this
            Debug.LogWarning($"No UIScreen found for ApplicationState {applicationState.GetType().Name}");
        }

        public T GetScreen<T>() where T : UIScreen
        {
            for (int i = 0; i < uiScreens.Length; i++)
            {
                UIScreen uiScreen = uiScreens[i];

                if (uiScreen.GetType() == typeof(T))
                {
                    return uiScreen as T;
                }
            }

            throw new Exception($"Trying to get screen {typeof(T).Name}, but it's not added to UIManager's list of screens!");
        }
    }
}