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
    public InputState(Vector3 _movementDirection)
    {
        movementDirection = _movementDirection;
    }

    Vector3 movementDirection;
}