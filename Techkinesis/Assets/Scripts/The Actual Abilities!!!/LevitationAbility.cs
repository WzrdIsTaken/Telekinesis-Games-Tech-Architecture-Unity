using UnityEngine;
using System.Collections;

// Allows the player to fly

public class LevitationAbility : AbilityBase<LevitationAbilityData>
{
    ThirdPersonCamera cam;
    PlayerController playerController;
    Animator animator;

    Rigidbody rb;
    CapsuleCollider col;

    Vector3 levitationForce;
    Vector3 driftForce;
    Vector2 tiltVelocity;

    float startCameraFov;

    // Grab the rigidbody and collider components
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        col.enabled = false;
    }

    // Pass LevitationAbility various references that are already 'got' in PlayerController
    public void PassReferences(ThirdPersonCamera _cam, PlayerController _playerController, Animator _animator)
    {
        cam = _cam;
        playerController = _playerController;
        animator = _animator;

        startCameraFov = cam.GetCamera().fieldOfView;
    }

    // Start levitating. Called from an event hooked up in PlayerController Start
    protected override void AbilityStart()
    {
        playerController.SetMovementState(PlayerController.MovementState.LEVITATION);
        col.enabled = true;
        rb.AddForce(abilityData.startBoostForce, ForceMode.Force);
        
        cam.ChangeCameraFov(abilityData.levitatingFov, abilityData.levitatingFovChangeTime);
        StartCoroutine(Levitate());
        StartCoroutine(Drift());

        DebugLogManager.Print("Levitation active! Would make a cool sound or something.", DebugLogManager.OutputType.NOT_MY_JOB);
    }

    // Stop levitating. Called from an event hooked up in PlayerController Start
    protected override void AbilityEnd()
    {
        playerController.SetMovementState(PlayerController.MovementState.GROUND);
        col.enabled = false;

        StopAllCoroutines();

        cam.ChangeCameraFov(startCameraFov, abilityData.levitatingFovChangeTime);

        DebugLogManager.Print("Levitation end! Would make a cool sound or something.", DebugLogManager.OutputType.NOT_MY_JOB);
    }

    // Move the player. Uses FixedUpdate because rigidbodies are in the physics system
    IEnumerator Levitate()
    {
        while (true)
        {
            rb.AddRelativeForce(levitationForce + driftForce);
            yield return new WaitForFixedUpdate();
        }
    }

    // Calculate the amount of drift that will be applied to the player. driftForce is lerped between min/max drift over min/max driftTime
    IEnumerator Drift()
    {
        Vector3 currentDrift = driftForce;
        Vector3 targetDrift = new Vector3(Random.Range(abilityData.minDrift, abilityData.maxDrift), 
                                          Random.Range(abilityData.minDrift, abilityData.maxDrift), 
                                          Random.Range(abilityData.minDrift, abilityData.maxDrift));

        float driftTime = Random.Range(abilityData.minDriftTime, abilityData.maxDriftTime);
        float time = 0;

        while (time < driftTime)
        {
            driftForce = Vector3.Lerp(currentDrift, targetDrift, time / driftTime);
            
            time += Time.deltaTime;
            yield return null;
        }

        driftForce = targetDrift;
        StartCoroutine(Drift());
    }

    // Turns the player. Called from PlayerController Update
    public void HandleLevitationRotation(InputState inputState)
    {
        // We always want to calculate the x rotation (tilting back / forth), but if the player is standing still then we don't calulate the y so they can look around

        float xRot = Mathf.SmoothDamp(transform.eulerAngles.x, abilityData.maxTilt * inputState.movementDirection.magnitude, ref tiltVelocity.x, abilityData.tiltSmoothTime);
        float yRot = transform.eulerAngles.y;

        if (inputState.movementDirection != Vector2.zero)
        {
            float targetYRotation = Mathf.Atan2(inputState.movementDirection.x, inputState.movementDirection.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            yRot = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetYRotation, ref tiltVelocity.y, abilityData.rotateSmoothTime);
        }
            
        transform.eulerAngles = new Vector3(xRot, yRot, transform.eulerAngles.z);
    }

    // Sets levitationForce. Called from PlayerController Update
    public void HandleLevitationMovement(PlayerInputState inputState)
    {
        float xForce = inputState.movementDirection.x * abilityData.levitationSpeed;
        float zForce = inputState.movementDirection.y * abilityData.levitationSpeed;
        float yForce = 0;

        if (inputState.levitationVerticalState == PlayerInputState.LevitationVerticalState.UP) yForce = abilityData.upForce;                    
        else if (inputState.levitationVerticalState == PlayerInputState.LevitationVerticalState.DOWN) yForce = abilityData.downForce;

        // Because the player leans forward when moving, we need to add a bit of force because we are using AddRelativeForce
        // Will (maybe) think of a better solution later. 8.815 is just a random number that I got after a few guesses that seems to work well xd
        if (Mathf.Abs(xForce) > 0 || Mathf.Abs(zForce) > 0) yForce += abilityData.maxTilt * 8.815f;  

        levitationForce = new Vector3(xForce, yForce, zForce);
    }

    // Sets the levitation animation. Called from PlayerController Update 
    public void HandleLevitationAnimation(InputState inputState)
    {
        // Not my job...
        animator.SetFloat("speedPercent", 0);
    }
}