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

        private Coroutine currentCoroutine;

        private float minDuration = 5f;
        private float maxDuration = 7f;
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
            currentCoroutine = StartCoroutine(SpawnCoroutine());
        }

        private IEnumerator SpawnCoroutine()
        {
            yield return new WaitForSeconds(5f);
            while (true)
            {
                Debug.Log("Spawning");
                float RandomDuration = Random.Range(minDuration, maxDuration);
                behaviors[Random.Range(0, behaviors.Count)].StartSpawning(RandomDuration);
                yield return new WaitForSeconds(RandomDuration);
            }
        }
        
        private void RemoveFromActive(Projectile obj)
        {
            obj.OnProjectileDespawned -= RemoveFromActive;
            activeProjectiles.Remove(obj);
        }

        public void StopSpawning()
        {
            StopCoroutine(currentCoroutine);
        }

        public void DespawnAll()
        {
            for(int i = activeProjectiles.Count -1 ; i >= 0; i--)
            {
                Runner.Despawn(activeProjectiles[i].Object);
            }
            foreach(SpawningBehavior behavior in behaviors)
            {
                behavior.StopSpawning();
            }
        }
    }
}