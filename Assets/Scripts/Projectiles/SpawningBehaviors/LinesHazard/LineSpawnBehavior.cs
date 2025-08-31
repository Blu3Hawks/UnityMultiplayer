using System.Collections.Generic;
using UnityEngine;

namespace Projectiles.SpawningBehaviors.LinesHazard
{
    public class LineSpawnBehavior : SpawningBehavior
    {
        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private ProjectileWithoutDespawn linePrefab;
        [SerializeField] private bool ShouldRotate = false;
        public override void StartSpawning(float Duration)
        {
            foreach (Transform spawnpoint in spawnPoints)
            {
                ProjectileWithoutDespawn current = Runner.Spawn(linePrefab, spawnpoint.position, spawnpoint.rotation);
                current.SetDirection(spawnpoint.forward);
                if(ShouldRotate) current.SetShouldRotate(true);
                InvokeProjectileSpawned(current);
            }
        }
    }
}