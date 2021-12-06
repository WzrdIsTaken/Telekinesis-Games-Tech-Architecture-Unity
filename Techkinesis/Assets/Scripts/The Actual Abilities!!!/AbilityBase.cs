using UnityEngine;
using System.Collections;

// The base class for all abilities. Implements the template method pattern because we only want to execute the ability code if the actor has enough energy

public abstract class AbilityBase<T> : MonoBehaviour where T: AbilityDataBase
{
    [Tooltip("The data for the ability (ie the values it will pull from)")]
    [SerializeField] protected T abilityData;
    
    EnergyModule energyModule;

    Coroutine useEnergyContinuously;

    void Start()
    {
        if (!abilityData)
        {
            Debug.LogError(gameObject.name + "'s ability data is not assigned!");
        }
    }

    public void Setup(EnergyModule _energyModule)
    {
        energyModule = _energyModule;
    }

    // Called from the actor. Checks what the EnergyDrainType is and if they have the energy to perform the ability
    public void DoAbilityStart()
    {
        switch (abilityData.energyDrainType)
        {
            case AbilityDataBase.EnergyDrainType.INSTANT:
                if (energyModule.UseEnergy(abilityData.energyCost))
                {
                    AbilityStart();
                    energyModule.SetCanRegen(false);
                }
                break;
            case AbilityDataBase.EnergyDrainType.CONTINUOUS:
                if (energyModule.GetEnergy() >= abilityData.energyCost)
                {
                    useEnergyContinuously = StartCoroutine(UseEnergyContinuously());
                    energyModule.SetCanRegen(false);

                    AbilityStart();
                }
                break;
            default:
                Debug.LogError("The EnergyDrainType of type " + abilityData.energyDrainType.ToString() + " does not exist!");
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
        switch (abilityData.energyDrainType)
        {
            case AbilityDataBase.EnergyDrainType.INSTANT:
                break;
            case AbilityDataBase.EnergyDrainType.CONTINUOUS:
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
            if (!energyModule.UseEnergy(abilityData.energyCost))
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