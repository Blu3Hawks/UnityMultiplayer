using Fusion;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : NetworkBehaviour
{
    //idk, just in case we might want to shift these to a manager
    [SerializeField] private PlayerHealthHandler _healthHandler;
    [SerializeField] private PlayerMovementHandler _playerMovementHandler;

    [SerializeField] private MeshRenderer meshRenderer;

    [SerializeField] private PlayerInput input;


    public static readonly string PLAYER_TAG = "Player";

    public override void Spawned()
    {
        input.enabled = HasStateAuthority;
    }


}
