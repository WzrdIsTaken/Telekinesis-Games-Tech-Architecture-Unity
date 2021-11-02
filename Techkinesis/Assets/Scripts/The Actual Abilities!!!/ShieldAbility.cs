using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Forms a shield infront of the player

public class ShieldAbility : MonoBehaviour
{
    public System.Action<int> TakeDamage;

    [SerializeField] ShieldCollider shieldCollider;       // The ShieldCollider gameobject
    [SerializeField, Range(0, 1)] float damageReduction;  // How much the shield reduces incoming damage. 1 = less reduction, 0 = more reduction

    [Space]
    [SerializeField] LayerMask shieldInteractionMask;     // What objects can be pulled to create the shield
    [SerializeField] float shieldPullRange;               // From how far away objects will be pulled to create the shield
    //[SerializeField] float minDistAwayFromPlayer;        // The minimum distance away from the player that objects will be pulled TODO
    [SerializeField, Range(0, 1)] float launchableBias;   // How much objects already tagged launchable will me prioritised with gathering debris. 1 = more bais, 0 = less bais

    [Space]
    [SerializeField] float shieldFormTime;                // How long it takes the shield to form to full size
    [SerializeField] float shieldSize;                    // How big the shield is
    [SerializeField] int minShieldObjects;                // The minimum objects that will be used to create a shield
    [SerializeField] int maxShieldObjects;                // The maximum objects that will be used to create a shield. NOTE: Not configurable at runtime as shieldPoints are pooled in Start

    [Space]
    [SerializeField] float minRandomShieldObjectSize;     // The minimum size object that are cut from the mesh will be
    [SerializeField] float maxRandomShieldObjectSize;     // The maximum size object that are cut from the mesh will be
    [SerializeField] bool actuallyCutMesh;                // WARNING! EXPENSIVE!! With current level of optimisation this is wayy to expensive

    [Space]
    //[SerializeField] float minDebrisDistFromPlayer;      // 
    //[SerializeField] float maxDebrisDistFromPlayer;      //

    [Space]
    [SerializeField] float wobblePosSpeed;                // How fast a held object will 'wobble' in the air
    [SerializeField] float wobblePosAmount;               // The maximum amount a held object can wobble
    [SerializeField] float wobbleRotSpeed;                // How fast a held object will rotate in the air
    [SerializeField] float wobbleRotAmount;               // The max amount a held object can rotate

    float playerHeight;
    float raycastDownFromCircleRange;                     // How far the raycasts will be shot down from points gathered in GrabDebris()

    List<Rigidbody> shieldObjects;
    List<Transform> shieldPoints = new List<Transform>();

    Coroutine formShield;
    Coroutine moveShieldObjects;

    // Create a pool of shieldPoints and setup shieldCollider
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

        shieldCollider.SetColliderState(false);
        shieldCollider.ObjectHitCollider += ObjectHitShield;
    }

    // Pass ShieldAbility the CharacterController as already been 'got' in PlayerController
    public void PassReferences(CharacterController controller)
    {
        playerHeight = controller.height;
        raycastDownFromCircleRange = playerHeight * 3f;
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

        shieldCollider.SetColliderState(false);

        DebugLogManager.Print("Shield down! Would make a nice sound.", DebugLogManager.OutputType.NOT_MY_JOB);
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

        shieldCollider.SetColliderState(true);
        moveShieldObjects = StartCoroutine(MoveShieldObjects());

        DebugLogManager.Print("Shield active! Would make a nice sound.", DebugLogManager.OutputType.NOT_MY_JOB);
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
                /** 
                   Could be a cool effect where the shield is a little behind the player when running, but when would have to solve the problem of objects clipping through the player.
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
        Stack<Collider> launchableObjects = GetLaunchableObjects();

        List<Rigidbody> debris = new List<Rigidbody>();

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
                    // Hit an object that can be launched : Hit an object, but not once that can be launched so need to cut the mesh
                    debrisObj = col.CompareTag(TagAndLayerNameManager.LAUNCHABLE) ? col.GetComponent<Rigidbody>() 
                                                                      : MeshCutter.CutAndReturnRandomMesh(hit, minRandomShieldObjectSize, maxRandomShieldObjectSize, actuallyCutMesh); 
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

        if (debris.Count == 0)
        {
            DebugLogManager.Print("No objects were in range. Would make a cool sound or something.", DebugLogManager.OutputType.NOT_MY_JOB);
        }

        return debris;
    }

    // Get all objects marked as Launchable within shieldPullRange. Make me think I should use layers instead of tags..
    Stack<Collider> GetLaunchableObjects()
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, shieldPullRange);
        return new Stack<Collider>(objects.Where(obj => obj.CompareTag(TagAndLayerNameManager.LAUNCHABLE))); 
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

    // Passes on damage via the TakeDamage event to PlayerController. Hooked up to the ObjectHitCollider event in ShieldCollider
    public void ObjectHitShield(Collider collider)
    {
        if (collider.CompareTag(TagAndLayerNameManager.ENEMY_PROJECTILE))
        {
            int damage = Mathf.RoundToInt(collider.GetComponent<EnemyProjectile>().GetDamage() * damageReduction);
            TakeDamage(damage);
        }
    }
}