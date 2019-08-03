using UnityEngine;
using VoodooTest.ApplicationManagement;
using VoodooTest.ApplicationManagement.States;
using VoodooTest.Themes;
using VoodooTest.World.Sequences;

namespace VoodooTest.Player
{
    public class PlayerView : MonoBehaviour
    {
        [SerializeField] private WorldSequenceController worldSequenceController;
        [SerializeField] private ObstacleHitDetector obstacleHitDetector;
        [SerializeField] private ParticleSystem deathParticlesPrefab;
        [SerializeField] private ParticleSystem trailParticlesPrefab;
        [SerializeField] private PlayerSkinController playerSkinController;
        [SerializeField] private float rotationSpeed = 15f;
        [SerializeField] private new Renderer renderer;
        
        private MainMenuState mainMenuState;
        private GameplayState gameplayState;
        private bool isActive;
        private ParticleSystem trailParticles;

        private float angle;

        private void Start()
        {
            mainMenuState = ApplicationManager.GetState<MainMenuState>();
            mainMenuState.EnterStateEvent += OnMainMenuEnterState;
            
            gameplayState = ApplicationManager.GetState<GameplayState>();
            gameplayState.EnterStateEvent += OnGameplayEnterState;
            gameplayState.ExitStateEvent += OnGameplayExitState;
            
            obstacleHitDetector.HitObstacleEvent += OnHitObstacle;
        }

        private void OnDestroy()
        {
            mainMenuState.EnterStateEvent -= OnMainMenuEnterState;
            
            gameplayState.EnterStateEvent -= OnGameplayEnterState;
            gameplayState.ExitStateEvent -= OnGameplayExitState;
            
            obstacleHitDetector.HitObstacleEvent -= OnHitObstacle;
        }

        private void Update()
        {
            if (!isActive)
            {
                return;
            }

            renderer.transform.Rotate(Vector3.right, worldSequenceController.CurrentMoveSpeed * rotationSpeed, Space.Self);
            
            trailParticles.transform.position = transform.position;
        }

        private void OnMainMenuEnterState()
        {
            renderer.enabled = false;

            if (trailParticles != null)
            {
                Destroy(trailParticles.gameObject);
            }
        }

        private void OnGameplayEnterState()
        {
            isActive = true;
            
            renderer.material.SetTexture("_MainTex", playerSkinController.GetSelectedSkin().Sprite.texture);
            renderer.enabled = true;

            trailParticles = Instantiate(trailParticlesPrefab);
            trailParticles.GetComponent<Renderer>().material
                .SetTexture("_MainTex", playerSkinController.GetSelectedSkin().Sprite.texture);
            trailParticles.Play();
        }
        
        private void OnGameplayExitState()
        {
            isActive = false;
            trailParticles.Stop();
        }

        private void OnHitObstacle(GameObject gameObject)
        {
            // TODO: Based on powerup, show something different!
            PlayDeathAnimation();
        }

        private void PlayDeathAnimation()
        {
            renderer.enabled = false;
            trailParticlesPrefab.Stop();

            // ParticleSystem will destroy itself when it's finished
            ParticleSystem deathParticles = Instantiate(deathParticlesPrefab);
            deathParticles.transform.position = transform.position;
            deathParticles.transform.rotation = Quaternion.identity;
            deathParticles.GetComponent<Renderer>().material
                .SetTexture("_MainTex", playerSkinController.GetSelectedSkin().Sprite.texture);
            deathParticles.Play();
        }
    }
}