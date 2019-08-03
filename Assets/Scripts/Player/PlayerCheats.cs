using UnityEngine;

namespace VoodooTest.Player
{
    public class PlayerCheats : MonoBehaviour
    {
#if UNITY_EDITOR
        
        private bool isGodMode;

        public bool IsGodMode => isGodMode;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                PlayerPrefs.DeleteAll();
                Debug.Log("Deleted all player prefs!");
            }
            
            if (Input.GetKeyDown(KeyCode.G))
            {
                isGodMode = !isGodMode;
                Debug.Log($"Is God Mode {isGodMode}");
            }
        }
#endif
    }
}