using DG.Tweening;
using UnityEngine;

namespace VoodooTest.World.Circles
{
    public class WorldCircleView : WorldObjectView
    {
        [SerializeField] private float spawnScale;
        [SerializeField] private float spawnAnimationDuration = 0.5f;
        [SerializeField] private Ease spawnAnimationEase = Ease.OutBack;
        
        private Vector3 originalScale;
        
        private void Awake()
        {
            originalScale = transform.localScale;
        }

        public override void PlaySpawnAnimation()
        {
            transform.localScale =
                new Vector3(originalScale.x * spawnScale, originalScale.y * spawnScale, originalScale.z);
            
            transform.DOScale(originalScale, spawnAnimationDuration).SetEase(spawnAnimationEase);
        }
    }
}