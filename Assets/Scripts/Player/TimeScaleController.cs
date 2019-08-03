using UnityEngine;

namespace VoodooTest.Player
{
    public class TimeScaleController : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private ObstacleHitDetector obstacleHitDetector;

        private void Awake()
        {
            playerInput.PointerDownEvent += OnPointerDown;
            playerInput.PointerUpEvent += OnPointerUp;
            
            obstacleHitDetector.HitObstacleEvent += OnHitObstacle;
        }

        private void OnDestroy()
        {
            playerInput.PointerDownEvent -= OnPointerDown;
            playerInput.PointerUpEvent -= OnPointerUp;
            
            obstacleHitDetector.HitObstacleEvent -= OnHitObstacle;
        }

        private void OnPointerDown()
        {
            Time.timeScale = 1f;
        }
        
        private void OnPointerUp()
        {
            Time.timeScale = 0.01f;
        }
        
        private void OnHitObstacle(GameObject obj)
        {
            Time.timeScale = 1f;
        }
    }
}