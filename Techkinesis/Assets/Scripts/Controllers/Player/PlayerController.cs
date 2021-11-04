using UnityEngine;

// Controls the player

[RequireComponent(typeof(ShieldAbility))]
[RequireComponent(typeof(LaunchAbility))]
[RequireComponent(typeof(LevitationAbility))]
public class PlayerController : MovementController, IProjectileInteraction
{
    #region Variables editable in the inspector (for a designer)

    [Header("References")]
    [Tooltip("The players InputProvider")]
    [SerializeField] PlayerInput inputProvider;

    [Tooltip("The players camera")]
    [SerializeField] ThirdPersonCamera cam;

    [Header("Movement")]
    [Tooltip("How long it will take for the player to go between stopping / walking / running")]
    [SerializeField] float speedSmoothTime = 0.1f;

    [Tooltip("How long it will take for the player to turn")]
    [SerializeField] float turnSmoothTime = 0.2f;

    [Tooltip("How fast the player will walk")]
    [SerializeField] float walkSpeed = 2;

    [Tooltip("How fast the player will run")]
    [SerializeField] float runSpeed = 6;

    [Tooltip("How high the player will jump")]
    [SerializeField] float jumpHeight = 1;

    [Tooltip("How much control the player has while jumping. 1 = more control, 0 = less control")]
    [SerializeField, Range(0, 1)] float airControlPercent = 0.5f;

    #endregion

    ShieldAbility shieldAbility;
    LaunchAbility launchAbility;
    LevitationAbility levitationAbility;

    float speedSmoothVelocity;
    float currentSpeed;
    float velocityY;
    float turnSmoothVelocity;

    public enum MovementState { GROUND, LEVITATION, DEAD, NONE }    // The players movement states. Dictates what rotation/movement/animation methods are called
    MovementState movementState;

    // Setup the PlayerController
    public override void Start()
    {
        // CharacterController and Animator references grabbed here
        base.Start();  

        // Grab the ability components and set them up 
        shieldAbility = GetComponent<ShieldAbility>();                          shieldAbility.PassReferences(controller);
        launchAbility = GetComponent<LaunchAbility>();                          launchAbility.PassReferences(cam, shieldAbility);
        levitationAbility = GetComponent<LevitationAbility>();                  levitationAbility.PassReferences(cam, this, animator);

        // Hook up the events
        inputProvider.OnJump += Jump;
        inputProvider.OnSwitchCameraSide += cam.GetComponent<ThirdPersonCamera>().SwitchCameraSide;

        inputProvider.OnLaunchStart += launchAbility.LaunchStart;               inputProvider.OnLaunchEnd += launchAbility.LaunchEnd;
        inputProvider.OnShieldStart += shieldAbility.ShieldStart;               inputProvider.OnShieldEnd += shieldAbility.ShieldEnd;
        inputProvider.OnLevitationStart += levitationAbility.LevitationStart;   inputProvider.OnLevitationEnd += levitationAbility.LevitationEnd;

        shieldAbility.TakeDamage += TakeDamage;

        // Set the first movement state
        movementState = MovementState.GROUND;
    }

    // Grab the current PlayerInputState and pass its values to the relevant methods depending on the movementState
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
            case MovementState.DEAD:
                // Similar to NONE, but in SetMovementState there is a check to make sure that the player can't transition out of this state
                break;
            default:
                Debug.LogError("MovementState of type " + movementState.ToString() + " does not exist!");
                break;
        }
    }

    // Rotates the player
    void HandleGroundRotation(InputState inputState)
    {
        // Is the player isn't providing any input, don't rotate the character so they can look around

        if (inputState.movementDirection != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputState.movementDirection.x, inputState.movementDirection.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }
    }

    // Move the player. Handles x/z and y explicitly (gravity) because we are using a CharacterController
    void HandleGroundMovement(InputState inputState)
    {
        float targetSpeed = (inputState.running ? runSpeed : walkSpeed) * inputState.movementDirection.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        velocityY += mass * Time.deltaTime;
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded) velocityY = 0f;
    }

    // Smoothly interpolates between the animation walk / run states based on the CharacterControllers speed
    void HandleGroundAnimation(InputState inputState)
    {
        float currentControllerSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
        float animationSpeedPercent = inputState.running ? currentControllerSpeed / runSpeed : currentControllerSpeed / walkSpeed * 0.5f;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }

    // Adds y velocity when the player jumps
    void Jump()
    {
        if (!controller.isGrounded) return;

        float jumpVelocity = Mathf.Sqrt(-2 * mass * jumpHeight);
        velocityY = jumpVelocity;
    }

    // Calculate the smoothTime (used for calculating the players rotation while they are in the air) based on if the are grounded or not
    float GetModifiedSmoothTime(float smoothTime)
    {
        if (controller.isGrounded)  return smoothTime;
        if (airControlPercent == 0) return float.MaxValue;  // Prevents any divide by 0 errors

        return smoothTime / airControlPercent;
    }

    // Set the players movementState
    public void SetMovementState(MovementState _movementState)
    {
        if (movementState == MovementState.DEAD) 
        {
            Debug.LogError("Something tried to set the player's MovementState when they are already dead! " +
                           "The requested state was " + _movementState.ToString() + ".");
            return;
        }

        movementState = _movementState;
    }

    // Reduce the player's hp. If the player's hp is less than 0, set MovementState to Dead (which cuts off player input) and trigger the death effect
    public override void TakeDamage(int damage)
    {
        print(damage);

        if (healthModule.Damage(damage))
        {
            SetMovementState(MovementState.DEAD);
            GetComponent<DeathEffectModule>().HumanoidDeath(animator);

            DebugLogManager.Print("Some cool player death logic would go here.", DebugLogManager.OutputType.NOT_MY_JOB);
        }
    }
}