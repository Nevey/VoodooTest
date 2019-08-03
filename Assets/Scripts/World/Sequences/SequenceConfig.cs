using UnityEngine;

namespace VoodooTest.World.Sequences
{
    [CreateAssetMenu(fileName = "SequenceConfig", menuName = "Sequences/SequenceConfig")]
    public class SequenceConfig : ScriptableObject
    {
        [SerializeField, Range(1, 30)] private int minDuration;
        [SerializeField, Range(1, 30)] private int maxDuration;
        [SerializeField] private bool willSpawnCoins;
        [SerializeField] private bool willSpawnObstacles;
        [SerializeField] private bool willRotateObstacles;
        [SerializeField] private bool isRewardSequence;

        public int MinDuration => minDuration;
        public int MaxDuration => maxDuration;
        public bool WillSpawnCoins => willSpawnCoins;
        public bool WillSpawnObstacles => willSpawnObstacles;
        public bool WillRotateObstacles => willRotateObstacles;
        public bool IsRewardSequence => isRewardSequence;

        private void OnValidate()
        {
            if (maxDuration < minDuration)
            {
                minDuration = maxDuration;
            }

            if (isRewardSequence)
            {
                willSpawnObstacles = false;
                willRotateObstacles = false;
                willSpawnCoins = true;
            }
        }
    }
}