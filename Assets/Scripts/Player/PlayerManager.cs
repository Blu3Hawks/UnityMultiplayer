using System;
using Fusion;
using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerManager : NetworkBehaviour
{
    //idk, just in case we might want to shift these to a manager
    [SerializeField] private PlayerHealthHandler _healthHandler;
    [SerializeField] private PlayerMovementHandler _playerMovementHandler;
    [SerializeField] private NetworkTransform _networkTransform;
    [SerializeField] private MeshRenderer meshRenderer;

    [SerializeField] private PlayerInput input;

    

    public static readonly string PLAYER_TAG = "Player";
    
    public event UnityAction<PlayerManager> OnPlayerDeath;

    public override void Spawned()
    {
        input.enabled = HasInputAuthority;
        if(HasStateAuthority)
            _healthHandler.OnDeath += PlayerDeath;
    }

    public void ToggleControls(bool value)
    {
        _playerMovementHandler.ToggleControls(value);
    }

    public void TeleportToPos(Vector3 pos)
    {
        _networkTransform.Teleport(pos);
    }

    private void PlayerDeath()
    {
        OnPlayerDeath?.Invoke(this);
    }

    public void ResetPlayer()
    {
        
    }

    private void OnValidate()
    {
        #if UNITY_EDITOR
        _networkTransform = GetComponent<NetworkTransform>();
#endif
    }
}
