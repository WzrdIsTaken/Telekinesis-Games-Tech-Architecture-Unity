using UnityEngine;

public class PlayerController : MovementController
{
    [SerializeField] float speedSmoothTime = 0.1f;
    [SerializeField] float walkSpeed = 2, runSpeed = 6, jumpHeight = 1;
    
    float speedSmoothVelocity, currentSpeed;
    
    public override void Update()
    {
        base.Update();

        // Movement
        float targetSpeed = (inputState.running ? runSpeed : walkSpeed) * inputState.movementDirection.magnitude;
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

        velocity += transform.forward * currentSpeed;
        controller.Move(velocity * Time.deltaTime);

        // Animation
        float currentControllerSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
        float animationSpeedPercent = inputState.running ? currentControllerSpeed / runSpeed : currentControllerSpeed / walkSpeed * 0.5f;
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }

    public override void Jump()
    {
        if (!controller.isGrounded) return;

        float jumpVelocity = Mathf.Sqrt(-2 * mass * jumpHeight);
        velocityY = jumpVelocity;
    }
}