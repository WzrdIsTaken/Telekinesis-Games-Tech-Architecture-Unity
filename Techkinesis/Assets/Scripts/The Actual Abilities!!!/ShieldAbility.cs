using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Forms a shield infront of the player

public class ShieldAbility : MonoBehaviour
{
    [SerializeField] Transform shieldFormPoint;          // Where the shield will form

    [Space]
    [SerializeField] LayerMask shieldInteractionMask;    // What objects can be pulled to create the shield
    [SerializeField] float shieldPullRange;              // From how far away objects will be pulled to create the shield
    //[SerializeField] float minDistAwayFromPlayer;        // The minimum distance away from the player that objects will be pulled TODO
    [SerializeField, Range(0, 1)] float launchableBias;  // How much objects already tagged launchable will me prioritised with gathering debris. 1 = more bais, 0 = less bais

    [Space]
    [SerializeField] float shieldFormTime;               // How long it takes the shield to form to full size
    [SerializeField] float shieldSize;                   // How big the shield is
    [SerializeField] int minShieldObjects;               // The minimum objects that will be used to create a shield
    [SerializeField] int maxShieldObjects;               // The maximum objects that will be used to create a shield. NOTE: Not configurable at runtime as shieldPoints are pooled in Start

    [Space]
    [SerializeField] float minRandomShieldObjectSize;    // The minimum size object that are cut from the mesh will be
    [SerializeField] float maxRandomShieldObjectSize;    // The maximum size object that are cut from the mesh will be
    [SerializeField] bool actuallyCutMesh;               // WARNING! EXPENSIVE!! With current level of optimisation this is wayy to expensive

    [Space]
    //[SerializeField] float minDebrisDistFromPlayer;      // 
    //[SerializeField] float maxDebrisDistFromPlayer;      //

    [Space]
    [SerializeField] float wobblePosSpeed;               // How fast a held object will 'wobble' in the air
    [SerializeField] float wobblePosAmount;              // The maximum amount a held object can wobble
    [SerializeField] float wobbleRotSpeed;               // How fast a held object will rotate in the air
    [SerializeField] float wobbleRotAmount;              // The max amount a held object can rotate

    float playerHeight;
    float raycastDownFromCircleRange = 25;               // How far the raycasts will be shot down from points gathered in GrabDebris() TODO: Make this value player height dependant

    List<Rigidbody> shieldObjects;
    List<Transform> shieldPoints = new List<Transform>();

    Coroutine formShield;
    Coroutine moveShieldObjects;

    // Create a pool of shieldPoints
    void Start()
    {
        Transform shieldPointParent = new GameObject("ShieldPoints").transform;
        shieldPointParent.transform.parent = transform;

        for (int i = 0; i < maxShieldObjects; i++)
        {
            Transform shieldPoint = new GameObject("ShieldPoint").transform;
            shieldPoint.parent = shieldPointParent;
            
            shieldPoints.Add(shieldPoint);
        }
    }

    // Pass ShieldAbility the CharacterController as already been 'got' in PlayerController
    public void PassReferences(CharacterController controller)
    {
        playerHeight = controller.height;
    }

    // Start the shield creation. Called from an event hooked up in PlayerController Start
    public void ShieldStart()
    {
        formShield = StartCoroutine(FormShield());
    }

    // Stop the shield. Called from an event hooked up in PlayerController Start
    public void ShieldEnd()
    {
        if (formShield != null) StopCoroutine(formShield);
        if (moveShieldObjects != null) StopCoroutine(moveShieldObjects);

        foreach (Rigidbody obj in shieldObjects) 
        {
            obj.transform.parent = null;

            obj.detectCollisions = true;
            obj.useGravity = true;
        }
        shieldObjects.Clear();
    }

    // Grab objects to be used in the shield, assign new shield points and then move the objects to those points
    IEnumerator FormShield()
    {
        shieldObjects = GrabDebris();
        CreateNewShieldPointPositions(shieldObjects.Count);

        Vector3[] startPositions = shieldObjects.Select(obj => obj.transform.position).ToArray();
        float time = 0f;

        while (time < shieldFormTime)
        {
            for (int i = 0; i < shieldObjects.Count; i++)
            {
                shieldObjects[i].transform.localPosition = Vector3.Lerp(startPositions[i], shieldPoints[i].position, time / shieldFormTime);
            }

            time += Time.deltaTime;
            yield return null;
        }

        moveShieldObjects = StartCoroutine(MoveShieldObjects());
    }

