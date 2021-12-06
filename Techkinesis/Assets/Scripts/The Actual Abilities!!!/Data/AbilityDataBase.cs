using UnityEngine;

// Base class that all ability data scripts inherit from. Contains values all abilities use
// Makes life for a designer easier. And hopefully ticks a mark scheme box...

public abstract class AbilityDataBase : ScriptableObject
{
    [Tooltip("Instant: The energy is used at the start. Continuous: The amount of energy is used every second")]
    public EnergyDrainType energyDrainType;
    public enum EnergyDrainType { INSTANT, CONTINUOUS };

    [Tooltip("How much energy the ability costs to use")]
    public int energyCost;
}