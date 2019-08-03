using System;
using UnityEngine;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;
using VoodooTest.World.Sequences;

namespace VoodooTest.World
{
    [RequireComponent(typeof(WorldObjectView))]
    public class WorldObject : MonoBehaviour
    {
        [Tooltip("Optional extra offset for checking when the object passed the camera")]
        [SerializeField] private float cameraCheckOffset;

        protected WorldObjectView worldObjectView;

        private WorldSequenceController worldSequenceController;
        private bool isActive;
        private bool isPassedCamera;
        private GameplayState gameplayState;
        private Camera worldCamera;
        
        public event Action<WorldObject> PassedCameraEvent;
        
        protected virtual void Awake()
        {
            gameplayState = ApplicationManager.GetState<GameplayState>();
            gameplayState.EnterStateEvent += OnGameplayStateEnter;
            gameplayState.ExitStateEvent += OnGameplayStateExit;
            
            // This object can be created after we entered GameplayState, so we also
            // check if we're already in GameplayState
            isActive = ApplicationManager.IsCurrentState<GameplayState>();
            
            worldObjectView = GetComponent<WorldObjectView>();
        }

        protected virtual void OnDestroy()
        {
            gameplayState.EnterStateEvent -= OnGameplayStateEnter;
            gameplayState.ExitStateEvent -= OnGameplayStateExit;
        }

        private void OnGameplayStateEnter()
        {
            isActive = true;
        }

        private void OnGameplayStateExit()
        {
            isActive = false;
        }

        protected virtual void Update()
        {
            if (!isActive || isPassedCamera)
            {
                return;
            }
            
            MoveTowardsCamera();
            CheckForCameraPassed();
        }

        private void MoveTowardsCamera()
        {
            Vector3 position = transform.position;
            position.z -= worldSequenceController.CurrentMoveSpeed;

            transform.position = position;
        }
        
        private void CheckForCameraPassed()
        {
            if (transform.position.z < worldCamera.transform.position.z - cameraCheckOffset)
            {
                isPassedCamera = true;
                PassedCameraEvent?.Invoke(this);
                
                Destroy(gameObject);
            }
        }
        
        public void Setup(Camera worldCamera, WorldSequenceController worldSequenceController, bool showSpawnTween = true)
        {
            this.worldCamera = worldCamera;
            this.worldSequenceController = worldSequenceController;

            if (!showSpawnTween)
            {
                return;
            }
            
            worldObjectView.PlaySpawnAnimation();
        }
    }
}