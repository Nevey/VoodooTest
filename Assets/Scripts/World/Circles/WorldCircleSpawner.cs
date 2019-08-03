using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoodooTest.World.Sequences;

namespace VoodooTest.World.Circles
{
    public class WorldCircleSpawner : MonoBehaviour
    {
        [Header("World Configuration Reference")]
        [SerializeField] private WorldConfig worldConfig;
        [SerializeField] private WorldSequenceController worldSequenceController;
        
        [Header("World Circle Prefabs")]
        [SerializeField] private WorldObject bezierCircleLightPrefab;
        [SerializeField] private WorldObject bezierCircleDarkPrefab;

        [Header("Spawn Settings")]
        [SerializeField] private Camera worldCamera;
        
        [Header("Debug Settings")]
        [SerializeField] private Color gizmoColor = Color.cyan;
        
        private readonly List<WorldObject> worldCircleControllers = new List<WorldObject>();

        private Bounds bezierCircleBounds;
        private Coroutine spawnCirclesRoutine;
        private bool spawnLight = true;

        private void Awake()
        {
            if (worldConfig == null)
            {
                throw new Exception("WorldConfig is not assign on WorldBuilder!");
            }
            
            // Both prefabs have the same size
            bezierCircleBounds = bezierCircleLightPrefab.GetComponent<Renderer>().bounds;
            
            SpawnCircles(true);

            // Start world spawning enumerator, didn't wanna shove this in update for performance purposes
            spawnCirclesRoutine = StartCoroutine(SpawnCirclesEnumerator());
        }

        private void OnDestroy()
        {
            StopCoroutine(spawnCirclesRoutine);
            worldCircleControllers.Clear();
        }

        private void OnDrawGizmos()
        {
            if (worldCamera == null || worldConfig == null)
            {
                return;
            }
            
            // Draw a little cube to help visualize the world's "size"
            Vector3 origin = worldCamera.transform.position;
            origin.z += worldConfig.MaxCircleSpawnDistance / 2f;

            // Value "5" is arbitrary
            Vector3 size = new Vector3(5f, 5f, worldConfig.MaxCircleSpawnDistance);
            
            Gizmos.color = gizmoColor;
            Gizmos.DrawCube(origin, size);
        }

        private void SpawnCircles(bool isInitialSpawn = false)
        {
            int requiredCirclesCount = (int)Mathf.Ceil(worldConfig.MaxCircleSpawnDistance / bezierCircleBounds.size.z);
            
            // Check if we need to spawn new circles to fill the world
            if (requiredCirclesCount <= worldCircleControllers.Count)
            {
                return;
            }

            int amountToSpawn = requiredCirclesCount - worldCircleControllers.Count;

            // Spawn the amount of circles required to fill the world
            for (int i = 0; i < amountToSpawn; i++)
            {
                SpawnCircle(isInitialSpawn);
            }
        }

        private void SpawnCircle(bool isInitialSpawn)
        {
            // Check which prefab to instantiate
            WorldObject worldCirclePrefab =
                spawnLight ? bezierCircleLightPrefab : bezierCircleDarkPrefab;

            spawnLight = !spawnLight;
            
            // Create new circle
            WorldObject worldCircle = Instantiate(worldCirclePrefab, transform);
            worldCircle.Setup(worldCamera, worldSequenceController, !isInitialSpawn);
            worldCircle.PassedCameraEvent += OnWorldCirclePassedCamera;
            
            // Place it in the world, based on camera position and amount of existing circles
            Vector3 position = worldCamera.transform.position;
            
            if (worldCircleControllers.Count > 0)
            {
                WorldObject lastCircleCreated = worldCircleControllers[worldCircleControllers.Count - 1];
                position.z = lastCircleCreated.transform.position.z + bezierCircleBounds.size.z;
            }
            
            worldCircle.transform.position = position;
            
            // Add circle to the existing circles list
            worldCircleControllers.Add(worldCircle);
        }

        private void OnWorldCirclePassedCamera(WorldObject worldObject)
        {
            worldObject.PassedCameraEvent -= OnWorldCirclePassedCamera;
            worldCircleControllers.Remove(worldObject);
        }

        private IEnumerator SpawnCirclesEnumerator()
        {
            while (true)
            {
                SpawnCircles();
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }
    }
}