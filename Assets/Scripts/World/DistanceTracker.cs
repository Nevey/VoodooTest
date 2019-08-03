using UnityEngine;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;
using VoodooTest.World.Sequences;

namespace VoodooTest.World
{
    public class DistanceTracker : MonoBehaviour
    {
        private const string DISTANCE_RECORD_KEY = "VoodooTest.DistanceRecord";

        [SerializeField] private WorldSequenceController worldSequenceController;

        private float distanceTraveled;
        private float distanceRecord;
        private GameplayState gameplayState;
        private bool isNewRecord;
        private bool isActive;

        public float DistanceTraveled => distanceTraveled;
        public bool IsNewRecord => isNewRecord;

        private void Awake()
        {
            gameplayState = ApplicationManager.GetState<GameplayState>();
            gameplayState.EnterStateEvent += OnGameplayEnterState;
            gameplayState.ExitStateEvent += OnGameplayExitState;
        }

        private void OnDestroy()
        {
            gameplayState.EnterStateEvent -= OnGameplayEnterState;
            gameplayState.ExitStateEvent -= OnGameplayExitState;
        }
        
        private void Update()
        {
            if (!isActive)
            {
                return;
            }

            distanceTraveled += worldSequenceController.CurrentMoveSpeed;
        }
        
        private void OnGameplayEnterState()
        {
            distanceTraveled = 0f;
            distanceRecord = PlayerPrefs.GetFloat(DISTANCE_RECORD_KEY);
            isNewRecord = false;
            isActive = true;
        }
        
        private void OnGameplayExitState()
        {
            SaveCurrentDistanceAsRecord();
            isActive = false;
        }
        
        private void SaveCurrentDistanceAsRecord()
        {
            if (distanceTraveled > distanceRecord)
            {
                PlayerPrefs.SetFloat(DISTANCE_RECORD_KEY, distanceTraveled);
                isNewRecord = true;
            }
        }
    }
}