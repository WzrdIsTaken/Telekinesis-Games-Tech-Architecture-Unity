using UnityEngine;

// Controls the player

public class PlayerController : MovementController
{
    [Header("References")]
    [SerializeField] PlayerInput inputProvider;
    [SerializeField] Camera cam;

    [Header("Attributes")]
    [SerializeField] float mass = -12f;
    [SerializeField, Range(0, 1)] float airControlPercent = 0.5f;  // 1 = more control, 0 = less control

    [Header("Movement")]
    [SerializeField] float speedSmoothTime = 0.1f;
    [SerializeField] float turnSmoothTime = 0.2f;

    [SerializeField] float walkSpeed = 2;
    [SerializeField] float runSpeed = 6;
    [SerializeField] float jumpHeight = 1;

    LaunchAbility launchAbility;
    ShieldAbility shieldAbility;
    LevitationAbility levitationAbility;

    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;
    float turnSmoothVelocity;

    public enum MovementState { GROUND, LEVITATION, NONE }
    MovementState movementState;

    public override void Start()
    {
        base.Start();  // CharacterController and Animator references grabbed here 

        launchAbility = GetComponent<LaunchAbility>();                          launchAbility.PassReferences(cam);
        shieldAbility = GetComponent<ShieldAbility>();                          // Some cool reference here
        levitationAbility = GetComponent<LevitationAbility>();                  levitationAbility.PassReferences(cam, this, animator);

        inputProvider.OnJump += Jump;
        inputProvider.OnSwitchCameraSide += cam.GetComponent<ThirdPersonCamera>().SwitchCameraSide;

        inputProvider.OnLaunchStart += launchAbility.LaunchStart;               inputProvider.OnLaunchEnd += launchAbility.LaunchEnd;
        inputProvider.OnShieldStart += shieldAbility.ShieldStart;               inputProvider.OnShieldEnd += shieldAbility.ShieldEnd;
        inputProvider.OnLevitationStart += levitationAbility.LevitationStart;   inputProvider.OnLevitationEnd += levitationAbility.LevitationEnd;
        // For future reference, can do event += () => thing. Here I don't think its clear + we need the SetMovementState anyway, but its cool.
        
        movementState = MovementState.GROUND;
    }

    void Update()
    {
        PlayerInputState inputState = inputProvider.GetState();
        
        switch (movementState)
        {
            case MovementState.GROUND:
                HandleGroundRotation(inputState);
                HandleGroundMovement(inputState);
                HandleGroundAnimation(inputState);
                break;
            case MovementState.LEVITATION:
                levitationAbility.HandleLevitationRotation(inputState);
                levitationAbility.HandleLevitationMovement(inputState);
                levitationAbility.HandleLevitationAnimation(inputState);
                break;
            case MovementState.NONE:
                // Could be used for a cutscene or something, where we don't want the player to be able to move
                break;
        }
    }

    void HandleGroundRotation(InputState inputState)
    {
        if (inputState.movementDirection != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputState.movementDirection.x, inputState.movementDirection.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }
    }

    void HandleGroundMovement(InputState inputState)
    {
        float targetSpeed = (inputState.running ? runSpeed : walkSpeed) * inputState.movementDirection.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        velocityY += mass * Time.deltaTime;
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded) velocityY = 0f;
    }

    void HandleGroundAnimation(InputState inputState)
    {
        float currentControllerSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
        float animationSpeedPercent = inputState.running ? currentControllerSpeed / runSpeed : currentControllerSpeed / walkSpeed * 0.5f;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }

    void Jump()
    {
        if (!controller.isGrounded) return;

        float jumpVelocity = Mathf.Sqrt(-2 * mass * jumpHeight);
        velocityY = jumpVelocity;
    }

    float GetModifiedSmoothTime(float smoothTime)
    {
        if (controller.isGrounded) return smoothTime;
        if (airControlPercent == 0) return float.MaxValue;

        return smoothTime / airControlPercent;
    }

    public void SetMovementState(MovementState _movementState)
    {
        movementState = _movementState;
    }
}