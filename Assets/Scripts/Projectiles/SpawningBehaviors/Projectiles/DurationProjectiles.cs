using System.Collections;
using Projectiles.SpawningBehaviors.LinesHazard;
using UnityEngine;

namespace Projectiles.SpawningBehaviors.Projectiles
{
    public class DurationProjectiles : SpawningBehavior
    {
        [SerializeField] Projectile projectile;

        [SerializeField] private float radius = 7;

        [SerializeField] private float projectilesOverDuration = 10;
        public override void StartSpawning(float duration)
        {
            StartCoroutine(SpawningCoroutine(duration));
        }

        private IEnumerator SpawningCoroutine(float duration)
        {
            for (int i = 0; i < projectilesOverDuration; i++)
            {
                Vector2 random = Random.insideUnitCircle.normalized * Random.Range(radius-2, radius+2);
                Projectile current = Runner.Spawn(projectile, new Vector3(random.x,transform.position.y,random.y), Quaternion.identity);
                Vector3 direction = transform.position - current.transform.position;
                current.SetDirection(direction.normalized);
                current.SetShouldRotate(true);
                InvokeProjectileSpawned(current);
                yield return new WaitForSeconds(duration / projectilesOverDuration);
            }
        }

        public override void StopSpawning()
        {
            base.StopSpawning();
            StopAllCoroutines();
        }
    }
}