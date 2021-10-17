using System;
using UnityEngine;

// Manages user input

[CreateAssetMenu(fileName = "PlayerInputProvider", menuName = "ScriptableObjects/InputProviders/Player InputProvider", order = 1)]
public class PlayerInput : ScriptableObject, IInputProvider
{
    public event Action OnJump;

    public event Action OnAbilityOneStart, OnAbilityOneEnd;
    public event Action OnAbilityTwoStart, OnAbilityTwoEnd;

    public event Action OnMindControlStart, OnMindControlEnd;

    [Header("Controls")]  // TODO: Allow customisation of movement keys
    [SerializeField] KeyCode RUN_KEY = KeyCode.LeftShift;
    [SerializeField] KeyCode JUMP_KEY = KeyCode.Space, LAUNCH_KEY = KeyCode.E, SHIELD_KEY = KeyCode.Q, CONTROL_KEY = KeyCode.C;

    public InputState GetState()
    {
        InputState input = new InputState
        (
            new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized,   // Movement Direction
            Input.GetKey(RUN_KEY)                                                                   // Running                         
        );

        if (Input.GetKeyDown(JUMP_KEY)) OnJump();                                                   // Jumping

        if (Input.GetKeyDown(LAUNCH_KEY)) OnAbilityOneStart();                                      // Launch Start
        if (Input.GetKeyUp(LAUNCH_KEY)) OnAbilityOneEnd();                                          // Launch End

        if (Input.GetKeyDown(SHIELD_KEY)) OnAbilityTwoStart();                                      // Shield Start
        if (Input.GetKeyUp(SHIELD_KEY)) OnAbilityTwoEnd();                                          // Shield End

        if (Input.GetKeyDown(CONTROL_KEY)) OnMindControlStart();                                   // Mind Control Start
        if (Input.GetKeyUp(CONTROL_KEY)) OnMindControlEnd();                                       // Mind Control End

        // ^ Need to think a little more about how mind control will work but defo want to do it
        // Simon talking about how the mind control in Control seems a little half hearted at the end, want to try and make it more enjoyable / interesting
        // You will actually 'become' the person you are controlling, and can use it to scout areas ahead
        // behind her eyes xd

        return input;
    }
}