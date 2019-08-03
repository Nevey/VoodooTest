using UnityEngine;
using VoodooTest.Player;

namespace VoodooTest.Themes
{
    /// <summary>
    /// This component is attached to the player which is not ideal.
    /// Put this in a more centralized place.
    /// </summary>
    public class PlayerSkinController : MonoBehaviour
    {
        private const string SELECTED_SKIN_KEY = "VoodooTest.SelectedSkinKey";

        [SerializeField] private CoinsTracker coinsTracker;
        [SerializeField] private PlayerSkinConfig[] playerSkinConfigs;
        
        private int selectedSkinIndex;
        
        public PlayerSkinConfig[] PlayerSkinConfigs => playerSkinConfigs;
        public int SelectedSkinIndex => selectedSkinIndex;

        private void Awake()
        {
            for (int i = 0; i < playerSkinConfigs.Length; i++)
            {
                playerSkinConfigs[i].Load();
            }
            
            selectedSkinIndex = PlayerPrefs.GetInt(SELECTED_SKIN_KEY);
        }
        
        public PlayerSkinConfig GetSelectedSkin()
        {
            return playerSkinConfigs[selectedSkinIndex];
        }

        public void SetSelectedSkin(int index)
        {
            selectedSkinIndex = index;
            PlayerPrefs.SetInt(SELECTED_SKIN_KEY, selectedSkinIndex);
        }

        public PlayerSkinConfig GetNextUnlockable()
        {
            int nearestDifference = 100000;

            PlayerSkinConfig nextUnlockableSkin = null;

            for (int i = 0; i < playerSkinConfigs.Length; i++)
            {
                PlayerSkinConfig playerSkinConfig = playerSkinConfigs[i];

                if (playerSkinConfig.IsUnlocked)
                {
                    continue;
                }

                // If there's a locked skin which we can purchase, return this one
                if (playerSkinConfig.Price < coinsTracker.TotalCoinsCollected)
                {
                    return playerSkinConfig;
                }

                int difference = playerSkinConfig.Price - coinsTracker.TotalCoinsCollected;

                if (difference < nearestDifference)
                {
                    nextUnlockableSkin = playerSkinConfig;
                    nearestDifference = difference;
                }
            }

            return nextUnlockableSkin;
        }
    }
}