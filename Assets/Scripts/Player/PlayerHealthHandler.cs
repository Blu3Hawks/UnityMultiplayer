using Fusion;
using Projectiles;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class PlayerHealthHandler : NetworkBehaviour
    {
        [SerializeField] private PlayerManager playerManager;

        [Header("Player Settings")]
        [Networked, OnChangedRender(nameof(OnHealthChanged))]
        [field: SerializeField]
        public int Health { get; set; }

        public event UnityAction OnDeath;

        private void OnEnable()
        {
            Projectile.OnProjectileSpawned += OnPlayerHitSpawnParticles;
        }

        private void OnDisable()
        {
            Projectile.OnProjectileSpawned -= OnPlayerHitSpawnParticles;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RPCTakeDamage(int damage)
        {
            
            Die();
            
        }

        private void OnHealthChanged()
        {
            Debug.Log($"Player health changed: {Health}");

            // TODO: Add UI Element
        }

        private void Die()
        {
            Debug.Log("Player has died.");
            OnDeath?.Invoke();
        }

        public void SpawnEffect(ParticleSystem ps, Transform transform)
        {
            Instantiate(ps, transform.position, Quaternion.identity);
        }

        public void OnPlayerHitSpawnParticles(PlayerHealthHandler player, ParticleSystem ps, Transform transform)
        {
            if (!player == this)
            {
                SpawnEffect(ps, transform);
            }
        }
        

    }
}
