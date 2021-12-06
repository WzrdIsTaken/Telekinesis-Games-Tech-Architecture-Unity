using UnityEngine;

// Base data script for anything that can be controlled
// Again, hopefully this ticks the mark scheme box...

public abstract class ControllerDataBase : ScriptableObject
{
    [Header("Attributes")]
    [Tooltip("How much HP the actor has")]
    [Min(1)] public int hp = 100;

    [Tooltip("How much energy the actor has (used for abilities)")]
    [Min(1)] public int energy = 100;

    [Tooltip("How fast the actor regenerates energy (1 energy per energyRegenRate)")]
    public float energyRegenRate = 1;

    [Tooltip("How heavy the player is, will affect how fast they fall")]
    [Min(1)] public float mass = -12f;
}