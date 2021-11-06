using UnityEngine;

// Base class for anything that can move

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public abstract class MovementController : MonoBehaviour, IProjectileInteraction
{
    [Header("Attributes")]
    [Tooltip("How much HP the actor has")]
    [SerializeField, Min(1)] int hp = 100;

    [Tooltip("How much energy the actor has (used for abilities)")]
    [SerializeField, Min(1)] int energy = 100;

    [Tooltip("How fast the actor regenerates energy (1 energy per energyRegenRate)")]
    [SerializeField] float energyRegenRate = 1;

    [Tooltip("How heavy the player is, will affect how fast they fall")]
    [SerializeField, Min(1)] protected float mass = -12f;

    protected CharacterController controller;  // I am using a CharacterController because it supports walking up stairs / slopes out of the box 
    protected Animator animator;

    protected HealthModule healthModule;
    protected EnergyModule energyModule;

    public virtual void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        healthModule = new HealthModule(hp);
        energyModule = new EnergyModule(energy, energyRegenRate, this);
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