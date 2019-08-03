using System;
using UnityEngine;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;
using VoodooTest.World.Sequences;

namespace VoodooTest.Player
{
    public class CoinsTracker : MonoBehaviour
    {
        private const string TOTAL_COINS_COLLECTED_KEY = "VoodooTest.TotalCoinsCollectedKey";
        
        [SerializeField] private ObstacleHitDetector obstacleHitDetector;
        [SerializeField] private WorldSequenceController worldSequenceController;

        private GameplayState gameplayState;
        private int totalCoinsCollected;

        public int TotalCoinsCollected => totalCoinsCollected;

        public event Action<int> TotalCoinsUpdatedEvent; 

        private void Awake()
        {
            totalCoinsCollected = PlayerPrefs.GetInt(TOTAL_COINS_COLLECTED_KEY);
            
            gameplayState = ApplicationManager.GetState<GameplayState>();
            gameplayState.ExitStateEvent += OnGameplayStateExit;
            
            obstacleHitDetector.HitCoinPickupEvent += OnHitCoinPickup;
        }

        private void OnDestroy()
        {
            gameplayState.ExitStateEvent -= OnGameplayStateExit;
            obstacleHitDetector.HitCoinPickupEvent -= OnHitCoinPickup;
        }
        
        private void OnGameplayStateExit()
        {
            PlayerPrefs.SetInt(TOTAL_COINS_COLLECTED_KEY, totalCoinsCollected);
        }

        private void OnHitCoinPickup()
        {
            totalCoinsCollected += worldSequenceController.CurrentMultiplier;
            TotalCoinsUpdatedEvent?.Invoke(totalCoinsCollected);
        }

        public void SpendCoins(int amount)
        {
            if (amount > totalCoinsCollected)
            {
                throw new Exception("Trying to spend more coins than available, this is never allowed to happen!");
            }
            
            totalCoinsCollected -= amount;
            PlayerPrefs.SetInt(TOTAL_COINS_COLLECTED_KEY, totalCoinsCollected);
            
            TotalCoinsUpdatedEvent?.Invoke(totalCoinsCollected);
        }
    }
}