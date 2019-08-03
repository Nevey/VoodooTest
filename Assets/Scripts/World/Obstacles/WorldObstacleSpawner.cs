using System.Collections.Generic;
using UnityEngine;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;
using VoodooTest.World.Sequences;
using Random = UnityEngine.Random;

namespace VoodooTest.World.Obstacles
{
    public class WorldObstacleSpawner : MonoBehaviour
    {
        [SerializeField] private WorldConfig worldConfig;
        [SerializeField] private WorldSequenceController worldSequenceController;
        [SerializeField] private Camera worldCamera;
        [SerializeField] private WorldObstacle[] worldObstaclePrefabs;

        private readonly List<WorldObstacle> worldObstacles = new List<WorldObstacle>();
        private bool isActive;
        private float distance;
        private GameplayState gameplayState;
        private GameOverState gameOverState;
        private SequenceConfig currentSequenceConfig;

        private void Awake()
        {
            gameplayState = ApplicationManager.GetState<GameplayState>();
            gameplayState.EnterStateEvent += OnGameplayStateEnter;
            gameplayState.ExitStateEvent += OnGameplayStateExit;

            gameOverState = ApplicationManager.GetState<GameOverState>();
            gameOverState.ExitStateEvent += OnGameOverExitState;
            
            worldSequenceController.SequenceUpdatedEvent += OnSequenceUpdated;
        }

        private void OnDestroy()
        {
            gameplayState.EnterStateEvent -= OnGameplayStateEnter;
            gameplayState.ExitStateEvent -= OnGameplayStateExit;
            
            gameOverState.ExitStateEvent -= OnGameOverExitState;
            
            worldSequenceController.SequenceUpdatedEvent -= OnSequenceUpdated;
        }
        
        private void OnGameplayStateEnter()
        {
            isActive = true;
        }

        private void OnGameplayStateExit()
        {
            isActive = false;
        }
        
        private void OnGameOverExitState()
        {
            RemoveAllObstacles();
        }
        
        private void OnSequenceUpdated(SequenceConfig sequenceConfig)
        {
            currentSequenceConfig = sequenceConfig;
        }

        private void Update()
        {
            if (!isActive || currentSequenceConfig == null)
            {
                return;
            }

            if (!currentSequenceConfig.IsRewardSequence)
            {
                distance += worldSequenceController.CurrentMoveSpeed;
            }

            if (distance >= worldConfig.DistanceUntilObstacleSpawn)
            {
                SpawnObstacle();
                distance -= worldConfig.DistanceUntilObstacleSpawn;
            }
        }

        private void SpawnObstacle()
        {
            if (!currentSequenceConfig.WillSpawnObstacles)
            {
                return;
            }
            
            int randomIndex = Random.Range(0, worldObstaclePrefabs.Length);

            WorldObstacle obstacle = Instantiate(worldObstaclePrefabs[randomIndex], transform);
            obstacle.PassedCameraEvent += OnObstaclePassedCamera;
            obstacle.Setup(worldCamera, worldSequenceController);

            if (currentSequenceConfig.WillRotateObstacles)
            {
                obstacle.EnableRotation();
            }

            Vector3 position = transform.position;
            position.z += worldConfig.MaxObstacleSpawnDistance;
            obstacle.transform.position = position;

            Quaternion rotation = obstacle.transform.rotation;
            rotation.eulerAngles = new Vector3(
                0f, 0f, worldSequenceController.CurrentAngle);
            
            obstacle.transform.rotation = rotation;
            
            worldObstacles.Add(obstacle);
        }

        private void OnObstaclePassedCamera(WorldObject obj)
        {
            obj.PassedCameraEvent -= OnObstaclePassedCamera;
            worldObstacles.Remove((WorldObstacle) obj);
        }

        private void RemoveAllObstacles()
        {
            for (int i = 0; i < worldObstacles.Count; i++)
            {
                Destroy(worldObstacles[i].gameObject);
            }
            
            worldObstacles.Clear();
        }
    }
}