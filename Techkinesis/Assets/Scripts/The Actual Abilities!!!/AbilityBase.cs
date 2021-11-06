using UnityEngine;
using System.Collections;

// The base class for all abilities. Implements the template method pattern because we only want to execute the ability code if the actor has enough energy

public abstract class AbilityBase : MonoBehaviour
{
    [Tooltip("Instant: The energy is used at the start. Continuous: The amount of energy is used every second")]
    [SerializeField] EnergyDrainType energyDrainType;
    enum EnergyDrainType { INSTANT, CONTINUOUS };

    [Tooltip("How much energy the ability costs to use")]
    [SerializeField] int energyCost;
    
    EnergyModule energyModule;

    Coroutine useEnergyContinuously;

    public void Setup(EnergyModule _energyModule)
    {
        energyModule = _energyModule;
    }

    // Called from the actor. Checks what the EnergyDrainType is and if they have the energy to perform the ability
    public void DoAbilityStart()
    {
        switch (energyDrainType)
        {
            case EnergyDrainType.INSTANT:
                if (energyModule.UseEnergy(energyCost))
                {
                    AbilityStart();
                }
                break;
            case EnergyDrainType.CONTINUOUS:
                if (energyModule.GetEnergy() >= energyCost)
                {
                    energyModule.SetCanRegen(false);
                    useEnergyContinuously = StartCoroutine(UseEnergyContinuously());

                    AbilityStart();
                }
                break;
            default:
                Debug.LogError("The EnergyDrainType of type " + energyDrainType.ToString() + " does not exist!");
                break;
        }
    }

    // Inherited in the ability and called if the condition required in DoAbilityStart is met
    protected virtual void AbilityStart()
    {
    }

    // Called from the actor. Checks what the EnergyDrainType is and performs the relevant cleanup
    public void DoAbilityEnd()
    {
        switch (energyDrainType)
        {
            case EnergyDrainType.INSTANT:
                AbilityEnd();
                break;
            case EnergyDrainType.CONTINUOUS:
                energyModule.SetCanRegen(true);
                StopCoroutine(useEnergyContinuously);

                AbilityEnd();
                break;
            default:
                Debug.LogError("Yeah dude I have no idea how you got in this position xd");
                break;
        }
    }

    // Inherited in the ability and called if the condition required in DoAbilityEnd is met
    protected virtual void AbilityEnd()
    {
    }

    IEnumerator UseEnergyContinuously()
    {
        while (true)
        {
            if (!energyModule.UseEnergy(energyCost))
            {
                DoAbilityEnd();
                yield break;
            }

            yield return new WaitForSeconds(1);
        }
    }
}