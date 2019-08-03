using System;
using UnityEngine;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;
using Random = UnityEngine.Random;

namespace VoodooTest.World.Sequences
{
    public class WorldSequenceController : MonoBehaviour
    {
        [Header("World References")]
        [SerializeField] private WorldConfig worldConfig;

        [Header("Path Settings")]
        [SerializeField] private float angleStepDistance = 0.2f;
        [SerializeField] private float angleStepStrength = 100f;
        [SerializeField] private float minDistanceUntilAngleSwap = 5f;
        [SerializeField] private float maxDistanceUntilAngleSwap = 20f;

        [Header("Difficulty Settings")]
        [SerializeField] private AnimationCurve difficultyCurve;
        [SerializeField, Tooltip("Time in Seconds")] private int timeUntilMaxDifficulty = 60;
        
        [Header("Sequence Settings")]
        [Tooltip("Sequences will be played in the order of this array")]
        [SerializeField] private SequenceConfig[] buildupSequenceConfigs;
        [Tooltip("Will loop these sequences once the buildup is done")]
        [SerializeField] private SequenceConfig[] loopableSequenceConfigs;
        
        [Header("Obstacle Visuals Settings")]
        [SerializeField] private Material obstacleBlockMaterial;
        [SerializeField] private Color easyColor;
        [SerializeField] private Color difficultColor;

        [Header("Multiplier Settings")]
        [SerializeField] private int maxMultiplier = 5;
        [SerializeField] private float requiredDistanceForMultiplierBump = 100f;

        private enum AngleDirection
        {
            Left = -1,
            Right = 1
        }
        
        private GameplayState gameplayState;
        private bool isActive;

        private float currentPlayTime;
        private float currentDifficulty;
        private float currentMoveSpeed;

        private float currentAngleStepDistance;
        private float currentAngleSwapDistance;
        private float requiredAngleSwapDistance;
        private AngleDirection angleDirection;
        private float currentAngle;

        private SequenceConfig currentSequenceConfig;
        private SequenceConfig[] currentSequenceArray;
        private int currentSequenceIndex;
        private float currentSequenceDuration;
        private float sequenceDuration;

        private float currentDistanceForMultiplierBump;
        private int currentMultiplier;
        
        public float CurrentMoveSpeed => currentMoveSpeed;
        public float CurrentAngle => currentAngle;
        public int CurrentMultiplier => currentMultiplier;

        public event Action<SequenceConfig> SequenceUpdatedEvent;
        public event Action<int> MultiplierUpdatedEvent;

        private void Awake()
        {
            gameplayState = ApplicationManager.GetState<GameplayState>();
            gameplayState.EnterStateEvent += OnGameplayStateEnter;
            gameplayState.ExitStateEvent += OnGameplayStateExit;
        }

        private void OnDestroy()
        {
            gameplayState.EnterStateEvent -= OnGameplayStateEnter;
            gameplayState.ExitStateEvent -= OnGameplayStateExit;
        }

        private void Update()
        {
            if (!isActive)
            {
                return;
            }

            currentPlayTime += Time.deltaTime;
            
            UpdateDifficulty();
            UpdateMoveSpeed();
            UpdateObstacleColor();

            currentDistanceForMultiplierBump += currentMoveSpeed;
            if (currentDistanceForMultiplierBump >= requiredDistanceForMultiplierBump * currentMultiplier)
            {
                currentDistanceForMultiplierBump -= requiredDistanceForMultiplierBump;
                UpdateMultiplier();
            }

            sequenceDuration += Time.deltaTime;
            if (sequenceDuration >= currentSequenceDuration)
            {
                sequenceDuration -= currentSequenceDuration;
                UpdateSequence();
            }

            currentAngleStepDistance += currentMoveSpeed;
            if (currentAngleStepDistance >= angleStepDistance)
            {
                currentAngleStepDistance -= angleStepDistance;
                UpdateAngle();
            }
            
            currentAngleSwapDistance += currentMoveSpeed;
            if (currentAngleSwapDistance >= requiredAngleSwapDistance)
            {
                currentAngleSwapDistance -= requiredAngleSwapDistance;
                SwapAngle();
            }
        }
        
        private void OnGameplayStateEnter()
        {
            isActive = true;

            currentMultiplier = 0;
            currentDistanceForMultiplierBump = 0f;
            
            UpdateSequence(true);

            currentPlayTime = 0f;
            UpdateDifficulty();
            
            currentAngleSwapDistance = 0f;
            angleDirection = (AngleDirection)Random.Range(0, Enum.GetValues(typeof(AngleDirection)).Length);

            SwapAngle();
        }

        private void OnGameplayStateExit()
        {
            isActive = false;
        }

        private void UpdateSequence(bool isInitialSequence = false)
        {
            if (isInitialSequence)
            {
                currentSequenceArray = buildupSequenceConfigs;
                currentSequenceIndex = 0;
                sequenceDuration = 0f;
            }
            else
            {
                if (++currentSequenceIndex >= currentSequenceArray.Length)
                {
                    currentSequenceIndex = 0;

                    if (currentSequenceArray == buildupSequenceConfigs)
                    {
                        currentSequenceArray = loopableSequenceConfigs;
                    }
                }
            }

            currentSequenceConfig = currentSequenceArray[currentSequenceIndex];
            currentSequenceDuration = Random.Range(currentSequenceConfig.MinDuration, currentSequenceConfig.MaxDuration);
            
            SequenceUpdatedEvent?.Invoke(currentSequenceConfig);
        }

        private void UpdateMultiplier()
        {
            if (currentMultiplier >= maxMultiplier)
            {
                return;
            }
            
            currentMultiplier++;
            MultiplierUpdatedEvent?.Invoke(currentMultiplier);
        }

        private void UpdateDifficulty()
        {
            float relativeDifficultyTime = 1f / timeUntilMaxDifficulty * currentPlayTime;
            currentDifficulty = difficultyCurve.Evaluate(relativeDifficultyTime);
        }

        private void UpdateMoveSpeed()
        {
            currentMoveSpeed = worldConfig.MoveSpeed * currentDifficulty * Time.deltaTime;
        }

        private void UpdateAngle()
        {
            currentAngle += (int)angleDirection * angleStepStrength * Time.deltaTime;
        }

        private void SwapAngle()
        {
            requiredAngleSwapDistance = Random.Range(minDistanceUntilAngleSwap, maxDistanceUntilAngleSwap);
            angleDirection = angleDirection == AngleDirection.Left ? AngleDirection.Right : AngleDirection.Left;
        }

        private void UpdateObstacleColor()
        {
            float relativeDifficultyTime = 1f / timeUntilMaxDifficulty * currentPlayTime;
            Color color = Color.Lerp(easyColor, difficultColor, relativeDifficultyTime);
            obstacleBlockMaterial.SetColor("_Color", color);
        }
    }
}