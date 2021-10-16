using System;
using UnityEngine;

// Manages user input

[CreateAssetMenu(fileName = "PlayerInputProvider", menuName = "ScriptableObjects/InputProviders/Player InputProvider", order = 1)]
public class PlayerInput : ScriptableObject, IInputProvider
{
    public event Action OnJump;

    [Header("Controls")]  // TODO: Allow customisation of movement keys
    [SerializeField] KeyCode RUN_KEY = KeyCode.LeftShift;
    [SerializeField] KeyCode JUMP_KEY = KeyCode.Space;  

    public InputState GetState()
    {
        InputState input = new InputState
        (
            new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized,   // Movement Direction
            Input.GetKey(RUN_KEY)                                                                   // Running                         
        );

        if (Input.GetKeyDown(JUMP_KEY)) OnJump();                                                   // Jumping

        return input;
    }
}