    // Make sure that the shield objects stay with the player and add a sine wave bob effect to them
    IEnumerator MoveShieldObjects()
    {
        for (int i = 0; i < shieldObjects.Count; i++)
        {
            shieldObjects[i].transform.parent = shieldPoints[i];
        }

        while (true)
        {
            for (int i = 0; i < shieldObjects.Count; i++)
            {
                /** Could be a cool effect where the shield is a little behind the player when running, but when would have to solve the problem of objects clipping through the player.
                   When the player is just moving forward, this would be easy. But if they turned around or stopped then the objects behind would have to take a different path to they
                   didn't move through the player. So at the moment, they are just parented.

                   shieldObjects[i].transform.position = Vector3.MoveTowards(shieldObjects[i].transform.position, shieldPoints[i].position, 5 * Time.deltaTime);
                **/

                ObjectBob.SineWaveBob(shieldObjects[i].gameObject, transform, wobblePosSpeed, wobblePosAmount, wobbleRotSpeed, wobblePosAmount);
            }

            yield return null;
        }
    }

    // Find points 
    List<Rigidbody> GrabDebris()
    {
        Vector3[] checkPoints = new Vector3[Random.Range(minShieldObjects, maxShieldObjects)];
        List<Rigidbody> debris = new List<Rigidbody>();

        Stack<Collider> launchableObjects = GetLaunchableObjects();

        for (int i = 0; i < checkPoints.Length; i++)
        {
            Rigidbody debrisObj = null;

            // Pick a launchable object from the launchableObjects stack
            if (launchableBias >= Random.Range(0.0f, 1.0f) && launchableObjects.Count != 0)
            {
                debrisObj = launchableObjects.Pop().attachedRigidbody;
            }
            // Fire a raycast down from a random position inside a sphere to check if we hit anything
            else
            {
                Vector3 point = (Random.insideUnitSphere * shieldPullRange) + transform.position;
                point.y = transform.position.y + playerHeight / 2;

                RaycastHit hit = RaycastSystem.Raycast(point, -Vector3.up, raycastDownFromCircleRange, shieldInteractionMask);
                Collider col = hit.collider;

                if (col)
                {
                    debrisObj = col.CompareTag(TagManager.LAUNCHABLE) ? col.GetComponent<Rigidbody>()  // Hit an object that can be launched
                                                                      : CreateRandomMesh(hit);         // Hit a mesh, need to cut an object out of it
                }
            }

            // debrisObj could be null if we didn't enter the launchableObjects if and the raycast didn't hit anything
            if (debrisObj)
            {
                debrisObj.detectCollisions = false;
                debrisObj.useGravity = false;

                debris.Add(debrisObj);
            }
        }

        return debris;
    }

    // Get all objects marked as Launchable within shieldPullRange. Make me think I should use layers instead of tags..
    Stack<Collider> GetLaunchableObjects()
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, shieldPullRange);
        return new Stack<Collider>(objects.Where(obj => obj.CompareTag(TagManager.LAUNCHABLE))); 
    }

    // Create a random mesh for forming the shield
    Rigidbody CreateRandomMesh(RaycastHit hit)
    {
        // We actually create two objects, one for the boolean operation and one to be part of the shield so they need to be the same size
        float meshSize = Random.Range(minRandomShieldObjectSize, maxRandomShieldObjectSize);

        // Create the object that the player will see / which will be thrown
        Rigidbody randomDebris = MeshCutoutCreator.CreateMesh(hit.point, meshSize, hit.collider.GetComponent<MeshRenderer>().sharedMaterials).AddComponent<Rigidbody>();
        randomDebris.tag = TagManager.LAUNCHABLE;

        if (actuallyCutMesh)
        {
            GameObject one = hit.collider.gameObject;                            // The object that will have the hole cut out of it
            GameObject two = MeshCutoutCreator.CreateMesh(hit.point, meshSize);  // The object that will be used to cut out the other object
            MeshCutter.DoOperation(MeshCutter.BoolOp.SubtractLR, one, two);      // Perform the boolean operation
        }
        else
        {
            // Shader magic
        }

        return randomDebris;
    }

    // Creates some random points that shield debris will anchor to
    void CreateNewShieldPointPositions(int amountOfDebris)
    {
        for (int i = 0; i < amountOfDebris; i++)
        {
            Vector3 pointPosition = (Random.insideUnitSphere * shieldSize) + transform.position;
            pointPosition.y = Random.Range(playerHeight / 2, playerHeight * 1.25f);

            shieldPoints[i].position = pointPosition;
        }
    }
}