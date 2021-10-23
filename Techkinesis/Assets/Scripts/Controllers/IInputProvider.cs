using System;
using UnityEngine;

// Interface that all Input Providers (eg player, npc, etc) must use 

public interface IInputProvider<T> where T: InputState
{
    event Action OnJump;                                

    T GetState();
}

public abstract class InputState
{
    public readonly Vector2 movementDirection;
    public readonly bool running;

    public InputState(Vector2 _movementDirection, bool _running)
    {
        movementDirection = _movementDirection;
        running = _running;
    }
}