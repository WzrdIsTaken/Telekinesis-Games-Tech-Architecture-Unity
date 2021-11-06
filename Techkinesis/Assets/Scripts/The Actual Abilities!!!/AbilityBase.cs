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
                    energyModule.SetCanRegen(false);
                }
                break;
            case EnergyDrainType.CONTINUOUS:
                if (energyModule.GetEnergy() >= energyCost)
                {
                    useEnergyContinuously = StartCoroutine(UseEnergyContinuously());
                    energyModule.SetCanRegen(false);

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
                break;
            case EnergyDrainType.CONTINUOUS:
                if (useEnergyContinuously != null) 
                {
                    StopCoroutine(useEnergyContinuously);
                }
                break;
            default:
                Debug.LogError("Yeah dude I have no idea how you got in this position xd");
                break;
        }

        energyModule.SetCanRegen(true);
        AbilityEnd();
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

/* 
    NOTE / TODO:
    There is a bug where if you have 0 energy and holding the shield, if you press any other ability then SetCanRegen will be called again with (true)
    so energy will begin to regenerate again. So if these scripts are ever used again for anything other than inspiration, then that will need to be fixed.
    However right now I can't (be asked to..) think of a nice way to fix it and it will probably never happen so :))
*/