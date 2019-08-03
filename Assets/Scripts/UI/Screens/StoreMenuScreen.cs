using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;
using VoodooTest.Player;
using VoodooTest.Themes;

namespace VoodooTest.UI.Screens
{
    public class StoreMenuScreen : UIScreen<StoreMenuState>
    {
        [Header("World References")]
        [SerializeField] private CoinsTracker coinsTracker;
        
        [Header("Skin Store References")]
        [SerializeField] private Button backButton;
        [SerializeField] private PlayerSkinController playerSkinController;
        [SerializeField] private PlayerSkinStoreView playerSkinStoreViewPrefab;
        [SerializeField] private Transform skinRectTransform;
        
        // TODO: Add popup support to ui system
        [Header("Unlock Popup References")]
        [SerializeField] private GameObject unlockPopup;
        [SerializeField] private TextMeshProUGUI skinNameText;
        [SerializeField] private Button buyButton;
        [SerializeField] private Button cancelBuyButton;
        [SerializeField] private PlayerSkinStoreView unlockSkinStoreView;
        [SerializeField] private TextMeshProUGUI unlockText;
        [SerializeField] private TextMeshProUGUI notEnoughCurrencyText;
        
        private readonly List<PlayerSkinStoreView> playerSkinStoreViews = new List<PlayerSkinStoreView>();

        // Unlock popup fields
        private string defaultUnlockText;
        private string defaultNotEnoughCurrencyText;
        private PlayerSkinConfig playerSkinConfigToUnlock;

        private void Awake()
        {
            defaultUnlockText = unlockText.text;
            defaultNotEnoughCurrencyText = notEnoughCurrencyText.text;
        }

        protected override void OnShow()
        {
            // Make sure unlock popup isn't showing...
            HideUnlockPopup();
            
            backButton.onClick.AddListener(OnBackButtonPressed);
            
            for (int i = 0; i < playerSkinController.PlayerSkinConfigs.Length; i++)
            {
                PlayerSkinStoreView playerSkinStoreView = Instantiate(playerSkinStoreViewPrefab, skinRectTransform);
                playerSkinStoreView.Show(playerSkinController.PlayerSkinConfigs[i], playerSkinController.SelectedSkinIndex == i);
                playerSkinStoreView.SelectEvent += OnPlayerSkinSelect;
                playerSkinStoreView.TryUnlockEvent += OnTryUnlockSkin;

                playerSkinStoreViews.Add(playerSkinStoreView);
            }
        }

        protected override void OnHide()
        {
            backButton.onClick.RemoveListener(OnBackButtonPressed);
            
            for (int i = 0; i < playerSkinStoreViews.Count; i++)
            {
                PlayerSkinStoreView playerSkinStoreView = playerSkinStoreViews[i];
                playerSkinStoreView.Hide();
                playerSkinStoreView.SelectEvent -= OnPlayerSkinSelect;
                playerSkinStoreView.TryUnlockEvent -= OnTryUnlockSkin;
                
                Destroy(playerSkinStoreView.gameObject);
            }
            
            playerSkinStoreViews.Clear();
        }
        
        private void OnBackButtonPressed()
        {
            ApplicationManager.GoToState<MainMenuState>();
        }
        
        private void OnPlayerSkinSelect(PlayerSkinStoreView obj)
        {
            playerSkinStoreViews[playerSkinController.SelectedSkinIndex].Unselect();
            
            // Not the best way, accessing a controller and telling it what to do from a "View"...
            int index = playerSkinStoreViews.IndexOf(obj);
            playerSkinController.SetSelectedSkin(index);
        }
        
        private void OnTryUnlockSkin(PlayerSkinStoreView obj)
        {
            playerSkinConfigToUnlock = obj.PlayerSkinConfig;
            ShowUnlockPopup();
        }

        private void ShowUnlockPopup()
        {
            unlockPopup.SetActive(true);
            
            buyButton.onClick.AddListener(OnBuyButtonPressed);
            cancelBuyButton.onClick.AddListener(OnCancelBuyButtonPressed);
            
            unlockSkinStoreView.Show(playerSkinConfigToUnlock, false, true);
            
            skinNameText.SetText(playerSkinConfigToUnlock.Name);

            if (playerSkinConfigToUnlock.Price <= coinsTracker.TotalCoinsCollected)
            {
                buyButton.gameObject.SetActive(true);
                unlockText.gameObject.SetActive(true);
                notEnoughCurrencyText.gameObject.SetActive(false);
                
                unlockText.SetText(string.Format(defaultUnlockText, playerSkinConfigToUnlock.Price));
            }
            else
            {
                buyButton.gameObject.SetActive(false);
                unlockText.gameObject.SetActive(false);
                notEnoughCurrencyText.gameObject.SetActive(true);

                int currencyAmountNeeded = playerSkinConfigToUnlock.Price - coinsTracker.TotalCoinsCollected;
                notEnoughCurrencyText.SetText(string.Format(defaultNotEnoughCurrencyText, currencyAmountNeeded));
            }
        }

        private void HideUnlockPopup()
        {
            unlockPopup.SetActive(false);
            
            buyButton.onClick.RemoveListener(OnBuyButtonPressed);
            cancelBuyButton.onClick.RemoveListener(OnCancelBuyButtonPressed);
            
            unlockSkinStoreView.Hide();
        }

        private void OnBuyButtonPressed()
        {
            HideUnlockPopup();
            coinsTracker.SpendCoins(playerSkinConfigToUnlock.Price);
            playerSkinConfigToUnlock.Unlock();
            
            for (int i = 0; i < playerSkinStoreViews.Count; i++)
            {
                playerSkinStoreViews[i].Show(playerSkinController.PlayerSkinConfigs[i],
                    playerSkinController.SelectedSkinIndex == i);
            }
        }
        
        private void OnCancelBuyButtonPressed()
        {
            HideUnlockPopup();
        }

        /// <summary>
        /// Use this to have control over when the unlock popup is shown externally
        /// </summary>
        /// <param name="playerSkinConfig"></param>
        public void ShowUnlockPopup(PlayerSkinConfig playerSkinConfig)
        {
            playerSkinConfigToUnlock = playerSkinConfig;
            ShowUnlockPopup();
        }
    }
}