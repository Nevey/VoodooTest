using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;
using VoodooTest.Player;
using VoodooTest.Themes;
using VoodooTest.UI.Screens;

namespace VoodooTest.UI.Utils
{
    public class NextUnlockView : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private PlayerSkinController playerSkinController;
        [SerializeField] private CoinsTracker coinsTracker;
        [SerializeField] private TextMeshProUGUI nextSkinText;
        [SerializeField] private TextMeshProUGUI coinsLeftText;
        [SerializeField] private Image skinImage;
        [SerializeField] private Button skinButton;

        private string defaultCoinsLeftText;
        private PlayerSkinConfig playerSkinConfig;

        private void Awake()
        {
            defaultCoinsLeftText = coinsLeftText.text;
            
            skinButton.onClick.AddListener(OnSkinButtonPressed);
        }

        private void OnDestroy()
        {
            skinButton.onClick.RemoveListener(OnSkinButtonPressed);
        }

        private void OnEnable()
        {
            playerSkinConfig = playerSkinController.GetNextUnlockable();

            bool foundUnlockable = playerSkinConfig != null;
            
            nextSkinText.gameObject.SetActive(foundUnlockable);
            coinsLeftText.gameObject.SetActive(foundUnlockable);
            skinImage.gameObject.SetActive(foundUnlockable);

            if (!foundUnlockable)
            {
                return;
            }

            skinImage.sprite = playerSkinConfig.Sprite;

            int coinsNeeded = playerSkinConfig.Price - coinsTracker.TotalCoinsCollected;

            if (coinsNeeded < 0)
            {
                coinsLeftText.SetText("GET THIS LOOK NOW!");
            }
            else
            {
                coinsLeftText.SetText(string.Format(defaultCoinsLeftText, coinsNeeded));
            }
        }

        private void OnSkinButtonPressed()
        {
            ApplicationManager.GoToState<StoreMenuState>();
            uiManager.GetScreen<StoreMenuScreen>().ShowUnlockPopup(playerSkinConfig);
        }
    }
}