using UnityEngine;

// Contains some methods all projectiles need

public static class ProjectileManager
{
    // Adds the Projectile script to a gameobject and passes in the relevant values
    public static void SetupProjectile(GameObject gO, LayerMask targetLayer, int damage, bool destroyOnCollision)
    {
        Projectile projectile = gO.GetComponent<Projectile>();
        if (!projectile) projectile = gO.AddComponent<Projectile>();

        projectile.Setup(targetLayer, damage, destroyOnCollision);
    }

    public static int CalculateProjectileDamage(float mass, float force, float multiplier)
    {
        return Mathf.RoundToInt(force * mass * multiplier);
    }
}