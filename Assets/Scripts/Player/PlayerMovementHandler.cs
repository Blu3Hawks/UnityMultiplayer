using Fusion;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementHandler : NetworkBehaviour
{
    private static readonly int IsRunning = Animator.StringToHash("isRunning");


    [Header("Movement Settings")]
    [SerializeField] private GameObject _playerModel;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed;
    private const float MovementThreshold = 0.001f;

    [SerializeField] private float mapSize = 5;//Not optimal but just wanted to add before hand in
    [Header("Gravity Settings")]
    [SerializeField] private float _gravityValue = -9.81f;
    [SerializeField] private float _gravityMultiplier = 1f;

    [Header("Animator")]
    [SerializeField] private Animator animator; //will be kept as null for now

    //values of the animator
    private readonly int _isWaving = Animator.StringToHash("isWaving");
    private readonly int _isRunning = Animator.StringToHash("isRunning");

    //values of the input
    //direction of the input
    private Vector3 _playerDirection;
    //our player's gravitational velocity
    private float _playerGravitationalVelocity;
    //player's current rotation velocity
    private float _playerRotationDirection;

    private bool _canMove = true;

    public override void Spawned()
    {
        base.Spawned();
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        HandlePlayerMovement();
    }


    private void HandlePlayerMovement()
    {
        //if has authority then - 
        if (Object.HasStateAuthority || HasInputAuthority && _canMove)
        {
            if (GetInput(out PlayerInputData data))
            {
                PlayerRotation(data);
                PlayerMovement(data);
            }
            // ApplyGravity();

        }
    }

    public void ToggleControls(bool value)
    {
        _canMove = value;
    }

    private void PlayerRotation(PlayerInputData data)
    {
        //if no changes in inputs then we don't need to keep going and change the rotation
        if (data.Movementvector.sqrMagnitude == 0) { return; }
        //calculate the degree of the angle that we want to look at
        float angleToRotate = Mathf.Atan2(data.Movementvector.x, data.Movementvector.z) * Mathf.Rad2Deg;
        //make a smooth transition between the angles - between the current angle and the new inputted angle
        float angle = Mathf.SmoothDampAngle(
            _playerModel.transform.eulerAngles.y,
            angleToRotate,
            ref _playerRotationDirection,
            _rotationSpeed * Runner.DeltaTime
        );

        //calculate the differences between the angles. Since the .Rotate is applying constant change to the angle,
        //it adds every time more to the angle - so we need to calculate, as we walk, the differences between the
        //angles, and apply them. Once the differences are 0 we no longer rotate
        float angleDifferences = Mathf.DeltaAngle(transform.eulerAngles.y, angle);
        //actually translate the rotation
        transform.Rotate(Vector3.up, angleDifferences);
    }

    private void PlayerMovement(PlayerInputData data)
    {
        Vector3 nextPos = transform.position + data.Movementvector * _moveSpeed * Runner.DeltaTime;
        if (nextPos.x > mapSize || nextPos.x < -mapSize || nextPos.z > mapSize || nextPos.z < -mapSize) return;
        transform.position +=  ( data.Movementvector * _moveSpeed * Runner.DeltaTime);
        if (Mathf.Abs(data.Movementvector.sqrMagnitude) < MovementThreshold)
        {
            //if the player is not moving, then we don't need to change the animator
            animator.SetBool(IsRunning, false);
        }
        else
        {
            animator.SetBool(IsRunning, true);
        }

        
    }

    
    private void ApplyGravity()
    {
        
        _playerDirection.y = _playerGravitationalVelocity;
    }


    
}
