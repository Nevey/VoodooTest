using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VoodooTest.Themes
{
    public class PlayerSkinStoreView : MonoBehaviour
    {
        [SerializeField] private Image skinImage;
        [SerializeField] private Image lockImage;
        [SerializeField] private Image selectedImage;
        [FormerlySerializedAs("button")] [SerializeField] private Button selectButton;
        [SerializeField] private Button unlockButton;

        private PlayerSkinConfig playerSkinConfig;

        public PlayerSkinConfig PlayerSkinConfig => playerSkinConfig;

        public event Action<PlayerSkinStoreView> TryUnlockEvent;
        public event Action<PlayerSkinStoreView> SelectEvent;

        private void OnSelectButtonPressed()
        {
            if (!playerSkinConfig.IsUnlocked)
            {
                return;
            }
            
            SelectEvent?.Invoke(this);
            selectedImage.gameObject.SetActive(true);
        }
        
        private void OnUnlockButtonPressed()
        {
            TryUnlockEvent?.Invoke(this);
        }

        public void Show(PlayerSkinConfig playerSkinConfig, bool isSelected, bool disableButtons = false)
        {
            this.playerSkinConfig = playerSkinConfig;
            
            skinImage.sprite = playerSkinConfig.Sprite;
            lockImage.gameObject.SetActive(!playerSkinConfig.IsUnlocked && !disableButtons);
            selectedImage.gameObject.SetActive(isSelected);

            if (disableButtons)
            {
                selectButton.enabled = false;
                unlockButton.enabled = false;
                return;
            }
            
            selectButton.enabled = true;
            unlockButton.enabled = true;
            
            selectButton.onClick.AddListener(OnSelectButtonPressed);
            unlockButton.onClick.AddListener(OnUnlockButtonPressed);
        }

        public void Hide()
        {
            selectButton.onClick.RemoveAllListeners();
            unlockButton.onClick.RemoveAllListeners();
        }

        public void Unselect()
        {
            selectedImage.gameObject.SetActive(false);
        }
    }
}