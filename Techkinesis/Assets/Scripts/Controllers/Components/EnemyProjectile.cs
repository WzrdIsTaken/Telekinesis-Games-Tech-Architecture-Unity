using UnityEngine;

// Base class for anything that enemies attack the player with

public class EnemyProjectile : MonoBehaviour
{
    [Tooltip("How much damage the projectile will deal")]
    [SerializeField] int damage;

    public int GetDamage()
    {
        return damage;
    }
}