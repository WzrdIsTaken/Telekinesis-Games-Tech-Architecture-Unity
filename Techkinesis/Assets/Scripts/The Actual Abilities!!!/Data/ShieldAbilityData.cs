using UnityEngine;

// Holds all the variables needed for the shield ability

[CreateAssetMenu(fileName = "ShieldAbilityData", menuName = "ScriptableObjects/AbilityData/Shield Ability Data", order = 2)]
public class ShieldAbilityData : AbilityDataBase
{
    [Tooltip("How much the shield reduces incoming damage. 1 = less reduction, 0 = more reduction")]
    [Range(0, 1)] public float damageReduction;

    [Space]
    [Tooltip("What objects can be pulled to create the shield")]
    public LayerMask shieldPullInteractionMask;

    [Tooltip("From how far away objects will be pulled to create the shield")]
    public float shieldPullRange;

    [Tooltip("How much objects already tagged launchable will me prioritised with gathering debris. 1 = more bais, 0 = less bias")]
    [Range(0, 1)] public float launchableBias;

    [Space]
    [Tooltip("How long it takes the shield to form to full size")]
    public float shieldFormTime;

    [Tooltip("How big the shield is")]
    public float shieldSize;

    [Tooltip("The minimum objects that will be used to create a shield")]
    public int minShieldObjects;

    [Tooltip("The maximum objects that will be used to create a shield. NOTE: Not configurable at runtime as shieldPoints are pooled in Start")]
    public int maxShieldObjects;

    [Space]
    [Tooltip("The minimum size objects that are cut from the mesh will be")]
    public float minRandomShieldObjectSize;

    [Tooltip("The maximum size objects that are cut from the mesh will be")]
    public float maxRandomShieldObjectSize;

    [Tooltip("[WARNING - EXPENSIVE (With current level of optimisation this is wayy to expensive..)] " +
             "Determines whether the mesh will be cut when a random object is created for the sheild")]
    public bool actuallyCutMesh;

    [Space]
    [Tooltip("How fast a held object will 'wobble' in the air")]
    public float wobblePosSpeed;

    [Tooltip("The maximum amount a held object can wobble")]
    public float wobblePosAmount;

    [Tooltip("How fast a held object will rotate in the air")]
    public float wobbleRotSpeed;

    [Tooltip("The max amount a held object can rotate")]
    public float wobbleRotAmount;

    [Space]
    [Tooltip("What layers launched shield objects will perfom special interactions with (eg: damaging enemies)")]
    public LayerMask shieldLaunchInteractionMask;

    [Tooltip("The minimum force a shield object will be launched")]
    public float minShieldObjectLaunchForce;

    [Tooltip("The maximum force a shield object will be launched")]
    public float maxShieldObjectLaunchForce;

    [Tooltip("A multiplier for damage applied to the damage = force * mass calculation")]
    public float damageMultiplier;
}