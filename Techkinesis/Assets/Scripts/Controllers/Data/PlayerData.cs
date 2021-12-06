using UnityEngine;

// All the variables needed for the player

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/ControllerData/Player Data", order = 1)]
public class PlayerData : ControllerDataBase
{
    [Header("References")]
    [Tooltip("The players InputProvider")]
    public PlayerInput inputProvider;

    [Header("Movement")]
    [Tooltip("How long it will take for the player to go between stopping / walking / running")]
    public float speedSmoothTime = 0.1f;

    [Tooltip("How long it will take for the player to turn")]
    public float turnSmoothTime = 0.2f;

    [Tooltip("How fast the player will walk")]
    public float walkSpeed = 2;

    [Tooltip("How fast the player will run")]
    public float runSpeed = 6;

    [Tooltip("How high the player will jump")]
    public float jumpHeight = 1;

    [Tooltip("How much control the player has while jumping. 1 = more control, 0 = less control")]
    [Range(0, 1)] public float airControlPercent = 0.5f;
}