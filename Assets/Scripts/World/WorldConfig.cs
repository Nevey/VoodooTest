using UnityEngine;

namespace VoodooTest.World
{
    [CreateAssetMenu(fileName = "WorldConfig", menuName = "World/WorldConfig")]
    public class WorldConfig : ScriptableObject
    {
        [Header("Motion Settings")]
        [SerializeField] private float moveSpeed = 1f;
        
        [Header("Circle Settings")]
        [SerializeField] private float maxCircleSpawnDistance = 20f;
        
        [Header("Obstacle Settings")]
        [SerializeField] private float distanceUntilObstacleSpawn = 10f;
        [SerializeField] private float maxObstacleSpawnDistance = 20f;

        [Header("Pickup Settings")]
        [SerializeField] private float distanceUntilCoinSpawn = 1f;

        public float MoveSpeed => moveSpeed;
        public float MaxCircleSpawnDistance => maxCircleSpawnDistance;
        public float DistanceUntilObstacleSpawn => distanceUntilObstacleSpawn;
        public float MaxObstacleSpawnDistance => maxObstacleSpawnDistance;
        public float DistanceUntilCoinSpawn => distanceUntilCoinSpawn;
    }
}