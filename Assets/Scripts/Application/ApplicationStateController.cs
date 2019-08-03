using UnityEngine;
using VoodooTest.ApplicationManagement.States;

namespace VoodooTest.ApplicationManagement
{
    public class ApplicationStateController : MonoBehaviour
    {
        private void Start()
        {
            ApplicationManager.GoToState<MainMenuState>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && ApplicationManager.IsCurrentState<MainMenuState>())
            {
                ApplicationManager.GoToState<GameplayState>();
            }
        }
    }
}