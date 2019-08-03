using System;

namespace VoodooTest.ApplicationManagement
{
    public abstract class ApplicationState
    {
        public event Action EnterStateEvent;
        public event Action ExitStateEvent;

        public void Enter()
        {
            EnterStateEvent?.Invoke();
        }

        public void Exit()
        {
            ExitStateEvent?.Invoke();
        }
    }
}