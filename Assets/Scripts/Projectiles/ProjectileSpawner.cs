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
                Transform chosen = spawnPoints[Random.Range(0, spawnPoints.Count)];
                NetworkObject spawnedObject = await Runner.SpawnAsync(projectilePrefab, chosen.transform.position);
                Projectile proj = spawnedObject.GetComponent<Projectile>();
                
                proj.SetDirection(chosen.forward);
                await Task.Delay(1000);
            }
        }
    }
}