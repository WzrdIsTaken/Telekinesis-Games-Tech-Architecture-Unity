using UnityEngine;

// The stuff you actually want to look at

public class AbilityController : MonoBehaviour
{
    [SerializeField] LayerMask abilityInteractionMask;
    [SerializeField] float launchRaycastRange = 10;

    public void LaunchStart()
    {
        print("Launch Start");

        //RaycastHit hit = RaycastSystem.Raycast(cam.Value.transform.position, cam.Value.transform.forward, launchRaycastRange, abilityInteractionMask);

        //switch (hit.collider.tag)
        //{
        //    default:
        //        
        //        break;
        //}
    }

    public void LaunchEnd()
    {
        print("Launch End");
    }

    public void ShieldStart()
    {
        print("Sheild Start");
    }

    public void ShieldEnd()
    {
        print("Shield End");
    }

    public void LevitationStart()
    {
        print("Levitation Start");
    }

    public void LevitationEnd()
    {
        print("Levitation End");
    }
}