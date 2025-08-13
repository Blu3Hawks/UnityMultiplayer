using Player;
using UnityEngine;

namespace Projectiles.SpawningBehaviors.LinesHazard
{
    public class ProjectileWithoutDespawn : Projectile
    {
        protected override void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PlayerManager.PLAYER_TAG))
            {
                PlayerHealthHandler playerHealthHandler = other.GetComponent<PlayerHealthHandler>();
                if (playerHealthHandler != null)
                {
                    if (HasStateAuthority)
                    {
                        playerHealthHandler.RPCTakeDamage(1);
                    }
                }
            }
        }
    }
}