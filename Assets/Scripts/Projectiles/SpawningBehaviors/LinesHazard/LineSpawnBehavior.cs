using System.Collections.Generic;
using UnityEngine;

namespace Projectiles.SpawningBehaviors.LinesHazard
{
    public class LineSpawnBehavior : SpawningBehavior
    {
        [SerializeField] private List<Transform> spawnPoints;
        [SerializeField] private ProjectileWithoutDespawn linePrefab;
        public override void StartSpawning()
        {
            foreach (Transform spawnpoint in spawnPoints)
            {
                ProjectileWithoutDespawn current = Runner.Spawn(linePrefab, spawnpoint.position, spawnpoint.rotation);
                current.SetDirection(spawnpoint.forward);
            }
        }
    }
}