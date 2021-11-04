// Handles everything to do with hp for anything that needs it. Should have a HAS-A relationship

public class HealthModule
{
    int health;
    readonly int MAX_HEALTH;

    public HealthModule(int maxHealth)
    {
        MAX_HEALTH = maxHealth;
        health = maxHealth;
    }

    public HealthModule(int maxHealth, int startHealth)
    {
        MAX_HEALTH = maxHealth;
        health = startHealth;
    }

    // Decrease the health value. Returns true if health is less than 1
    public bool Damage(int damage)
    {
        health -= damage;
        return health < 1;
    }

    // Increase the health value. Returns true if health is greater than or equal to MAX_HEALTH
    public bool Heal(int heal)
    {
        health = health + heal > MAX_HEALTH ? MAX_HEALTH : health + heal;
        return health >= MAX_HEALTH;
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetMaxHealth()
    {
        return MAX_HEALTH;
    }
}