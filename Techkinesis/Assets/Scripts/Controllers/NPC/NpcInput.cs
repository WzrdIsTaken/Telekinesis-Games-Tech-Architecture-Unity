using System;
using UnityEngine;

// Manages NPC input. In this project its just for demonstration 

[CreateAssetMenu(fileName = "NpcInputProvider", menuName = "ScriptableObjects/InputProviders/NPC InputProvider", order = 2)]
public class NpcInput : ScriptableObject, IInputProvider<InputState>
{
#pragma warning disable CS0067  // Disable this warning because I know its not being used - because this sort of logic is out of scope!
    public event Action OnJump;
#pragma warning restore

    public InputState GetState()
    {
        // Some cool AI Logic would go here!

        throw new NotImplementedException();
    }
}