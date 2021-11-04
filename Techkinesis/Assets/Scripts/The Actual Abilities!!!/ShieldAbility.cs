using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Forms a shield infront of the player

public class ShieldAbility : MonoBehaviour
{
    public System.Action<int> TakeDamage;  // Used in PlayerController

    #region Variables editable in the inspector (for a designer)

    [Tooltip("The ShieldCollider gameobject")]
    [SerializeField] ShieldCollider shieldCollider;

    [Tooltip("How much the shield reduces incoming damage. 1 = less reduction, 0 = more reduction")]
    [SerializeField, Range(0, 1)] float damageReduction;

    [Space]
    [Tooltip("What objects can be pulled to create the shield")]
    [SerializeField] LayerMask shieldPullInteractionMask;

    [Tooltip("From how far away objects will be pulled to create the shield")]
    [SerializeField] float shieldPullRange;  

    [Tooltip("How much objects already tagged launchable will me prioritised with gathering debris. 1 = more bais, 0 = less bias")]
    [SerializeField, Range(0, 1)] float launchableBias;

    [Space]
    [Tooltip("How long it takes the shield to form to full size")]
    [SerializeField] float shieldFormTime;

    [Tooltip("How big the shield is")]
    [SerializeField] float shieldSize;

    [Tooltip("The minimum objects that will be used to create a shield")]
    [SerializeField] int minShieldObjects; 

    [Tooltip("The maximum objects that will be used to create a shield. NOTE: Not configurable at runtime as shieldPoints are pooled in Start")]
    [SerializeField] int maxShieldObjects; 

    [Space]
    [Tooltip("The minimum size objects that are cut from the mesh will be")]
    [SerializeField] float minRandomShieldObjectSize;

    [Tooltip("The maximum size objects that are cut from the mesh will be")]
    [SerializeField] float maxRandomShieldObjectSize; 

    [Tooltip("[WARNING - EXPENSIVE (With current level of optimisation this is wayy to expensive..)] " +
             "Determines whether the mesh will be cut when a random object is created for the sheild")]
    [SerializeField] bool actuallyCutMesh;  

    [Space]
    [Tooltip("How fast a held object will 'wobble' in the air")]
    [SerializeField] float wobblePosSpeed; 

    [Tooltip("The maximum amount a held object can wobble")]
    [SerializeField] float wobblePosAmount;

    [Tooltip("How fast a held object will rotate in the air")]
    [SerializeField] float wobbleRotSpeed;   

    [Tooltip("The max amount a held object can rotate")]
    [SerializeField] float wobbleRotAmount;

    [Space]
    [Tooltip("What layers launched shield objects will perfom special interactions with (eg: damaging enemies)")]
    [SerializeField] LayerMask shieldLaunchInteractionMask;

    [Tooltip("The minimum force a shield object will be launched")]
    [SerializeField] float minShieldObjectLaunchForce;

    [Tooltip("The maximum force a shield object will be launched")]
    [SerializeField] float maxShieldObjectLaunchForce;

    [Tooltip("A multiplier for damage applied to the damage = force * mass calculation")]
    [SerializeField] float damageMultiplier;

    #endregion

    float playerHeight;
    float raycastDownFromCircleRange;  // How far the raycasts will be shot down from points gathered in GrabDebris()

    List<Rigidbody> shieldObjects = new List<Rigidbody>();
    List<Transform> shieldPoints = new List<Transform>();

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
        StartCoroutine(FormShield());
    }

    // Stop the shield. Called from an event hooked up in PlayerController Start
    public void ShieldEnd()
    {
        StopAllCoroutines();

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
        StartCoroutine(MoveShieldObjects());

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
                    
                    Or maybe spring joints on all of the bodies with their connectedBody being the player? Idk dude... so many cool possibilities but I gotta move onto other assignments...
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

                RaycastHit hit = RaycastSystem.Raycast(point, -Vector3.up, raycastDownFromCircleRange, shieldPullInteractionMask);
                Collider col = hit.collider;

                if (col)
                {
                    // Hit an object that can be launched
                    if (col.CompareTag(TagNameManager.LAUNCHABLE))
                    {
                        debrisObj = col.GetComponent<Rigidbody>();
                    }
                    // Hit an object, but not once that can be launched so need to cut the mesh
                    else if (col.CompareTag(TagNameManager.CUTTABLE))
                    {
                        debrisObj = MeshCutter.CutAndReturnRandomMesh(hit, minRandomShieldObjectSize, maxRandomShieldObjectSize, actuallyCutMesh);
                    }
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
        return new Stack<Collider>(objects.Where(obj => obj.CompareTag(TagNameManager.LAUNCHABLE))); 
    }

    // Creates some random points that shield debris will anchor to
    void CreateNewShieldPointPositions(int amountOfDebris)
    {
        for (int i = 0; i < amountOfDebris; i++)
        {
            Vector3 pointPosition = (Random.insideUnitSphere * shieldSize) + transform.position;
            pointPosition.y = Random.Range(transform.position.y + playerHeight / 2, transform.position.y + playerHeight * 1.25f);

            shieldPoints[i].position = pointPosition;
        }
    }

    // Passes on damage via the TakeDamage event to PlayerController. Hooked up to the ObjectHitCollider event in ShieldCollider
    public void ObjectHitShield(int baseDamage)
    {
        int damage = Mathf.RoundToInt(baseDamage * damageReduction);
        TakeDamage(damage);
    }

    // Called from LaunchAbility. Throws shield objects in direction by min/max ShieldObjectLaunchForce. Returns true/false depending if the shield is currenly active
    public bool LaunchShieldObjects(Vector3 direction)
    {
        if (shieldObjects.Count == 0) return false;

        List<Rigidbody> objectsInShield = new List<Rigidbody>(shieldObjects);
        ShieldEnd();

        foreach (Rigidbody obj in objectsInShield)
        {
            float launchForce = Random.Range(minShieldObjectLaunchForce, maxShieldObjectLaunchForce);
            int damage = ProjectileManager.CalculateProjectileDamage(obj.mass, launchForce, damageMultiplier);

            ProjectileManager.SetupProjectile(obj.gameObject, shieldLaunchInteractionMask, damage, false);
            obj.AddForce(direction * launchForce);
        }

        return true;
    }
}