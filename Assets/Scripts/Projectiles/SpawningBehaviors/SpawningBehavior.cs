using Fusion;
using UnityEngine;
using UnityEngine.Events;

namespace Projectiles.SpawningBehaviors
{
    public abstract class SpawningBehavior : NetworkBehaviour
    {
        public event UnityAction<Projectile> OnProjectileSpawned;
        public abstract void StartSpawning(float Duration);

        protected void InvokeProjectileSpawned(Projectile proj) => OnProjectileSpawned?.Invoke(proj);
        #if UNITY_EDITOR
        //context menu testing
        [ContextMenu("Test behavior")]
        public void TestSpawning()
        {
            StartSpawning(7);
        }

        public virtual void StopSpawning()
        {
            //for specific behaviors that need extra logic when despawning all
        }
        #endif
    }
}