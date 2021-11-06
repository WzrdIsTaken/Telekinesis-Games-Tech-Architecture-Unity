using UnityEngine;
using System;
using System.Collections;

// Handles the useage of energy (energy is used for using abilities)

public class EnergyModule
{
    public Action UpdateEnergyUI;

    int currentEnergy;
    readonly int MAX_ENERGY;

    float energyRegenRate;
    bool canRegen = true;

    public EnergyModule(int _maxEnergy, float _energyRegenRate, MonoBehaviour mono)  // MonoBehaviour needed to start coroutines
    {
        MAX_ENERGY = _maxEnergy;
        energyRegenRate = _energyRegenRate;
        currentEnergy = MAX_ENERGY;

        mono.StartCoroutine(RegenEnergy());
    }

    // Take away energy from currentEnergy. Returns true if their is enough energy to do so
    public bool UseEnergy(int energy)
    {
        int energyAfterUsage = currentEnergy - energy;
        
        if (energyAfterUsage >= 0)
        {
            currentEnergy = energyAfterUsage;
            UpdateEnergyUI?.Invoke();  // Checks is the action is assigned to before calling it

            return true;
        }
        else return false;
    }

    // Increase currentEnergy. Returns true if currentEnergy is greater than or equal to MAX_HEALTH
    public bool IncreaseEnergy(int increase)
    {
        currentEnergy = currentEnergy + increase > MAX_ENERGY ? MAX_ENERGY : currentEnergy + increase;
        UpdateEnergyUI?.Invoke();

        return currentEnergy >= MAX_ENERGY;
    }

    // Regnerate energy. 1 energy per energyRegenRate
    IEnumerator RegenEnergy()
    {
        while (true)
        {
            if (canRegen && currentEnergy < MAX_ENERGY) 
            {
                currentEnergy++;
                UpdateEnergyUI?.Invoke();
            }

            yield return new WaitForSeconds(energyRegenRate);
        }
    }

    public int GetEnergy()
    {
        return currentEnergy;
    }

    public int GetMaxEnergy()
    {
        return MAX_ENERGY;
    }

    public void SetCanRegen(bool _canRegen)
    {
        canRegen = _canRegen;
    }
}