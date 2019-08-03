using DG.Tweening;
using UnityEngine;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;

namespace VoodooTest.Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class CircularMovement : MonoBehaviour
    {
        [SerializeField] private float movementSpeed = 1;

        private GameplayState gameplayState;
        private GameOverState gameOverState;
        private PlayerInput playerInput;
        
        private bool isActive;
        private bool isStartJumpDone;
        
        private Vector3 originalPosition;

        private float targetDelta;
        private float currentDelta;
        private float deltaVelocity;
        private float currentAngle;
        private bool hadPointerUp;
        
        private void Awake()
        {
            gameplayState = ApplicationManager.GetState<GameplayState>();
            gameplayState.EnterStateEvent += OnGameplayStateEnter;
            gameplayState.ExitStateEvent += OnGameplayStateExit;

            gameOverState = ApplicationManager.GetState<GameOverState>();
            gameOverState.ExitStateEvent += OnGameOverStateExit;
            
            playerInput = GetComponent<PlayerInput>();
            playerInput.HorizontalDragEvent += OnHorizontalDrag;
            playerInput.PointerUpEvent += OnPointerUp;

            originalPosition = transform.position;
        }
        
        private void OnDestroy()
        {
            gameplayState.EnterStateEvent -= OnGameplayStateEnter;
            gameplayState.ExitStateEvent -= OnGameplayStateExit;
            
            gameOverState.ExitStateEvent -= OnGameOverStateExit;
            
            playerInput.HorizontalDragEvent -= OnHorizontalDrag;
            playerInput.PointerUpEvent -= OnPointerUp;
        }

        private void Update()
        {
            if (!isActive)
            {
                return;
            }
            
            DoTranslation();
        }

        private void OnGameplayStateEnter()
        {
            isActive = true;
            
            currentAngle = 0f;
            currentDelta = 0f;
            targetDelta = 0f;
            
            transform.DOLocalJump(transform.position, 0.5f, 1, 0.5f).SetEase(Ease.Linear)
                .OnComplete(() => { isStartJumpDone = true; });
        }
        
        private void OnGameplayStateExit()
        {
            isActive = false;
            isStartJumpDone = false;
        }

        private void OnGameOverStateExit()
        {
            // TODO: Does not work yet, trail renderer shows when doing this at this point
            // Resetting the position at this point in the game to avoid trail renderer being ugly...
            transform.position = originalPosition;
        }
        
        private void OnHorizontalDrag(float delta)
        {
            if (!isStartJumpDone)
            {
                return;
            }
            
            if (hadPointerUp)
            {
                targetDelta = 0f;
                currentDelta = 0f;
                hadPointerUp = false;
                return;
            }
            
            targetDelta = delta * movementSpeed;
        }
        
        private void OnPointerUp()
        {
            if (!isStartJumpDone)
            {
                return;
            }
            
            targetDelta = 0f;
            currentDelta = 0f;
            hadPointerUp = true;
        }
        
        private void DoTranslation()
        {
            if (hadPointerUp)
            {
                return;
            }
            
            // Mathf.SmoothDamp is taking Time.deltaTime into account
            currentDelta = Mathf.SmoothDamp(currentDelta, targetDelta , ref deltaVelocity, 0.01f);
            currentAngle += currentDelta;
            
            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(
                rotation.eulerAngles.x,
                rotation.eulerAngles.y,
                currentAngle);

            transform.rotation = rotation;
            
            // We're assuming the point we want to rotate around is at 0, 0, 0 in the world
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, transform.rotation, Vector3.one);

            transform.position = matrix.MultiplyPoint3x4(originalPosition);
        }
    }
}