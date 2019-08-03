using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using VoodooTest.Utilities;

namespace VoodooTest.World.Obstacles
{
    public class WorldObstacleView : WorldObjectView
    {
        [SerializeField] private float spawnAnimationDelay = 0.1f;
        [SerializeField] private float spawnAnimationDuration = 0.5f;
        [SerializeField] private Ease spawnAnimationEase = Ease.OutBack;
        
        public override void PlaySpawnAnimation()
        {
            List<WorldObstacleBlock> blocks = GetComponentsInChildren<WorldObstacleBlock>(true).ToList();
            blocks.Shuffle();
            
            for (int i = 0; i < blocks.Count; i++)
            {
                WorldObstacleBlock block = blocks[i];

                Vector3 originalScale = block.transform.localScale;
                block.transform.localScale = Vector3.zero;
                block.transform.DOScale(originalScale, spawnAnimationDuration)
                    .SetEase(spawnAnimationEase)
                    .SetDelay(i * spawnAnimationDelay);
            }
        }
    }
}