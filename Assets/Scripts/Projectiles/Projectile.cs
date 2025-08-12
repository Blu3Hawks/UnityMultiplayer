using System;
using System.Collections;
using Fusion;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Projectiles
{
    public class Projectile : NetworkBehaviour
    {
        [SerializeField] private ProjectileData projectileData;
        [SerializeField] private ParticleSystem _particleSystem;
        
        public event UnityAction<Projectile> OnProjectileDespawned;

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
                        StartCoroutine(DespawnProjectile());
                    }
                    //spawn the particle system
                    playerHealthHandler.SpawnEffect(_particleSystem, transform);
                    //first - there will be an event that will be called
                    OnProjectileSpawned?.Invoke(playerHealthHandler, _particleSystem, transform);
                }


            }
        }
        private IEnumerator DespawnProjectile()
        {
            yield return new WaitForSeconds(0.1f);
            Runner.Despawn(Object);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            OnProjectileDespawned?.Invoke(this);
        }
    }
}