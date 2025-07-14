using System;
using Fusion;
using Player;
using UnityEngine;

namespace Projectiles
{
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private ProjectileData projectileData;
        [SerializeField] private ParticleSystem _particleSystem;


        private Vector3 _direction;

        private float _lifeTime = 0;

        public static event Action<PlayerHealthHandler, ParticleSystem, Transform> OnProjectileSpawned;


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
                if (_lifeTime <= 0)
                    Runner.Despawn(Object);
            }

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(PlayerManager.PLAYER_TAG))
            {
                PlayerHealthHandler playerHealthHandler = other.GetComponent<PlayerHealthHandler>();
                if (playerHealthHandler != null)
                {
                    if (HasStateAuthority)
                    {
                        playerHealthHandler.RPCTakeDamage(10);
                        Debug.Log("Player hit");
                        Runner.Despawn(Object);
                    }
                    //spawn the particle system
                    playerHealthHandler.SpawnEffect(_particleSystem, transform);
                    //first - there will be an event that will be called
                    OnProjectileSpawned?.Invoke(playerHealthHandler, _particleSystem, transform);
                }


            }
        }
    }
}