using UnityEngine;

// Shield

public class ShieldAbility : MonoBehaviour
{
    [SerializeField] LayerMask shieldInteractionMask;
    [SerializeField] float shieldPullRange;

    public void ShieldStart()
    {
        print("Sheild Start");
    }

    public void ShieldEnd()
    {
        print("Shield End");
    }
}
