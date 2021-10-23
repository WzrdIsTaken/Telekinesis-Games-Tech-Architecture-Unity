using System;
using UnityEngine;

// Manages NPC input

[CreateAssetMenu(fileName = "NpcInputProvider", menuName = "ScriptableObjects/InputProviders/NPC InputProvider", order = 2)]
public class NpcInput : ScriptableObject, IInputProvider<InputState>
{
    public event Action OnJump;

    public InputState GetState()
    {
        // Get some cool AI Logic

        throw new NotImplementedException();
    }
}