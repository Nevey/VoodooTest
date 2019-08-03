using TMPro;
using UnityEngine;

namespace VoodooTest.Utilities
{
    public class TextBlinkUtil : MonoBehaviour
    {
        [SerializeField] private float blinkDuration;
        [SerializeField] private TextMeshProUGUI textToBlink;

        private float currentDuration;
        
        private void Update()
        {
            currentDuration += Time.deltaTime;

            if (currentDuration >= blinkDuration)
            {
                currentDuration -= blinkDuration;
                textToBlink.enabled = !textToBlink.enabled;
            }
        }
    }
}