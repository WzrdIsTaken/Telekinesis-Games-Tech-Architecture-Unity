using UnityEngine;
using System;

// Triggers the ObjectHitCollider which calls the ObjectHitShield method in ShieldAbility when the collider is hit

public class ShieldCollider : MonoBehaviour, IProjectileInteraction
{
    public Action<int> ObjectHitCollider;  // Used in ShieldAbility

    SphereCollider col;

    void Start()
    {
        col = GetComponent<SphereCollider>();
    }

    // Called rom the IProjectileInteraction interface. Called with a projectile collides with the SphereCollider
    public void ProjectileCollision(int damage)
    {
        ObjectHitCollider(damage);
    }

    // Called from ShieldAbility. Enables/disables the SphereCollider
    public void SetColliderState(bool state)
    {
        col.enabled = state;
    }
}