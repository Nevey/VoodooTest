using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;
using Random = UnityEngine.Random;

namespace VoodooTest.World.Obstacles
{
    public class WorldObstacle : WorldObject
    {
        [SerializeField, Range(0, 20)] private int blockCount;
        [SerializeField] private WorldObstacleBlock worldObstacleBlockPrefab;
        [SerializeField] private float rotationSpeed = 0.75f;

        private Sequence rotationSequence;
        private GameOverState gameOverState;

        private enum RotationState
        {
            OneOff,
            Constant,
            Rocking
        }

        protected override void Awake()
        {
            base.Awake();

            gameOverState = ApplicationManager.GetState<GameOverState>();
            gameOverState.EnterStateEvent += OnGameOverStateEnter;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            gameOverState.EnterStateEvent -= OnGameOverStateEnter;
        }
        
        private void OnGameOverStateEnter()
        {
            rotationSequence.Kill();
        }

        private void StartRotation(RotationState rotationState)
        {
            switch (rotationState)
            {
                // Rotate 90 degrees just once
                case RotationState.OneOff:
                    DoRotation(GetAngle(), Ease.InOutSine);
                    break;
                
                // Rotate constantly
                // Just doing a 360 degrees as the obstacle will be long gone before this tween is finished
                // NOTE: Decided to not do rocking rotation as it's too big of a punishment
                case RotationState.Rocking:
                case RotationState.Constant:
                    DoRotation(360f);
                    break;
                
                // Rotate 90 degrees back and forth
                // Just doing this twice, as the obstacle will be long gone by the end of the rocking tween
//                case RotationState.Rocking:
//                    DoRotation(GetAngle(), Ease.InOutSine, 2);
//                    break;
            }
        }

        private float GetAngle()
        {
            return Random.Range(45f, 90f);
        }

        private int GetDirection()
        {
            int direction = Random.Range(-1, 1);
            if (direction == 0)
            {
                direction = 1;
            }

            return direction;
        }

        private float GetDuration(float angle)
        {
            return (angle / 90f) * rotationSpeed;
        }

        private void DoRotation(float angle, Ease ease = Ease.Linear, int amount = 1)
        {
            rotationSequence = DOTween.Sequence();

            float direction = GetDirection();
            
            Vector3 targetRotation = transform.eulerAngles;

            for (int i = 0; i < amount; i++)
            {
                targetRotation.z += angle * direction;

                rotationSequence.Append(
                    transform.DOLocalRotate(targetRotation, GetDuration(angle), RotateMode.LocalAxisAdd)
                        .SetEase(ease));
                
                // Turn direction the other way around, so we can do some rocking...
                // We'll have to multiply by 2 since the rotation tween is always relative
                direction = -direction * 2;
            }
        }

        /// <summary>
        /// Enables rotation behaviour based on a configurable chance
        /// </summary>
        public void EnableRotation()
        {
            // Get a random rotation state
            int randomRotationIndex = Random.Range(0, Enum.GetValues(typeof(RotationState)).Length);
            StartRotation((RotationState) randomRotationIndex);
            
        }

        /// <summary>
        /// Updates the obstacle blocks based on the amount of blocks we want. Only runs in editor mode
        /// </summary>
        public void UpdateBlocksEditMode()
        {
            if (Application.isPlaying)
            {
                return;
            }
            
            List<WorldObstacleBlock> blocks = GetComponentsInChildren<WorldObstacleBlock>(true).ToList();

            int difference = Mathf.Abs(blockCount - blocks.Count);
            if (difference == 0)
            {
                return;
            }
            
            bool willAdd = blockCount > blocks.Count;
            
            for (int i = 0; i < difference; i++)
            {
                if (willAdd)
                {
                    Instantiate(worldObstacleBlockPrefab, transform);
                }
                else
                {
                    int index = blocks.Count - 1 - i;
                    DestroyImmediate(blocks[index].gameObject);
                }
            }
        }
    }
}