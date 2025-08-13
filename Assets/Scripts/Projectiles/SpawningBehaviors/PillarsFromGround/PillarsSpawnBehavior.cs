using Projectiles.SpawningBehaviors.LinesHazard;
using UnityEngine;

namespace Projectiles.SpawningBehaviors.PillarsFromGround
{
    public class PillarsSpawnBehavior : SpawningBehavior
    {
        [SerializeField] ProjectileWithoutDespawn pillarPrefab;

        [SerializeField] private Transform minBound;
        [SerializeField] private Transform maxBound;
        [SerializeField] private int pillarCount;
        public override void StartSpawning()
        {
            for (int i = 0; i < pillarCount; i++)
            {
                float randomX = Random.Range(minBound.position.x, maxBound.position.x);
                float randomZ = Random.Range(minBound.position.z, maxBound.position.z);
                ProjectileWithoutDespawn current = Runner.Spawn(pillarPrefab, new Vector3(randomX,0, randomZ), Quaternion.identity);
                current.SetDirection(Vector3.zero);
                InvokeProjectileSpawned(current);
            }
        }
    }
}