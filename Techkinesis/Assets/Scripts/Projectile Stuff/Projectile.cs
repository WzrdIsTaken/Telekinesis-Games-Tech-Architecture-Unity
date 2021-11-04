using UnityEngine;

// Base class for any projectile
// Handles both Collision and Trigger Enter as we don't know what the designer will want to use

public class Projectile : MonoBehaviour
{
    LayerMask collisionLayer;
    int damage;
    bool destroyOnCollision;

    public void Setup(LayerMask _collisionLayer, int _damage, bool _destroyOnCollision)
    {
        collisionLayer = _collisionLayer;
        damage = _damage;
        destroyOnCollision = _destroyOnCollision;
    }

    void OnCollisionEnter(Collision collision)
    {
        CallProjectileCollision(collision.collider);
    }

    void OnTriggerEnter(Collider collider)
    {
        CallProjectileCollision(collider);
    }

    void CallProjectileCollision(Collider col)
    {
        if ((collisionLayer.value & (1 << col.gameObject.layer)) > 0)  // Checks if collisionLayer and col.gameObject.layer are the same
        {
            IProjectileInteraction projectileInteraction = col.gameObject.GetComponent<IProjectileInteraction>();
            if (projectileInteraction != null)
            {
                projectileInteraction.ProjectileCollision(damage);
            }
            else
            {
                Debug.LogError(gameObject.name + "(projectile) tried to interact with " + col.name + " but this gameobject " +
                                                 "doesn't use the IProjectileInteraction interface!");
            }
        }

        if (destroyOnCollision) Destroy(gameObject);
    }
}