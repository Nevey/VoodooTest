using UnityEngine;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;
using VoodooTest.World.Sequences;

namespace VoodooTest.Player
{
    public class ScoreTracker : MonoBehaviour
    {
        private const string SCORE_RECORD_KEY = "VoodooTest.ScoreRecordKey";
        
        [SerializeField] private ObstacleHitDetector obstacleHitDetector;
        [SerializeField] private WorldSequenceController worldSequenceController;

        private GameplayState gameplayState;
        private int score;
        private int scoreRecord;
        private bool isNewRecord;

        public int Score => score;
        public bool IsNewRecord => isNewRecord;

        private void Awake()
        {
            gameplayState = ApplicationManager.GetState<GameplayState>();
            gameplayState.EnterStateEvent += OnGameplayStateEnter;
            gameplayState.ExitStateEvent += OnGameplayStateExit;
            
            obstacleHitDetector.HitScoreEvent += OnHitScore;
            obstacleHitDetector.HitCoinPickupEvent += OnHitCoinPickup;
        }

        private void OnDestroy()
        {
            obstacleHitDetector.HitScoreEvent -= OnHitScore;
            obstacleHitDetector.HitCoinPickupEvent -= OnHitCoinPickup;
        }

        private void OnGameplayStateEnter()
        {
            score = 0;
            scoreRecord = PlayerPrefs.GetInt(SCORE_RECORD_KEY);
            isNewRecord = false;
        }
        
        private void OnGameplayStateExit()
        {
            SaveScoreRecord();
        }

        private void OnHitScore()
        {
            score += 5 * worldSequenceController.CurrentMultiplier;
        }
        
        private void OnHitCoinPickup()
        {
            score += worldSequenceController.CurrentMultiplier;
        }

        private void SaveScoreRecord()
        {
            if (score > scoreRecord)
            {
                PlayerPrefs.SetInt(SCORE_RECORD_KEY, score);
                isNewRecord = true;
            }
        }
    }
}