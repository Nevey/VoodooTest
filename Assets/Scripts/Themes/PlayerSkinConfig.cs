using System;
using UnityEngine;

namespace VoodooTest.Themes
{
    [CreateAssetMenu(fileName = "PlayerSkinConfig", menuName = "Themes/PlayerSkinConfig")]
    public class PlayerSkinConfig : ScriptableObject
    {
        private const string IS_LOCKED_KEY = "VoodooTest.SkinIsLocked";

        [SerializeField] private new string name;
        [SerializeField] private Sprite sprite;
        [SerializeField] private bool isUnlockedByDefault;
        [SerializeField] private int price;
        
        private bool isUnlocked;

        public string Name => name;
        public Sprite Sprite => sprite;
        public bool IsUnlocked => isUnlocked;
        public int Price => price;

        public void Load()
        {
            isUnlocked = isUnlockedByDefault || Convert.ToBoolean(PlayerPrefs.GetInt(IS_LOCKED_KEY + name));
        }

        public void Unlock()
        {
            isUnlocked = true;
            PlayerPrefs.SetInt(IS_LOCKED_KEY + name, Convert.ToInt32(isUnlocked));
        }
    }
}