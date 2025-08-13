using Fusion;
using UnityEngine;

namespace Projectiles.SpawningBehaviors
{
    public abstract class SpawningBehavior : NetworkBehaviour
    {
        
        public abstract void StartSpawning();
        
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