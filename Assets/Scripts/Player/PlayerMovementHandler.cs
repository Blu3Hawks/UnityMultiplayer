using Fusion;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementHandler : NetworkBehaviour
{


    [Header("Movement Settings")]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private GameObject _playerModel;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationSpeed;

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
        if (Object.HasStateAuthority)
        {
            if (GetInput(out PlayerInputData data))
            {
                PlayerRotation(data);
                PlayerMovement(data);
            }
            // ApplyGravity();

        }
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
        _characterController.Move(data.Movementvector * _moveSpeed * Runner.DeltaTime);
        if (data.Movementvector.sqrMagnitude < 0.01f)
        {
            //if the player is not moving, then we don't need to change the animator
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isRunning", true);
        }

        if (Input.GetKey(KeyCode.G))
        {
            //for testing purposes, we can wave
            StartCoroutine(WavingAnimationCooldown());
        }
    }

    private IEnumerator WavingAnimationCooldown()
    {
        animator.SetBool(_isWaving, true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool(_isWaving, false);
    }
    private void ApplyGravity()
    {
        //if we are on the ground, and somehow if the player's velocity is less than 0 by any other... stuff
        if (_characterController.isGrounded && _playerGravitationalVelocity < 0f)
        {
            _playerGravitationalVelocity = 0f;
        }
        else
        {
            _playerGravitationalVelocity += _gravityValue * _gravityMultiplier * Runner.DeltaTime;
        }
        //apply the velocity properly
        _playerDirection.y = _playerGravitationalVelocity;
    }


    
}
