using UnityEngine;
using System;

// Triggers the ObjectHitCollider which calls the ObjectHitShield method in ShieldAbility when the collider is hit
// Handles both Collision and Trigger Enter as we don't know what the designer will want to use

public class ShieldCollider : MonoBehaviour
{
    public Action<Collider> ObjectHitCollider;

    SphereCollider col;

    void Start()
    {
        col = GetComponent<SphereCollider>();
    }

    void OnCollisionEnter(Collision collision)
    {
        ObjectHitCollider(collision.collider);
    }

    void OnTriggerEnter(Collider collider)
    {
        ObjectHitCollider(collider);
    }

    // Called from ShieldAbility. Enables/disables the SphereCollider
    public void SetColliderState(bool state)
    {
        col.enabled = state;
    }
}