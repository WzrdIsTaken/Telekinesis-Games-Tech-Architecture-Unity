using System;

// Handles everything to do with hp for anything that needs it. Should have a HAS-A relationship

public class HealthModule
{
    public Action UpdateHealthUI;  // Used in PlayerUIManager

    int currentHealth;
    readonly int MAX_HEALTH;

    public HealthModule(int maxHealth)
    {
        MAX_HEALTH = maxHealth;
        currentHealth = maxHealth;
    }

    // Decrease the health value. Returns true if health is less than 1
    public bool Damage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHealthUI?.Invoke();  // Checks is the action is assigned to before calling it

        return currentHealth < 1;
    }

    // Increase the health value. Returns true if health is greater than or equal to MAX_HEALTH
    public bool Heal(int heal)
    {
        currentHealth = currentHealth + heal > MAX_HEALTH ? MAX_HEALTH : currentHealth + heal;
        UpdateHealthUI?.Invoke();

        return currentHealth >= MAX_HEALTH;
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return MAX_HEALTH;
    }
}