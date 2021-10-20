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

    public override void Start()
    {
        base.Start();  // CharacterController and Animator references grabbed here 

        launchAbility = GetComponent<LaunchAbility>();
        launchAbility.AssignCamera(cam);
        shieldAbility = GetComponent<ShieldAbility>();
        levitationAbility = GetComponent<LevitationAbility>();

        inputProvider.OnJump += Jump;

        inputProvider.OnLaunchStart += launchAbility.LaunchStart;               inputProvider.OnLaunchEnd += launchAbility.LaunchEnd;
        inputProvider.OnShieldStart += shieldAbility.ShieldStart;               inputProvider.OnShieldEnd += shieldAbility.ShieldEnd;
        inputProvider.OnLevitationStart += levitationAbility.LevitationStart;   inputProvider.OnLevitationEnd += levitationAbility.LevitationEnd;

        inputProvider.OnSwitchCameraSide += cam.GetComponent<ThirdPersonCamera>().SwitchCameraSide;
    }

    void Update()
    {
        InputState inputState = inputProvider.GetState();

        HandleRotation(inputState);
        HandleMovement(inputState);
        HandleAnimation(inputState);
    }

    void HandleRotation(InputState inputState)
    {
        if (inputState.movementDirection != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputState.movementDirection.x, inputState.movementDirection.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }
    }

    void HandleMovement(InputState inputState)
    {
        float targetSpeed = (inputState.running ? runSpeed : walkSpeed) * inputState.movementDirection.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        velocityY += mass * Time.deltaTime;
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded) velocityY = 0f;
    }

    void HandleAnimation(InputState inputState)
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
}