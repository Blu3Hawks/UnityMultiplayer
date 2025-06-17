using Fusion;
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
    private readonly int moveSpeedHash = Animator.StringToHash("Movement Speed");


    //values of the input
    private Vector2 _playerInput;
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

            PlayerRotation();
            PlayerMovement();
            ApplyGravity();

        }
    }

    private void PlayerRotation()
    {
        //if no changes in inputs then we don't need to keep going and change the rotation
        if (_playerInput.sqrMagnitude == 0) { return; }
        //calculate the degree of the angle that we want to look at
        float angleToRotate = Mathf.Atan2(_playerDirection.x, _playerDirection.z) * Mathf.Rad2Deg;
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

    private void PlayerMovement()
    {
        _characterController.Move(_playerDirection * _moveSpeed * Runner.DeltaTime);
        //animator.SetFloat(moveSpeedHash, _characterController.velocity.magnitude); //unable until we will add some stuff, like TRUE ANIMATIONS
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


    public void OnPlayerMovement(InputAction.CallbackContext context)
    {
        _playerInput = context.ReadValue<Vector2>();
        _playerDirection = new Vector3(_playerInput.x, 0, _playerInput.y);
    }
}
