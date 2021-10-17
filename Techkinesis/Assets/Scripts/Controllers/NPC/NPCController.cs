using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows for NPCs to be controlled, both by 'themselves' and by the player

public class NPCController : MovementController
{
    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public bool TryMindControl()
    {
        // Check if the player can mind control the target. Maybe some sort of 'mind control strength' variable?
        // Maybe also a variable which controls how much 'energy' it takes to control the NPC per second?

        return false;
    }

    public override void MindControlEnd()
    {
        base.MindControlEnd();

        // Give control back to the players character
    }
}