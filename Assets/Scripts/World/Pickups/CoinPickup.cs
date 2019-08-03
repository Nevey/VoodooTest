using UnityEngine;

namespace VoodooTest.World.Pickups
{
    public class CoinPickup : WorldObject
    {
        private void OnTriggerEnter(Collider other)
        {
            // If colliding with an obstacle block, remove me!
            
            if (other.gameObject.layer != 11)
            {
                return;
            }

            // TODO: Make dependency on a specific view a bit safer
            CoinView coinView = (CoinView) worldObjectView; 
            coinView.PlayCollectAnimation();
        }
    }
}