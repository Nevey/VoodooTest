using DG.Tweening;
using UnityEngine;

namespace VoodooTest.World.Pickups
{
    public class CoinView : WorldObjectView
    {
        [Header("Spawn Animation Settings")]
        [SerializeField] private float spawnAnimationDuration = 0.5f;
        [SerializeField] private Ease spawnAnimationEase = Ease.OutBack;
        [SerializeField] private float spawnScale;

        [Header("Collect Animation Settings")]
        [SerializeField] private ParticleSystem collectParticlesPrefab;

        private new Renderer renderer;
        private Vector3 originalScale;
        
        private void Awake()
        {
            renderer = GetComponent<Renderer>();
            originalScale = transform.localScale;
        }

        public override void PlaySpawnAnimation()
        {
            transform.localScale =
                new Vector3(originalScale.x * spawnScale, originalScale.y * spawnScale, originalScale.z);
            
            transform.DOScale(originalScale, spawnAnimationDuration).SetEase(spawnAnimationEase);
        }

        public void PlayCollectAnimation()
        {
            renderer.enabled = false;

            ParticleSystem particleSystem = Instantiate(collectParticlesPrefab, transform);
            particleSystem.transform.position = transform.position;
            particleSystem.Play();
        }
    }
}