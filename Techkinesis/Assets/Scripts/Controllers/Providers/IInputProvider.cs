using System;
using UnityEngine;

// Interface that all Input Providers (eg player, npc, etc) can use 

public interface IInputProvider
{
    event Action OnJump;
   
    InputState GetState();
}

public struct InputState
{
    public Vector2 movementDirection;
    public bool running;

    public InputState(Vector2 _movementDirection, bool _running=false)
    {
        movementDirection = _movementDirection;
        running = _running;
    }
}