using System.Collections.Generic;
using UnityEngine;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;
using VoodooTest.World.Sequences;

namespace VoodooTest.World.Pickups
{
    // TODO: A bunch of these spawners have shared logic, unify it!
    public class PickupSpawner : MonoBehaviour
    {
        [Header("World References")]
        [SerializeField] private WorldConfig worldConfig;
        [SerializeField] private WorldSequenceController worldSequenceController;
        [SerializeField] private Camera worldCamera;
        [SerializeField] private Transform playerTransform;

        [Header("Prefab Settings")]
        [SerializeField] private CoinPickup coinPickupPrefab;

        private readonly List<CoinPickup> coinPickups = new List<CoinPickup>();

        private GameplayState gameplayState;
        private GameOverState gameOverState;
        private bool isActive;
        private SequenceConfig currentSequenceConfig;
        private float distance;
        private float previousAngle;
        private Vector3 pickupSpawnOffset;

        private void Awake()
        {
            gameplayState = ApplicationManager.GetState<GameplayState>();
            gameplayState.EnterStateEvent += OnGameplayStateEnter;
            gameplayState.ExitStateEvent += OnGameplayStateExit;

            gameOverState = ApplicationManager.GetState<GameOverState>();
            gameOverState.ExitStateEvent += OnGameOverStateExit;
            
            worldSequenceController.SequenceUpdatedEvent += OnSequenceUpdated;

            pickupSpawnOffset = playerTransform.position;
        }

        private void OnDestroy()
        {
            gameplayState.EnterStateEvent -= OnGameplayStateEnter;
            gameplayState.ExitStateEvent -= OnGameplayStateExit;
            
            gameOverState.ExitStateEvent -= OnGameOverStateExit;
            
            worldSequenceController.SequenceUpdatedEvent -= OnSequenceUpdated;
        }

        private void Update()
        {
            if (!isActive || currentSequenceConfig == null)
            {
                return;
            }

            distance += worldSequenceController.CurrentMoveSpeed;
            
            // We want pickups to be spawned closer together when we're in a reward sequence
            if (currentSequenceConfig.IsRewardSequence)
            {
                distance += worldSequenceController.CurrentMoveSpeed;
            }

            if (distance >= worldConfig.DistanceUntilCoinSpawn)
            {
                if (currentSequenceConfig.IsRewardSequence)
                {
                    SpawnRewardCoins();
                }
                else
                {
                    SpawnCoin();
                }
                
                distance -= worldConfig.DistanceUntilCoinSpawn;
            }
        }
        
        private void OnGameplayStateEnter()
        {
            isActive = true;
        }
        
        private void OnGameplayStateExit()
        {
            isActive = false;
        }
        
        private void OnGameOverStateExit()
        {
            RemoveAllPickups();
        }
        
        private void OnSequenceUpdated(SequenceConfig sequenceConfig)
        {
            currentSequenceConfig = sequenceConfig;
        }

        private void SpawnCoin(float? overrideAngle = null)
        {
            if (!currentSequenceConfig.WillSpawnCoins)
            {
                return;
            }
            
            // Create coin
            CoinPickup coinPickup = Instantiate(coinPickupPrefab, transform);
            coinPickup.Setup(worldCamera, worldSequenceController);
            coinPickup.PassedCameraEvent += OnCoinPassedCamera;

            // Set coin position and rotation on Y axis
            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(
                rotation.eulerAngles.x,
                rotation.eulerAngles.y,
                overrideAngle ?? worldSequenceController.CurrentAngle);

            coinPickup.transform.rotation = rotation;
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, coinPickup.transform.rotation, Vector3.one);

            coinPickup.transform.position = matrix.MultiplyPoint3x4(pickupSpawnOffset);
            
            // Set coin position on Z axis
            Vector3 position = coinPickup.transform.position;
            position.z += worldConfig.MaxObstacleSpawnDistance;
            coinPickup.transform.position = position;

            // Add coin to list
            coinPickups.Add(coinPickup);
        }

        private void SpawnRewardCoins()
        {
            float angle = 0;
            
            for (int i = 0; i < 20; i++)
            {
                angle += 360 / 20;
                SpawnCoin(angle);
            }
        }

        private void OnCoinPassedCamera(WorldObject obj)
        {
            obj.PassedCameraEvent -= OnCoinPassedCamera;
            coinPickups.Remove((CoinPickup) obj);
        }

        private void RemoveAllPickups()
        {
            for (int i = 0; i < coinPickups.Count; i++)
            {
                Destroy(coinPickups[i].gameObject);
            }
            
            coinPickups.Clear();
        }
    }
}