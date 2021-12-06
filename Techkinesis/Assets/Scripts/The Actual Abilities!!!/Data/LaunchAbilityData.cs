using UnityEngine;

// Holds all the variables needed for the launch ability

[CreateAssetMenu(fileName = "LaunchAbilityData", menuName = "ScriptableObjects/AbilityData/Launch Ability Data", order = 1)]
public class LaunchAbilityData : AbilityDataBase
{
    [Space]
    [Tooltip("What layers of objects can be pulled")]
    public LayerMask launchPullInteractionMask;

    [Tooltip("How far away objects can be")]
    public float launchRaycastRange;

    [Space]
    [Tooltip("The minimum size objects that are cut from the mesh will be")]
    public float minPulledObjectSize = 0.11f;  // WARNING! If actuallyCutMesh is enabled, having to big of an object size can cause.. interesting.. mesh slices! 

    [Tooltip("The maximum size objects that are cut from the mesh will be")]
    public float maxPulledObjectSize = 0.11f;  // 0.11 seems to be kinda a magic number lol. If actuallyCutMesh is enabled don't change unless the algorithm is optimised!

    [Tooltip("The minimum time it will take to pull an object out of a mesh")]
    public float minBreakTime;

    [Tooltip("The maximum time it will take to pull an object out of a mesh")]
    public float maxBreakTime;

    [Tooltip("[WARNING - EXPENSIVE] Determines whether the mesh will be cut when a random object is created to be launched")]
    public bool actuallyCutMesh;

    [Space]
    [Tooltip("How minimum speed an object will be pulled")]
    public float minPullSpeed;

    [Tooltip("How maximum speed an object will be pulled")]
    public float maxPullSpeed;

    [Space]
    [Tooltip("The minimum distance an object will float up before being pulled to the player")]
    public float minFloatUpDistance;

    [Tooltip("The maximum distance an object will float up before being pulled to the player")]
    public float maxFloatUpDistance;

    [Tooltip("The minimum amount an object will rotate before being pulled to the player")]
    public float minFloatUpRotation;

    [Tooltip("The maximum amount an object will rotate before being pulled to the player")]
    public float maxFloatUpRotation;

    [Tooltip("The minimum time an object will float up before being pulled to the player")]
    public float minFloatUpTime;

    [Tooltip("The maximum time an object will float up before being pulled to the player")]
    public float maxFloatUpTime;

    [Tooltip("The minimum time an object will just hover in the air before being pulled to the player")]
    public float minHoverTime;

    [Tooltip("The maximum time an object will just hover in the air before being pulled to the player")]
    public float maxHoverTime;

    [Space]
    [Tooltip("Will the held object use a spring joint to simulate its movement? " +
             "Note - Cool effect but will really bug out when levitating")]  // See LaunchAbility.PullObjectToPlayer()
    public bool useSpringJoint = false;

    [Tooltip("How fast a held object will 'wobble' in the air (only used while levitating)")]
    public float wobblePosSpeed;

    [Tooltip("The maximum amount a held object can wobble (only used while levitating)")]
    public float wobblePosAmount;

    [Tooltip("How fast a held object will rotate in the air (only used while levitating)")]
    public float wobbleRotSpeed;

    [Tooltip("The max amount a held object can rotate (only used while levitating)")]
    public float wobbleRotAmount;

    [Space]
    [Tooltip("What layers launched objects will perfom special interactions with (eg: damaging enemies)")]
    public LayerMask launchLaunchInteractionMask;

    [Tooltip("How minimum force the object will be launched with")]
    public float minLaunchForce;

    [Tooltip("The maximum force the object will be launched with")]
    public float maxLaunchForce;

    [Tooltip("A multiplier for damage applied to the damage = force * mass calculation")]
    public float damageMultiplier;
}