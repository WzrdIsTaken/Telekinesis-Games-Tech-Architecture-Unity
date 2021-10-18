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
    [SerializeField] KeyCode levitate = KeyCode.V;

    bool isLevitating = false;

    public InputState GetState()
    {
        InputState input = new InputState
        (
            new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized,   // Movement Direction
            Input.GetKey(run)                                                                       // Running                         
        );

        if (Input.GetKeyDown(jump)) OnJump();                                                       // Jump

        if (Input.GetKeyDown(levitate))
        {
            if (!isLevitating) OnLevitationStart();                                                 // Levitation Start
            else OnLevitationEnd();                                                                 // Levitation End

            isLevitating = !isLevitating;
        }

        if (Input.GetKeyDown(launch)) OnLaunchStart();                                              // Launch Start
        if (Input.GetKeyUp(launch)) OnLaunchEnd();                                                  // Launch End

        if (Input.GetKeyDown(shield)) OnShieldStart();                                              // Shield Start
        if (Input.GetKeyUp(shield)) OnShieldEnd();                                                  // Shield End

        return input;
    }
}