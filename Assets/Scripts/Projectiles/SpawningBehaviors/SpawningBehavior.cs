using Fusion;
using UnityEngine;
using UnityEngine.Events;

namespace Projectiles.SpawningBehaviors
{
    public abstract class SpawningBehavior : NetworkBehaviour
    {
        public event UnityAction<Projectile> OnProjectileSpawned;
        public abstract void StartSpawning();

        protected void InvokeProjectileSpawned(Projectile proj) => OnProjectileSpawned?.Invoke(proj);
        #if UNITY_EDITOR
        //context menu testing
        [ContextMenu("Test behavior")]
        public void TestSpawning()
        {
            StartSpawning();
        }
        
        #endif
    }
}