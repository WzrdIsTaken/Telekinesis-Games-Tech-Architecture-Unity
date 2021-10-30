using UnityEngine;
using System.Collections;

// Allows the player to fly

public class LevitationAbility : MonoBehaviour
{
    [SerializeField] float levitationSpeed;    // How fast the player can fly on the x/y axis
    [SerializeField] float upForce;            // How fast the player can move up
    [SerializeField] float downForce;          // How fast the player can move down

    [Space]
    [SerializeField] float maxTilt;            // How far the player tilts forward when levitating
    [SerializeField] float tiltSmoothTime;     // How long it takes from the player to go from upright to tilted
    [SerializeField] float rotateSmoothTime;   // How long it takes the player to rotate 

    [Space]
    [SerializeField] float minDrift;           // The minimum amount that the player will be buffeted around while levitating 
    [SerializeField] float maxDrift;           // The maximum amount that the player will be buffeted around while levitating
    [SerializeField] float minDriftTime;       // The minimum time that the player will move in a certain 'buffet direction'
    [SerializeField] float maxDriftTime;       // The maximum time that the player will move in a certain 'buffet direction'

    [Space]
    [SerializeField] Vector3 startBoostForce;  // How much the player will boosted off the ground when they start levitating

    Camera cam;
    PlayerController playerController;
    Animator animator;
    Rigidbody rb;

    Vector3 levitationForce;
    Vector3 driftForce;
    Vector2 tiltVelocity;

    Coroutine levitate;
    Coroutine drift;

    // Grab the rigid body and disable collisions because these are already handled by the character controller
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.detectCollisions = false;
    }

    // Pass LevitationAbility various references that are already 'got' in PlayerController
    public void PassReferences(Camera _cam, PlayerController _playerController, Animator _animator)
    {
        cam = _cam;
        playerController = _playerController;
        animator = _animator;
    }

    // Start levitating. Called from an event hooked up in PlayerController Start
    public void LevitationStart()
    {
        playerController.SetMovementState(PlayerController.MovementState.LEVITATION);
        rb.isKinematic = false;
        rb.AddForce(startBoostForce, ForceMode.Force);

        levitate = StartCoroutine(Levitate());
        drift = StartCoroutine(Drift());
    }

    // Stop levitating. Called from an event hooked up in PlayerController Start
    public void LevitationEnd()
    {
        playerController.SetMovementState(PlayerController.MovementState.GROUND);
        rb.isKinematic = true;

        if (levitate != null) StopCoroutine(levitate);
        if (drift != null) StopCoroutine(drift);
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
        Vector3 targetDrift = new Vector3(Random.Range(minDrift, maxDrift), Random.Range(minDrift, maxDrift), Random.Range(minDrift, maxDrift));

        float driftTime = Random.Range(minDriftTime, maxDriftTime);
        float time = 0;

        while (time < driftTime)
        {
            driftForce = Vector3.Lerp(currentDrift, targetDrift, time / driftTime);
            
            time += Time.deltaTime;
            yield return null;
        }

        driftForce = targetDrift;
        drift = StartCoroutine(Drift());
    }

    // Turns the player. Called from PlayerController Update
    public void HandleLevitationRotation(InputState inputState)
    {
        // We always want to calculate the x rotation (tilting back / forth), but if the player is standing still then we don't calulate the y so they can look around

        float xRot = Mathf.SmoothDamp(transform.eulerAngles.x, maxTilt * inputState.movementDirection.magnitude, ref tiltVelocity.x, tiltSmoothTime);
        float yRot = transform.eulerAngles.y;

        if (inputState.movementDirection != Vector2.zero)
        {
            float targetYRotation = Mathf.Atan2(inputState.movementDirection.x, inputState.movementDirection.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            yRot = Mathf.SmoothDamp(transform.eulerAngles.y, targetYRotation, ref tiltVelocity.y, rotateSmoothTime);
        }
            
        transform.eulerAngles = new Vector3(xRot, yRot, transform.eulerAngles.z);
    }

    // Sets levitationForce. Called from PlayerController Update
    public void HandleLevitationMovement(PlayerInputState inputState)
    {
        float xForce = inputState.movementDirection.x * levitationSpeed;
        float zForce = inputState.movementDirection.y * levitationSpeed;
        float yForce = 0;

        if (inputState.levitationVerticalState == PlayerInputState.LevitationVerticalState.UP) yForce = upForce;                    
        else if (inputState.levitationVerticalState == PlayerInputState.LevitationVerticalState.DOWN) yForce = downForce;

        // Because the player leans forward when moving, we need to add a bit of force because we are using AddRelativeForce.
        // Will (maybe) think of a better solution later. 8.815 is just a random number that I got after a few guesses that seems to work well xd.
        if (Mathf.Abs(xForce) > 0 || Mathf.Abs(zForce) > 0) yForce += maxTilt * 8.815f;  

        levitationForce = new Vector3(xForce, yForce, zForce);
    }

    // Sets the levitation animation. Called from PlayerController Update 
    public void HandleLevitationAnimation(InputState inputState)
    {
        // Not my job...
        animator.SetFloat("speedPercent", 0);
    }
}