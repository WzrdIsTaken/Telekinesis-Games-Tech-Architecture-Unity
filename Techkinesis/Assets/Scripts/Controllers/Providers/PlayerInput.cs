using System;
using UnityEngine;

// Manages user input

[CreateAssetMenu(fileName = "PlayerInputProvider", menuName = "ScriptableObjects/InputProviders/Player InputProvider", order = 1)]
public class PlayerInput : ScriptableObject, IInputProvider
{
    public event Action OnJump;

    public InputState GetState()
    {
        // TODO: Finish setting up user input with the new input system

        return new InputState(Vector3.zero);
    }
}