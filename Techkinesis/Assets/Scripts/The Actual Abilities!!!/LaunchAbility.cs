using UnityEngine;
using System.Collections;

// Pulls and launches and rock or object

public class LaunchAbility : MonoBehaviour
{ 
    [SerializeField] SpringJoint launchPullPoint;        // The point to which the object is drawn towards

    [Space]
    [SerializeField] LayerMask launchInteractionMask;    // What layers of objects can be pulled
    [SerializeField] float launchRaycastRange;           // How far away objects can be

    [Space]
    [SerializeField] float minPulledObjectSize = 0.11f;  // WARNING!!! Having to big of an object size can cause.. interesting.. mesh slices! 
    [SerializeField] float maxPulledObjectSize = 0.11f;  // 0.11 seems to be kinda a magic number lol. Don't change it unless algorithm is optimised!
    [SerializeField] bool actuallyCutMesh;               // WARNING! EXPENSIVE!! (but cool!)

    [Space]
    [SerializeField] float pullForce;                    // How fast the object will be pulled
    [SerializeField] float pulledObjectMaxVelocity;      // The maximum velocity which an object can be pulled
    [SerializeField] float throwForce;                   // How hard the object will be thrown

    [Space]
    [SerializeField] float wobblePosSpeed;               // How fast a held object will 'wobble' in the air
    [SerializeField] float wobblePosAmount;              // The maximum amount a held object can wobble
    [SerializeField] float wobbleRotSpeed;               // How fast a held object will rotate in the air
    [SerializeField] float wobbleRotAmount;              // The max amount a held object can rotate

    Rigidbody heldObject;
    Coroutine pullObject;
    Coroutine makeObjectWobble;

    Camera cam;

    // Recieve the player camera reference from PlayerController
    public void PassReferences(Camera playerCamera)
    {
        cam = playerCamera;
    }

    // Grab an object to be launched
    public void LaunchStart()
    {
        RaycastHit hit = RaycastSystem.Raycast(cam.transform.position, cam.transform.forward, launchRaycastRange, launchInteractionMask);
        Rigidbody selectedObject = null;

        // We didn't hit an object
        if (!hit.collider)
        {
            print("You didn't hit an object! Would make a cool sound or something");
            return;
        }

        // Hit an object that can be launched : Hit an object, but not once that can be launched so need to cut the mesh
        selectedObject = hit.collider.CompareTag(TagManager.LAUNCHABLE) ? hit.collider.gameObject.GetComponent<Rigidbody>()  
                                                                        : MeshCutter.CutAndReturnRandomMesh(hit, minPulledObjectSize, maxPulledObjectSize, actuallyCutMesh);

        pullObject = StartCoroutine(PullObject(selectedObject));
    }

    // Reset the launch system / the pulled object and launch it using throwForce
    public void LaunchEnd()
    {
        if (!heldObject) return;

        if (pullObject != null) StopCoroutine(pullObject);
        if (makeObjectWobble != null) StopCoroutine(makeObjectWobble);

        heldObject.transform.parent = null;
        launchPullPoint.connectedBody = null;
        heldObject.useGravity = true;

        heldObject.velocity = cam.transform.forward * throwForce;
        heldObject = null;
    }

    // Pull the object towards launchPullPoint
    IEnumerator PullObject(Rigidbody selectedObject)
    {
        heldObject = selectedObject;
        float selectedObjectSize = selectedObject.GetComponent<MeshRenderer>().bounds.size.y / 2;  // Needed so we know when to stop the coroutine

        while (true)
        {
            Vector3 pullDirection = launchPullPoint.transform.position - heldObject.transform.position;
            Vector3 pullForceVec = pullDirection.normalized * pullForce;
            float distanceToPullPoint = Vector3.Distance(heldObject.transform.position, launchPullPoint.transform.position);

            // Once the object has reached launchPullPoint set values and break out of the coroutine
            if (distanceToPullPoint < selectedObjectSize)
            {
                heldObject.transform.position = launchPullPoint.transform.position;
                heldObject.transform.parent = launchPullPoint.transform;
                heldObject.useGravity = false;

                heldObject.velocity = Vector3.zero;
                heldObject.angularVelocity = Vector3.zero;

                makeObjectWobble = StartCoroutine(MakeObjectWobble());

                break;
            }

            // So the object can't used keep accelerating forever
            if (heldObject.velocity.magnitude < pulledObjectMaxVelocity)
            {
                heldObject.AddForce(pullForceVec, ForceMode.Force);
            }
            else
            {
                heldObject.velocity = pullDirection.normalized * pulledObjectMaxVelocity;
            }

            yield return null;
        }
    }

    // Make the object wobble around in the air once it has reached launchPullPoint. Combination of a spring joint and sin wave
    IEnumerator MakeObjectWobble()
    {
        launchPullPoint.connectedBody = heldObject;

        while (true)
        {
            ObjectBob.SineWaveBob(heldObject.gameObject, transform, wobblePosSpeed, wobblePosAmount, wobbleRotSpeed, wobbleRotAmount);

            yield return null;
        }
    }
}