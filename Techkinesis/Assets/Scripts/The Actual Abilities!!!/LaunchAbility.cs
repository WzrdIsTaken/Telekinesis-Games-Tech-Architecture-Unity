using UnityEngine;
using System.Collections;

// Launch

public class LaunchAbility : MonoBehaviour
{ 
    [SerializeField] SpringJoint launchPullPoint;        // The point to which the object is drawn towards
    [SerializeField] LayerMask launchInteractionMask;    // What layers of objects can be pulled
    [SerializeField] float launchRaycastRange;           // How far away objects can be

    [Space]
    [SerializeField] float minPulledObjectSize = 0.11f;  // WARNING!!! Having to big of an object size can cause.. interesting.. mesh slices! 
    [SerializeField] float maxPulledObjectSize = 0.11f;  // 0.11 seems to be kinda a magic number lol. Don't change it unless algorithm is optimised!
    [SerializeField] bool actuallySliceMesh;             // WARNING! EXPENSIVE!! (but cool!)

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

    public void PassReferences(Camera playerCamera)
    {
        cam = playerCamera;
    }

    public void LaunchStart()
    {
        RaycastHit hit = RaycastSystem.Raycast(cam.transform.position, cam.transform.forward, launchRaycastRange, launchInteractionMask);
        Rigidbody selectedObject = null;

        if (!hit.collider)                                         // We didn't hit an object
        {
            print("You didn't hit an object! Would make a cool sound or something");
            return;
        }
        else if (hit.collider.CompareTag(TagManager.LAUNCHABLE))   // Hit an object that can be launched
        {
            selectedObject = hit.collider.gameObject.GetComponent<Rigidbody>();
        }
        else                                                       // Hit an object, but not once that can be launched
        {
            float meshSize = Random.Range(minPulledObjectSize, maxPulledObjectSize); 

            selectedObject = MeshCutoutCreator.CreateMesh(hit.point, meshSize, hit.collider.GetComponent<MeshRenderer>().sharedMaterials).AddComponent<Rigidbody>();
            selectedObject.tag = TagManager.LAUNCHABLE;

            if (actuallySliceMesh) 
            {
                GameObject one = hit.collider.gameObject;
                GameObject two = MeshCutoutCreator.CreateMesh(hit.point, meshSize);
                MeshCutter.DoOperation(MeshCutter.BoolOp.SubtractLR, one, two);

                // TODO: Cutting an object *sometimes* causes stackoverflow kek
            }
        }

        pullObject = StartCoroutine(PullObject(selectedObject));
    }

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

    IEnumerator PullObject(Rigidbody selectedObject)
    {
        heldObject = selectedObject;
        float selectedObjectSize = selectedObject.GetComponent<MeshRenderer>().bounds.size.y / 2;

        while (true)
        {
            Vector3 pullDirection = launchPullPoint.transform.position - heldObject.transform.position;
            Vector3 pullForceVec = pullDirection.normalized * pullForce;
            float distanceToPullPoint = Vector3.Distance(heldObject.transform.position, launchPullPoint.transform.position);

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

    IEnumerator MakeObjectWobble()
    {
        launchPullPoint.connectedBody = heldObject;

        while (true)
        {
            float addToPos = Mathf.Sin(Time.time * wobblePosSpeed) * wobblePosAmount;
            heldObject.transform.localPosition += Vector3.up * addToPos * Time.deltaTime;

            float xRot = Mathf.Sin(Time.time * wobbleRotSpeed) * wobbleRotAmount;
            float zRot = Mathf.Sin((Time.time - 1f) * wobbleRotSpeed) * wobbleRotAmount;

            heldObject.transform.localEulerAngles = new Vector3(xRot, transform.eulerAngles.y, zRot);

            yield return null;
        }
    }
}