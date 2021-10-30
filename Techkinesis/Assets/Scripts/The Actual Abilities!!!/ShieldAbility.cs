using UnityEngine;
using System.Collections;

// Forms a shield infront of the player

public class ShieldAbility : MonoBehaviour
{
    [SerializeField] Transform shieldFormPoint;        // Where the shield will form
    [SerializeField] LayerMask shieldInteractionMask;  // What objects can be pulled to create the shield
    [SerializeField] float shieldPullRange;            // From how far away objects will be pulled to create the shield

    [Space]
    [SerializeField] float shieldSize;                 // How big the shield is

    public void ShieldStart()
    {
        print("Sheild Start");
    }

    public void ShieldEnd()
    {
        print("Shield End");
    }

    IEnumerator FormShield()
    {
        yield return null;
    }
}