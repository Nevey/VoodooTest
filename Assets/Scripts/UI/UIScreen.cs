using System;
using UnityEngine;
using VoodooTest.ApplicationManagement;

namespace VoodooTest.UI
{
    public abstract class UIScreen : MonoBehaviour
    {
        public abstract Type ApplicationStateType { get; }

        public void Initialize()
        {
            // Make sure we're always hidden at the start
            // Don't animate anything when hiding here...
            Hide();
        }
        
        public void Show()
        {
            // TODO: Tweens
            gameObject.SetActive(true);
            OnShow();
        }

        public void Hide()
        {
            // TODO: Tweens
            gameObject.SetActive(false);
            OnHide();
        }

        private void Update()
        {
            OnUpdate();
        }

        protected abstract void OnShow();
        protected abstract void OnHide();

        protected virtual void OnUpdate()
        {
            
        }
    }
    
    public abstract class UIScreen<T> : UIScreen where T : ApplicationState
    {
        public override Type ApplicationStateType => typeof(T);
    }
}