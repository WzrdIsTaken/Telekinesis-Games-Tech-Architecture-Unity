using UnityEngine;

// Handles raycasts. Has some basic debug features

public static class RaycastSystem
{
    // Fires a raycast from startPosition in direction
    public static RaycastHit Raycast(Vector3 startPosition, Vector3 direction, float range, LayerMask mask, bool debug=false)
    {
        bool hit = Physics.Raycast(startPosition, direction, out RaycastHit raycast, range, mask);
        
        if (debug)
        {
            if (hit)
            {
                Debug.DrawRay(startPosition, direction * raycast.distance, Color.yellow);
                Debug.Log("Raycast hit " + raycast.collider.name + "!");
            }
            else
            {
                Debug.DrawRay(startPosition, direction * raycast.distance, Color.white);
                Debug.Log("Raycast did not hit!");
            }
        }

        return raycast;
    }
}