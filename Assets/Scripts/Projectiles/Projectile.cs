using System;
using Fusion;
using Player;
using UnityEngine;

namespace Projectiles
{
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private ProjectileData projectileData;

        private Vector3 _direction;

        private float _lifeTime = 0;

        
        public override void Spawned()
        {
            base.Spawned();
            _lifeTime = projectileData.Lifetime;
        }

        public void SetDirection(Vector3 direction)
        {
            this._direction = direction;
        }


        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            if (HasStateAuthority)
            {
                this.transform.position += this._direction * projectileData.Speed * Runner.DeltaTime;
            
                _lifeTime -= Runner.DeltaTime;
                if(_lifeTime <= 0)
                    Runner.Despawn(Object);
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PlayerManager.PLAYER_TAG))
            {
                PlayerHealthHandler playerHealthHandler = other.GetComponent<PlayerHealthHandler>();
                if (playerHealthHandler != null && HasStateAuthority)
                {
                    playerHealthHandler.RPCTakeDamage(10);
                    Debug.Log("Player hit");
                    Runner.Despawn(Object);
                }
                
                
            }
        }
    }
}