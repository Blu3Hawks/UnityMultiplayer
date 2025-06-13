using Fusion;
using UnityEngine;

public class PlayerHealthHandler : NetworkBehaviour
{
    [SerializeField] private PlayerManager _playerManager;

    [Header("Player Settings")]
    [Networked, OnChangedRender(nameof(OnHealthChanged))]
    [field: SerializeField]
    public int Health { get; set; }


    public void TakeDamage(int damage)
    {
        if (Health <= 0) return;

        Health -= damage;

    }

    private void OnHealthChanged()
    {
        Debug.Log($"Player health changed: {Health}");
        if (Health <= 0 && HasStateAuthority)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        Runner.Despawn(Object);
    }
}
