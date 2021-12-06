using UnityEngine;

// Controls the player

[RequireComponent(typeof(ShieldAbility))]
[RequireComponent(typeof(LaunchAbility))]
[RequireComponent(typeof(LevitationAbility))]
public class PlayerController : MovementController<PlayerData>, IProjectileInteraction
{
    [Space]
    [Tooltip("The players camera")]
    [SerializeField] ThirdPersonCamera cam;

    ShieldAbility shieldAbility;
    LaunchAbility launchAbility;
    LevitationAbility levitationAbility;

    PlayerUIManger playerUIManger;

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
        shieldAbility = GetComponent<ShieldAbility>();           shieldAbility.PassReferences(controller);                shieldAbility.Setup(energyModule);
        launchAbility = GetComponent<LaunchAbility>();           launchAbility.PassReferences(cam, shieldAbility);        launchAbility.Setup(energyModule);
        levitationAbility = GetComponent<LevitationAbility>();   levitationAbility.PassReferences(cam, this, animator);   levitationAbility.Setup(energyModule);

        // Hook up the events
        controllerData.inputProvider.OnJump += Jump;
        controllerData.inputProvider.OnSwitchCameraSide += cam.GetComponent<ThirdPersonCamera>().SwitchCameraSide;

        controllerData.inputProvider.OnLaunchStart += launchAbility.DoAbilityStart;           controllerData.inputProvider.OnLaunchEnd += launchAbility.DoAbilityEnd;
        controllerData.inputProvider.OnShieldStart += shieldAbility.DoAbilityStart;           controllerData.inputProvider.OnShieldEnd += shieldAbility.DoAbilityEnd;
        controllerData.inputProvider.OnLevitationStart += levitationAbility.DoAbilityStart;   controllerData.inputProvider.OnLevitationEnd += levitationAbility.DoAbilityEnd;

        shieldAbility.TakeDamage += TakeDamage;

        // Setup UI
        playerUIManger = GetComponent<PlayerUIManger>();
        playerUIManger.Setup(healthModule, energyModule);

        // Set the first movement state
        movementState = MovementState.GROUND;
    }

    // Grab the current PlayerInputState and pass its values to the relevant methods depending on the movementState
    void Update()
    {
        PlayerInputState inputState;
        
        switch (movementState)
        {
            case MovementState.GROUND:
                inputState = controllerData.inputProvider.GetState();

                HandleGroundRotation(inputState);
                HandleGroundMovement(inputState);
                HandleGroundAnimation(inputState);
                break;
            case MovementState.LEVITATION:
                inputState = controllerData.inputProvider.GetState();

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
            float targetYRotation = Mathf.Atan2(inputState.movementDirection.x, inputState.movementDirection.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetYRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(controllerData.turnSmoothTime));
        }
    }

    // Move the player. Handles x/z and y explicitly (gravity) because we are using a CharacterController
    void HandleGroundMovement(InputState inputState)
    {
        float targetSpeed = (inputState.running ? controllerData.runSpeed : controllerData.walkSpeed) * inputState.movementDirection.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(controllerData.speedSmoothTime));

        velocityY += controllerData.mass * Time.deltaTime;
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

        controller.Move(velocity * Time.deltaTime);

        if (controller.isGrounded) velocityY = 0f;
    }

    // Smoothly interpolates between the animation walk / run states based on the CharacterControllers speed
    void HandleGroundAnimation(InputState inputState)
    {
        float currentControllerSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
        float animationSpeedPercent = inputState.running ? currentControllerSpeed / controllerData.runSpeed : currentControllerSpeed / controllerData.walkSpeed * 0.5f;
        animator.SetFloat("speedPercent", animationSpeedPercent, controllerData.speedSmoothTime, Time.deltaTime);
    }

    // Adds y velocity when the player jumps
    void Jump()
    {
        if (!controller.isGrounded) return;

        float jumpVelocity = Mathf.Sqrt(-2 * controllerData.mass * controllerData.jumpHeight);
        velocityY = jumpVelocity;
    }

    // Calculate the smoothTime (used for calculating the players rotation while they are in the air) based on if the are grounded or not
    float GetModifiedSmoothTime(float smoothTime)
    {
        if (controller.isGrounded)  return smoothTime;
        if (controllerData.airControlPercent == 0) return float.MaxValue;  // Prevents any divide by 0 errors

        return smoothTime / controllerData.airControlPercent;
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
        if (healthModule.Damage(damage))
        {
            SetMovementState(MovementState.DEAD);
            GetComponent<DeathEffectModule>().HumanoidDeath(animator);

            DebugLogManager.Print("Some cool player death logic would go here.", DebugLogManager.OutputType.NOT_MY_JOB);
        }
    }
}