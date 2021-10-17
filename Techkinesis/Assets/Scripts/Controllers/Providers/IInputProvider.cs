using System;
using UnityEngine;

// Interface that all Input Providers (eg player, npc, etc) can use 

public interface IInputProvider
{
    event Action OnJump;                                 // Most NPCs can jump

    event Action OnAbilityOneStart, OnAbilityOneEnd;     // Unique ability per character 1
    event Action OnAbilityTwoStart, OnAbilityTwoEnd;     // Unique ability per character 2

    event Action OnMindControlStart, OnMindControlEnd;   // All NPCs can be mind controlled by the player
   
    InputState GetState();
}

public struct InputState
{
    public Vector2 movementDirection;
    public bool running;

    public InputState(Vector2 _movementDirection, bool _running)
    {
        movementDirection = _movementDirection;
        running = _running;
    }
}