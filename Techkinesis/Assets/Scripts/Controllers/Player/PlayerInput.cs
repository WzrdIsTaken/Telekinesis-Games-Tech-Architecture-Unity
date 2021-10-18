using System;
using UnityEngine;

// Manages user input

[CreateAssetMenu(fileName = "PlayerInputProvider", menuName = "ScriptableObjects/InputProviders/Player InputProvider", order = 1)]
public class PlayerInput : ScriptableObject, IInputProvider
{
    public event Action OnJump;

    public event Action OnLaunchStart,       OnLaunchEnd;
    public event Action OnShieldStart,       OnShieldEnd;
    public event Action OnLevitationStart,   OnLevitationEnd;

    [Header("Controls")]  // TODO: Allow customisation of movement keys
    [SerializeField] KeyCode run = KeyCode.LeftShift;
    [SerializeField] KeyCode jump = KeyCode.Space;
    [SerializeField] KeyCode launch = KeyCode.E;
    [SerializeField] KeyCode shield = KeyCode.Q;

    [Space]
    [SerializeField, Min(0.1f)] float levitationToggleTime = 0.3f;

    float lastJumpTime;
    bool isLevitating = false;

    public InputState GetState()
    {
        InputState input = new InputState
        (
            new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized,   // Movement Direction
            Input.GetKey(run)                                                                       // Running                         
        );

        if (Input.GetKeyDown(jump))              
        {
            float timeSinceLastJump = Time.time - lastJumpTime;
            
            if (timeSinceLastJump <= levitationToggleTime)
            {
                if (!isLevitating) OnLevitationStart();                                             // Levitation Start
                else OnLevitationEnd();                                                             // Levitation End
  
                isLevitating = !isLevitating;
            }
            else OnJump();                                                                          // Jumping

            lastJumpTime = Time.time;
        }

        if (Input.GetKeyDown(launch)) OnLaunchStart();                                              // Launch Start
        if (Input.GetKeyUp(launch)) OnLaunchEnd();                                                  // Launch End

        if (Input.GetKeyDown(shield)) OnShieldStart();                                              // Shield Start
        if (Input.GetKeyUp(shield)) OnShieldEnd();                                                  // Shield End

        return input;
    }
}

// TODO: Fix bug I have no idea why its happening but wtf levitation issue awdhugdwiahuhiwud