using UnityEngine;

// Levitation

public class LevitationAbility : MonoBehaviour
{
    [SerializeField] float levitationSpeed;
    [SerializeField] float upForce;
    [SerializeField] float downForce;

    [Space]
    [SerializeField] float maxTilt;
    [SerializeField] float tiltSmoothTime;

    [Space]
    [SerializeField] float rotateSmoothTime;

    [Space]
    [SerializeField] float minDrift;
    [SerializeField] float maxDrift;

    Camera cam;
    PlayerController playerController;
    Animator animator;
    Rigidbody rb;

    Vector3 levitationForce;
    Vector3 levitationRotation;
    Vector3 tiltVelocity;
    //float rotateVelocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.detectCollisions = false;
        rb.isKinematic = true;
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
    }

    public void LevitationEnd()
    {
        playerController.SetMovementState(PlayerController.MovementState.GROUND);
        rb.isKinematic = true;
    }

    void FixedUpdate()
    {
        if (rb.isKinematic) return;

        rb.AddRelativeForce(Vector3.up * levitationForce.y);        // Up / down
        rb.AddRelativeForce(Vector3.forward * levitationForce.z);   // Forward / back

        rb.rotation = Quaternion.Euler(levitationRotation);
    }

    public void HandleLevitationRotation(InputState inputState)
    {
        float xRot = rb.rotation.x;
        float yRot = rb.rotation.y;
        float zRot = rb.rotation.z;

        //xRot = Mathf.SmoothDamp(levitationRotation.x, maxTilt * inputState.movementDirection.y, ref tiltVelocity.x, tiltSmoothTime);
        // yRot = Mathf.SmoothDamp(levitationRotation.y, maxTilt * inputState.movementDirection.x, ref tiltVelocity.y, tiltSmoothTime);

        float targetYRotation = Mathf.Atan2(inputState.movementDirection.x, inputState.movementDirection.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
        yRot = Mathf.SmoothDamp(levitationRotation.y, targetYRotation, ref tiltVelocity.y, rotateSmoothTime);

        levitationRotation = new Vector3(xRot, yRot, zRot);
    }

    public void HandleLevitationMovement(InputState inputState)
    {
        // Space becomes go up, shift go down?

        float xForce = 0;
        float yForce = 0;
        float zForce = 0;

        if (Input.GetKey(KeyCode.Space)) yForce = upForce;                         // TODO: Change these controls
        else if (Input.GetKey(KeyCode.LeftControl)) yForce = downForce;

        xForce = inputState.movementDirection.x * levitationSpeed;
        zForce = inputState.movementDirection.y * levitationSpeed;

        /*
        levitationForce = new Vector3
        {
            x = 0,
            y = inputState.movementDirection.x > 1 ? upForce : downForce,
            z = inputState.movementDirection.y * levitationSpeed
        };
        */

        levitationForce = new Vector3(xForce, yForce, zForce);
    }

    public void HandleLevitationAnimation(InputState inputState)
    {
        // Not my job...
        animator.SetFloat("speedPercent", 0);
    }
}
