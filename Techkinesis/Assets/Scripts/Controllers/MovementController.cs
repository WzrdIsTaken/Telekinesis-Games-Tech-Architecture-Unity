using UnityEngine;

// Base class for anything that can move

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public abstract class MovementController<T> : MonoBehaviour, IProjectileInteraction where T: ControllerDataBase
{
    [Tooltip("The data for the controller (ie the values it will pull from)")]
    [SerializeField] protected T controllerData;

    protected CharacterController controller;  // I am using a CharacterController because it supports walking up stairs / slopes out of the box 
    protected Animator animator;

    protected HealthModule healthModule;
    protected EnergyModule energyModule;

    public virtual void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (controllerData)
        {
            healthModule = new HealthModule(controllerData.hp);
            energyModule = new EnergyModule(controllerData.energy, controllerData.energyRegenRate, this);
        }
        else
        {
            // Ok so this is a little ugly but if there is no controller data, then just setup these modules with some really low default values
            // This is only needed because I don't want to make a DemoNPCControllerData class 

            // Debug.LogError(gameObject.name + "'s controller data is not assigned!");

            healthModule = new HealthModule(1);
            energyModule = new EnergyModule(1, 1, this);
        }
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