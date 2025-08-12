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
        [SerializeField] private CharacterSelectionManager characterSelectionManager;

        private List<Projectile> activeProjectiles = new List<Projectile>();
        
        public override void Spawned()
        {
            base.Spawned();
            if (Runner.IsServer)
            {
                characterSelectionManager.OnAllPlayersSelected += SpawnProjectiles;
            }
        }

        private async void SpawnProjectiles()
        {
            if(Runner.IsServer) characterSelectionManager.OnAllPlayersSelected -= SpawnProjectiles;
            while (true)
            {
                Transform chosen = spawnPoints[Random.Range(0, spawnPoints.Count)];
                NetworkObject spawnedObject = await Runner.SpawnAsync(projectilePrefab, chosen.transform.position);
                Projectile proj = spawnedObject.GetComponent<Projectile>();
                activeProjectiles.Add(proj);
                proj.OnProjectileDespawned += RemoveFromActive;
                proj.SetDirection(chosen.forward);
                await Task.Delay(1000);
            }
        }
        
        private void RemoveFromActive(Projectile obj)
        {
            obj.OnProjectileDespawned -= RemoveFromActive;
            activeProjectiles.Remove(obj);
        }

        public void DespawnAll()
        {
            for(int i = activeProjectiles.Count -1 ; i > 0; i--)
            {
                Runner.Despawn(activeProjectiles[i].Object);
            }
        }
    }
}