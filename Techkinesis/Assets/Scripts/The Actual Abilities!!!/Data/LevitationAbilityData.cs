using UnityEngine;

// Holds all the variables needed for the levitation ability

[CreateAssetMenu(fileName = "LevitationAbilityData", menuName = "ScriptableObjects/AbilityData/Levitation Ability Data", order = 0)]
public class LevitationAbilityData : AbilityDataBase
{
    [Space]
    [Tooltip("How fast the player can fly on the x/y axis")]
    public float levitationSpeed;

    [Tooltip("How fast the player can move up")]
    public float upForce;

    [Tooltip("How fast the player can move down")]
    public float downForce;

    [Space]
    [Tooltip("How far the player tilts forward when levitating")]
    public float maxTilt;

    [Tooltip("How long it takes from the player to go from upright to tilted")]
    public float tiltSmoothTime;

    [Tooltip("How long it takes the player to rotate ")]
    public float rotateSmoothTime;

    [Space]
    [Tooltip("The minimum amount that the player will be buffeted around while levitating ")]
    public float minDrift;

    [Tooltip("The maximum amount that the player will be buffeted around while levitating")]
    public float maxDrift;

    [Tooltip("The minimum time that the player will move in a certain 'buffet direction'")]
    public float minDriftTime;

    [Tooltip("The maximum time that the player will move in a certain 'buffet direction'")]
    public float maxDriftTime;

    [Space]
    [Tooltip("How much the player will boosted off the ground when they start levitating")]
    public Vector3 startBoostForce;

    [Space]
    [Tooltip("The cameras FoV when levitating")]
    public float levitatingFov;

    [Tooltip("How long it will take the cameras FoV to change when the player starts / stops levitating")]
    public float levitatingFovChangeTime;
}