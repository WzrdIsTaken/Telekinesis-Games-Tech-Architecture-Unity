using System;
using UnityEngine;

// Manages NPC input

[CreateAssetMenu(fileName = "NpcInputProvider", menuName = "ScriptableObjects/InputProviders/NPC InputProvider", order = 2)]
public class NpcInput : ScriptableObject, IInputProvider
{
    public event Action OnJump;

    public event Action OnAbilityOneStart, OnAbilityOneEnd;
    public event Action OnAbilityTwoStart, OnAbilityTwoEnd;

    public event Action OnMindControlStart, OnMindControlEnd;

    public InputState GetState()
    {
        // Get some cool AI Logic

        throw new NotImplementedException();
    }
}