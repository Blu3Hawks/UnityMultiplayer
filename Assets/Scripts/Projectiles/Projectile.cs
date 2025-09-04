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
        
        private bool _shouldRotate = false;
        public event UnityAction<Projectile> OnProjectileDespawned;

        private Vector3 _direction;

        private float _lifeTime = 0;

        private bool _hit = false;

        public static event UnityAction<Vector3> OnProjectileHit;


        public override void Spawned()
        {
            base.Spawned();
            _lifeTime = projectileData.Lifetime;
        }

        public void SetDirection(Vector3 direction)
        {
            this._direction = direction;
        }
        
        public void SetShouldRotate(bool shouldRotate)
        {
            this._shouldRotate = shouldRotate;
        }


        public override void FixedUpdateNetwork()
        {
            base.FixedUpdateNetwork();
            if (HasStateAuthority)
            {
                this.transform.position += this._direction * projectileData.Speed * Runner.DeltaTime;
                if(_shouldRotate) this.transform.Rotate(Vector3.up * 360f * Runner.DeltaTime);
                _lifeTime -= Runner.DeltaTime;
                if (_lifeTime <= 0)
                    Runner.Despawn(Object);
            }

        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (_hit && HasStateAuthority) return;//prevents hitting 2 players
            if (other.CompareTag(PlayerManager.PLAYER_TAG))
            {
                PlayerHealthHandler playerHealthHandler = other.GetComponent<PlayerHealthHandler>();
                if (playerHealthHandler != null)
                {
                    if (HasStateAuthority)
                    {
                        _hit = true;
                        RpcInvokeOnHit(playerHealthHandler.transform.position);
                        playerHealthHandler.RPCTakeDamage(10);
                        
                    }
                    //spawn the particle system
                    //first - there will be an event that will be called
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
            StopAllCoroutines();
            OnProjectileDespawned?.Invoke(this);
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
        public void RpcInvokeOnHit(Vector3 playerPosition)
        {
            OnProjectileHit?.Invoke(playerPosition);
            StartCoroutine(DespawnProjectile());

        }
    }
}