using Fusion;
using UnityEngine;

namespace Player {
    public class PlayerHealthHandler : NetworkBehaviour {
        [SerializeField] private PlayerManager playerManager;

        [Header("Player Settings")]
        [Networked, OnChangedRender(nameof(OnHealthChanged))] [field: SerializeField]
        public int Health { get; set; }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPCTakeDamage(int damage) {
            Health -= damage;

            if (Health <= 0) {
                Health = 0;
                Die();
            }
        }

        private void OnHealthChanged() {
            Debug.Log($"Player health changed: {Health}");
            
            // TODO: Add UI Element
        }

        private void Die() {
            Debug.Log("Player has died.");
            Runner.Despawn(Object);
        }
    }
}
