using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;

namespace Projectiles
{
    public class ProjectileSpawner : NetworkBehaviour
    {
        [SerializeField] private Projectile projectilePrefab;
        [SerializeField] private List<Transform> spawnPoints;

        public override void Spawned()
        {
            base.Spawned();
            if(HasStateAuthority)
                SpawnProjectiles();
        }

        private async void SpawnProjectiles()
        {
            while (true)
            {
                NetworkObject spawnedObject = await Runner.SpawnAsync(projectilePrefab);
                Projectile proj = spawnedObject.GetComponent<Projectile>();
                Transform chosen = spawnPoints[Random.Range(0, spawnPoints.Count)];
                
                proj.transform.position = chosen.position;
                proj.SetDirection(chosen.forward);
                await Task.Delay(1000);
            }
        }
    }
}