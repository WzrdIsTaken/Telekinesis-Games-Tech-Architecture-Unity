using UnityEngine;
using System.Collections;

// Levitation

public class LevitationAbility : MonoBehaviour
{
    [SerializeField] float levitationSpeed;
    [SerializeField] float upForce;
    [SerializeField] float downForce;

    [Space]
    [SerializeField] float maxTilt;
    [SerializeField] float tiltSmoothTime;
    [SerializeField] float rotateSmoothTime;

    [Space]
    [SerializeField] float minDrift;
    [SerializeField] float maxDrift;
    [SerializeField] float minDriftTime;
    [SerializeField] float maxDriftTime;

    [Space]
    [SerializeField] Vector3 startBoostForce;

    Camera cam;
    PlayerController playerController;
    Animator animator;
    Rigidbody rb;

    Vector3 levitationForce;
    Vector3 driftForce;
    Vector2 tiltVelocity;

    Coroutine levitate;
    Coroutine drift;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.detectCollisions = false;
    }

    public void PassReferences(Camera _cam, PlayerController _playerController, Animator _animator)
    {
        cam = _cam;
        playerController = _playerController;
        animator = _animator;
    }

    public void LevitationStart()
    {
        playerController.SetMovementState(PlayerController.MovementState.LEVITATION);
        rb.isKinematic = false;
        rb.AddForce(startBoostForce, ForceMode.Force);

        levitate = StartCoroutine(Levitate());
        drift = StartCoroutine(Drift());
    }

    public void LevitationEnd()
    {
        playerController.SetMovementState(PlayerController.MovementState.GROUND);
        rb.isKinematic = true;

        if (levitate != null) StopCoroutine(levitate);
        if (drift != null) StopCoroutine(drift);
    }

    IEnumerator Levitate()
    {
        while (true)
        {
            rb.AddRelativeForce(levitationForce + driftForce);
            yield return new WaitForFixedUpdate();
        }
    }

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

    public void HandleLevitationRotation(InputState inputState)
    {
        float xRot = Mathf.SmoothDamp(transform.eulerAngles.x, maxTilt * inputState.movementDirection.magnitude, ref tiltVelocity.x, tiltSmoothTime);
        float yRot = transform.eulerAngles.y;

        if (inputState.movementDirection != Vector2.zero)
        {
            float targetYRotation = Mathf.Atan2(inputState.movementDirection.x, inputState.movementDirection.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            yRot = Mathf.SmoothDamp(transform.eulerAngles.y, targetYRotation, ref tiltVelocity.y, rotateSmoothTime);
        }
            
        transform.eulerAngles = new Vector3(xRot, yRot, transform.eulerAngles.z);
    }

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

    public void HandleLevitationAnimation(InputState inputState)
    {
        // Not my job...
        animator.SetFloat("speedPercent", 0);
    }
}