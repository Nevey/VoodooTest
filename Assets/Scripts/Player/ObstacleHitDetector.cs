using System;
using UnityEngine;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;

namespace VoodooTest.Player
{
    public class ObstacleHitDetector : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private PlayerCheats playerCheats;
#endif
        
        public event Action HitScoreEvent;
        public event Action HitCoinPickupEvent;
        public event Action<GameObject> HitObstacleEvent;

        private void OnTriggerEnter(Collider other)
        {
#if UNITY_EDITOR
            if (playerCheats.IsGodMode)
            {
                return;
            }
#endif
            if (other.gameObject.layer == 8)
            {
                HitScoreEvent?.Invoke();
                return;
            }

            if (other.gameObject.layer == 9)
            {
                HitCoinPickupEvent?.Invoke();
                return;
            }
            
            HitObstacleEvent?.Invoke(other.gameObject);
            ApplicationManager.GoToState<GameOverState>();
        }
    }
}