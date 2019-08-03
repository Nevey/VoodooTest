using UnityEngine;
using UnityEngine.UI;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;

namespace VoodooTest.UI.Screens
{
    public class MainMenuScreen : UIScreen<MainMenuState>
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button storeButton;
        
        protected override void OnShow()
        {
            playButton.onClick.AddListener(OnPlayButtonPressed);
            storeButton.onClick.AddListener(OnStoreButtonPressed);
        }

        protected override void OnHide()
        {
            playButton.onClick.RemoveListener(OnPlayButtonPressed);
            storeButton.onClick.RemoveListener(OnStoreButtonPressed);
        }
        
        private void OnPlayButtonPressed()
        {
            ApplicationManager.GoToState<GameplayState>();
        }
        
        private void OnStoreButtonPressed()
        {
            ApplicationManager.GoToState<StoreMenuState>();
        }
    }
}