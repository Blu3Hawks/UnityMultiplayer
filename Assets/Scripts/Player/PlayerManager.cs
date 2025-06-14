using Fusion;
using UnityEngine;

public class PlayerManager : NetworkBehaviour
{
    //idk, just in case we might want to shift these to a manager
    [SerializeField] private PlayerHealthHandler _healthHandler;
    [SerializeField] private PlayerMovementHandler _playerMovementHandler;

    [SerializeField] private MeshRenderer meshRenderer;


    

    public override void Spawned()
    {
    }


}
