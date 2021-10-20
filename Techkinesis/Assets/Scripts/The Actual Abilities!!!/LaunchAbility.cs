using UnityEngine;
using System.Collections;

// Launch

public class LaunchAbility : MonoBehaviour
{
    [SerializeField] SpringJoint launchPullPoint;
    [SerializeField] LayerMask launchInteractionMask;
    [SerializeField] float launchRaycastRange;

    [Space]
    [SerializeField] float pullForceMultiplier;
    [SerializeField] float throwForceMultiplier;
    [SerializeField] float startReleaseVelocity;
    [SerializeField] float velocityBuildUpRate;

    [Space]
    [SerializeField] float pulledObjectMaxVelocity;
    [SerializeField] float thrownObjectMaxVelocity;
    [SerializeField] float velocityDistanceThreshold;
    [SerializeField] float positionDistanceThreshold;

    [Space]
    [SerializeField] Color pulledObjectOutlineColour; // TODO: Do we need this?

    Rigidbody heldObject;
    Coroutine pullObject;
    Coroutine storeReleaseEnergy;

    float releaseVelocity;

    Camera cam;

    void Start()
    {
        releaseVelocity = startReleaseVelocity;
    }

    public void AssignCamera(Camera playerCamera)
    {
        cam = playerCamera;
    }

    public void LaunchStart()
    {
        RaycastHit hit = RaycastSystem.Raycast(cam.transform.position, cam.transform.forward, launchRaycastRange, launchInteractionMask);
        Rigidbody selectedObject = null;

        if (!hit.collider)                                         // We didn't hit an object
        {
            // TODO: Make a nice sound or something
            print("You didn't hit an object!");
            return;
        }
        else if (hit.collider.CompareTag(TagManager.LAUNCHABLE))   // Hit an object that can be launched
        {
            selectedObject = hit.collider.gameObject.GetComponent<Rigidbody>();
        }
        else                                                       // Hit an object, but not once that can be launched
        {
            float meshSize = Random.Range(0.1f, 0.25f);

            selectedObject = MeshCutoutCreator.CreateMesh(hit.point, meshSize).AddComponent<Rigidbody>();
            selectedObject.tag = TagManager.LAUNCHABLE;

            GameObject one = hit.collider.gameObject;
            GameObject two = MeshCutoutCreator.CreateMesh(hit.point, meshSize);
            MeshCutter.DoOperation(MeshCutter.BoolOp.SubtractLR, one, two);

            // TODO: Cutting the same object twice *sometimes* causes stackoverflow kek
            // I can't be asked to actually code any optimisiations but can discuss them. Eg multithreading, 
            // spliting the map up into sections so don't have to cut as big of a mesh, etc
        }

        pullObject = StartCoroutine(PullObject(selectedObject));
        storeReleaseEnergy = StartCoroutine(StoreReleaseEnergy());
    }

    public void LaunchEnd()
    {
        if (pullObject != null) StopCoroutine(pullObject);
        if (storeReleaseEnergy != null) StopCoroutine(storeReleaseEnergy);

        heldObject.transform.parent = null;
        launchPullPoint.connectedBody = null;
        heldObject.constraints = RigidbodyConstraints.None;
        heldObject.velocity = cam.transform.forward * releaseVelocity * throwForceMultiplier;

        releaseVelocity = startReleaseVelocity;
        heldObject = null;
    }

    IEnumerator PullObject(Rigidbody selectedObject)
    {
        heldObject = selectedObject;
        launchPullPoint.connectedBody = heldObject;

        while (true)
        {
            Vector3 pullDirection = launchPullPoint.transform.position - heldObject.transform.position;
            Vector3 pullForce = pullDirection.normalized * pullForceMultiplier;
            float distanceToPullPoint = Vector3.Distance(heldObject.transform.position, launchPullPoint.transform.position);

            if (distanceToPullPoint < positionDistanceThreshold)
            {
                heldObject.transform.position = launchPullPoint.transform.position;
                heldObject.transform.parent = launchPullPoint.transform;
                heldObject.constraints = RigidbodyConstraints.FreezeAll;

                break;
            }

            if (heldObject.velocity.magnitude < pulledObjectMaxVelocity && distanceToPullPoint > velocityDistanceThreshold)
            {
                heldObject.AddForce(pullForce, ForceMode.Force);
            }
            else
            {
                heldObject.velocity = pullDirection.normalized * pulledObjectMaxVelocity;
            }

            yield return null;
        }
    }

    IEnumerator MakeObjectWobble()
    {
        while (true)
        {
            yield return null;
        }
    }

    IEnumerator StoreReleaseEnergy()
    {
        while (true)
        {
            releaseVelocity += velocityBuildUpRate * Time.deltaTime;
            if (releaseVelocity >= thrownObjectMaxVelocity) break;
            print(releaseVelocity);
            yield return null;
        }
    }

    void OutlinePulledObject(bool outline)
    {
        // pulledObjectOutlineColour
    }
}