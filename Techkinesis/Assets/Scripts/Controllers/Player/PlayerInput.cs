using System;
using UnityEngine;

// Gets user input and feeds it to the PlayerController via events and the PlayerInputState struct

[CreateAssetMenu(fileName = "PlayerInputProvider", menuName = "ScriptableObjects/InputProviders/Player InputProvider", order = 1)]
public class PlayerInput : ScriptableObject, IInputProvider<PlayerInputState>
{
    public event Action OnJump;

    public event Action OnLaunchStart,       OnLaunchEnd;
    public event Action OnShieldStart,       OnShieldEnd;
    public event Action OnLevitationStart,   OnLevitationEnd;

    public event Action OnSwitchCameraSide;

    [Header("Controls")]  // TODO: Allow customisation of movement keys
    [SerializeField] KeyCode run = KeyCode.LeftShift;
    [SerializeField] KeyCode jump = KeyCode.Space;
    [SerializeField] KeyCode launch = KeyCode.E;
    [SerializeField] KeyCode shield = KeyCode.Q;
    [SerializeField] KeyCode levitate = KeyCode.V;

    [Space]
    [SerializeField] KeyCode levitateUpKey = KeyCode.Space;
    [SerializeField] KeyCode levitateDownKey = KeyCode.LeftControl;

    [Space]
    [SerializeField] KeyCode switchCameraSide = KeyCode.R;

    bool isLevitating = false;

    // Returns a PlayerInputState struct containing movement infomation, as well as fires the various events. Called from PlayerController Update
    public PlayerInputState GetState()
    {
        PlayerInputState input = new PlayerInputState
        (
            new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized,   // Movement Direction
            Input.GetKey(run),                                                                      // Running  
            GetLevitationState()                                                                    // Levitation State
        );

        if (Input.GetKeyDown(jump)) OnJump();                                                       // Jump

        if (Input.GetKeyDown(launch)) OnLaunchStart();                                              // Launch Start
        if (Input.GetKeyUp(launch)) OnLaunchEnd();                                                  // Launch End

        if (Input.GetKeyDown(shield)) OnShieldStart();                                              // Shield Start
        if (Input.GetKeyUp(shield)) OnShieldEnd();                                                  // Shield End

        if (Input.GetKeyDown(levitate))
        {
            if (!isLevitating) OnLevitationStart();                                                 // Levitation Start
            else OnLevitationEnd();                                                                 // Levitation End

            isLevitating = !isLevitating;
        }

        if (Input.GetKeyDown(switchCameraSide)) OnSwitchCameraSide();                               // Switch Camera Side                       

        return input;
    }

    // Returns if the player is levitating up or down
    PlayerInputState.LevitationVerticalState GetLevitationState()
    {
        if (isLevitating)
        {
            if (Input.GetKey(levitateUpKey))   return PlayerInputState.LevitationVerticalState.UP;
            if (Input.GetKey(levitateDownKey)) return PlayerInputState.LevitationVerticalState.DOWN;
        }

        return PlayerInputState.LevitationVerticalState.NONE;
    }
}

// This struct will be created and used to work out the players movement values every frame
public class PlayerInputState : InputState
{
    public enum LevitationVerticalState { UP, DOWN, NONE };
    public readonly LevitationVerticalState levitationVerticalState;

    public PlayerInputState(Vector2 _movementDirection, bool _running, LevitationVerticalState _levitationVerticalState) : base(_movementDirection, _running)
    {
        levitationVerticalState = _levitationVerticalState;
    }
}