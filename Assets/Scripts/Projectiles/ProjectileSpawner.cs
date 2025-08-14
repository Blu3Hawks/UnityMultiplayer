using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Projectiles.SpawningBehaviors;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileSpawner : NetworkBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private CharacterSelectionManager characterSelectionManager;

        [SerializeField] private List<SpawningBehavior> behaviors;
        private List<Projectile> activeProjectiles = new List<Projectile>();

        private bool shouldSpawn = false;

        public override void Spawned()
        {
            base.Spawned();
            foreach(SpawningBehavior behavior in behaviors)
            {
                behavior.OnProjectileSpawned += HandleSpawnedProjectile;
            }
            
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);
            foreach(SpawningBehavior behavior in behaviors)
            {
                behavior.OnProjectileSpawned -= HandleSpawnedProjectile;
            }
        }

        private void HandleSpawnedProjectile(Projectile obj)
        {
            obj.OnProjectileDespawned += RemoveFromActive;
            activeProjectiles.Add(obj);
        }

        public void SpawnProjectiles()
        {
            if (!Runner.IsServer && !HasStateAuthority) return;
            StartCoroutine(SpawnCoroutine());
        }

        private IEnumerator SpawnCoroutine()
        {
            while (true)
            {
                behaviors[Random.Range(0, behaviors.Count)].StartSpawning();
                yield return new WaitForSeconds(7f);
            }
        }
        
        private void RemoveFromActive(Projectile obj)
        {
            obj.OnProjectileDespawned -= RemoveFromActive;
            activeProjectiles.Remove(obj);
        }

        public void StopSpawning()
        {
            StopCoroutine(SpawnCoroutine());
        }

        public void DespawnAll()
        {
            for(int i = activeProjectiles.Count -1 ; i >= 0; i--)
            {
                Runner.Despawn(activeProjectiles[i].Object);
            }
        }
    }
}