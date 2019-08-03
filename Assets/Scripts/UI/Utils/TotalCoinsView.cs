using TMPro;
using UnityEngine;
using VoodooTest.Player;

namespace VoodooTest.UI.Utils
{
    public class TotalCoinsView : MonoBehaviour
    {
        [SerializeField] private CoinsTracker coinsTracker;
        [SerializeField] private TextMeshProUGUI coinsValue;

        private void Awake()
        {
            coinsValue.SetText(coinsTracker.TotalCoinsCollected.ToString());
            
            coinsTracker.TotalCoinsUpdatedEvent += OnTotalCoinsUpdated;
        }

        private void OnDestroy()
        {
            coinsTracker.TotalCoinsUpdatedEvent -= OnTotalCoinsUpdated;
        }

        private void OnTotalCoinsUpdated(int totalCoinsCollected)
        {
            coinsValue.SetText(totalCoinsCollected.ToString());
        }
    }
}