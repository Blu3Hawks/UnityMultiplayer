using UnityEngine;

namespace Projectiles
{
	[CreateAssetMenu (menuName = "Projectiles/ProjectileData")]
    public class ProjectileData : ScriptableObject
    {
	    [SerializeField] private float speed;
	    
	    [SerializeField] private float damage;

	    [SerializeField] private float lifetime;
	    
	    public float Speed => speed;
	    public float Damage => damage;
	    public float Lifetime => lifetime;
	    
    }
}