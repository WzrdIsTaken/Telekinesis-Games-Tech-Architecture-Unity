using UnityEngine;

// Base class for anything that can move

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public abstract class MovementController : MonoBehaviour, IProjectileInteraction
{
    [Header("Attributes")]
    [Tooltip("How much HP the player has")]
    [SerializeField, Min(1)] int hp = 100;

    [Tooltip("How heavy the player is, will affect how fast they fall")]
    [SerializeField, Min(1)] protected float mass = -12f;

    protected CharacterController controller;  // I am using a CharacterController because it supports walking up stairs / slopes out of the box 
    protected Animator animator;

    protected HealthModule healthModule;

    public virtual void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        healthModule = new HealthModule(hp);
    }

    // Called from the IProjectileInteraction interface. Called with a projectile collides with the CharacterController
    public void ProjectileCollision(int damage)
    {
        TakeDamage(damage);
    }

    public virtual void TakeDamage(int damage)
    {
    }
}