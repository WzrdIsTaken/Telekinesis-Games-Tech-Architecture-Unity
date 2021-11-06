using UnityEngine;
using System.Collections;

// Pulls and launches and rock or object

public class LaunchAbility : AbilityBase
{
    #region Variables editable in the inspector (for a designer)

    [Space]
    [Tooltip("The point to which the object is drawn towards")]
    [SerializeField] SpringJoint launchPullPoint;

    [Tooltip("What layers of objects can be pulled")]
    [SerializeField] LayerMask launchPullInteractionMask;

    [Tooltip("How far away objects can be")]
    [SerializeField] float launchRaycastRange;

    [Space]
    [Tooltip("The minimum size objects that are cut from the mesh will be")]
    [SerializeField] float minPulledObjectSize = 0.11f;  // WARNING! If actuallyCutMesh is enabled, having to big of an object size can cause.. interesting.. mesh slices! 

    [Tooltip("The maximum size objects that are cut from the mesh will be")]
    [SerializeField] float maxPulledObjectSize = 0.11f;  // 0.11 seems to be kinda a magic number lol. If actuallyCutMesh is enabled don't change unless the algorithm is optimised!

    [Tooltip("The minimum time it will take to pull an object out of a mesh")]
    [SerializeField] float minBreakTime;

    [Tooltip("The maximum time it will take to pull an object out of a mesh")]
    [SerializeField] float maxBreakTime;

    [Tooltip("[WARNING - EXPENSIVE] Determines whether the mesh will be cut when a random object is created to be launched")]
    [SerializeField] bool actuallyCutMesh;

    [Space]
    [Tooltip("How minimum speed an object will be pulled")]
    [SerializeField] float minPullSpeed;

    [Tooltip("How maximum speed an object will be pulled")]
    [SerializeField] float maxPullSpeed;

    [Space]
    [Tooltip("The minimum distance an object will float up before being pulled to the player")]
    [SerializeField] float minFloatUpDistance;

    [Tooltip("The maximum distance an object will float up before being pulled to the player")]
    [SerializeField] float maxFloatUpDistance;

    [Tooltip("The minimum amount an object will rotate before being pulled to the player")]
    [SerializeField] float minFloatUpRotation;

    [Tooltip("The maximum amount an object will rotate before being pulled to the player")]
    [SerializeField] float maxFloatUpRotation;

    [Tooltip("The minimum time an object will float up before being pulled to the player")]
    [SerializeField] float minFloatUpTime;

    [Tooltip("The maximum time an object will float up before being pulled to the player")]
    [SerializeField] float maxFloatUpTime;

    [Tooltip("The minimum time an object will just hover in the air before being pulled to the player")]
    [SerializeField] float minHoverTime;

    [Tooltip("The maximum time an object will just hover in the air before being pulled to the player")]
    [SerializeField] float maxHoverTime;

    [Space]
    [Tooltip("Will the held object use a spring joint to simulate its movement? " +
             "Note - Cool effect but will really bug out when levitating")]  // See PullObjectToPlayer()
    [SerializeField] bool useSpringJoint = false;

    [Tooltip("How fast a held object will 'wobble' in the air (only used while levitating)")]
    [SerializeField] float wobblePosSpeed;

    [Tooltip("The maximum amount a held object can wobble (only used while levitating)")]
    [SerializeField] float wobblePosAmount;

    [Tooltip("How fast a held object will rotate in the air (only used while levitating)")]
    [SerializeField] float wobbleRotSpeed;

    [Tooltip("The max amount a held object can rotate (only used while levitating)")]
    [SerializeField] float wobbleRotAmount;

    [Space]
    [Tooltip("What layers launched objects will perfom special interactions with (eg: damaging enemies)")]
    [SerializeField] LayerMask launchLaunchInteractionMask;

    [Tooltip("How minimum force the object will be launched with")]
    [SerializeField] float minLaunchForce;

    [Tooltip("The maximum force the object will be launched with")]
    [SerializeField] float maxLaunchForce;

    [Tooltip("A multiplier for damage applied to the damage = force * mass calculation")]
    [SerializeField] float damageMultiplier;

    #endregion

    Rigidbody heldObject;

    ThirdPersonCamera cam;
    ShieldAbility shieldAbility;

    // Recieve the player camera reference from PlayerController
    public void PassReferences(ThirdPersonCamera playerCamera, ShieldAbility _shieldAbility)
    {
        cam = playerCamera;
        shieldAbility = _shieldAbility;
    }

    // Grab an object to be launched
    protected override void AbilityStart()
    {
        // If the player is holding objects in their shield, then we want to throw those objects instead 
        if (shieldAbility.LaunchShieldObjects(cam.transform.forward)) 
        {
            // LaunchShieldObjects returns true if the shield is enabled

            return;
        }

        RaycastHit hit = RaycastSystem.Raycast(cam.transform.position, cam.transform.forward, launchRaycastRange, launchPullInteractionMask);
        Rigidbody selectedObject = null;

        // We didn't hit an object
        if (!hit.collider)
        {
            DebugLogManager.Print("You didn't hit an object! Would make a cool sound or something.", DebugLogManager.OutputType.NOT_MY_JOB);
            return;
        }

        // Hit an object that can be launched
        bool launchableObject = hit.collider.CompareTag(TagNameManager.LAUNCHABLE);
        if (launchableObject)
        {
            selectedObject = hit.collider.gameObject.GetComponent<Rigidbody>();
        }
        // Hit an object, but not once that can be launched so need to cut the mesh
        else if (hit.collider.CompareTag(TagNameManager.CUTTABLE))
        {
            selectedObject = MeshCutter.CutAndReturnRandomMesh(hit, minPulledObjectSize, maxPulledObjectSize, actuallyCutMesh);
        }
        
        StartCoroutine(PullObject(selectedObject, !launchableObject));

        DebugLogManager.Print("Launch pull active! Would make a cool sound or something.", DebugLogManager.OutputType.NOT_MY_JOB);
    }

    // Reset the launch system / the pulled object and launch it using throwForce
    protected override void AbilityEnd()
    {
        if (!heldObject) return;

        StopAllCoroutines();

        heldObject.transform.parent = null;
        launchPullPoint.connectedBody = null;
        heldObject.useGravity = true;

        float launchForce = Random.Range(minLaunchForce, maxLaunchForce);
        int damage = ProjectileManager.CalculateProjectileDamage(heldObject.mass, launchForce, damageMultiplier);
        ProjectileManager.SetupProjectile(heldObject.gameObject, launchLaunchInteractionMask, damage, false);

        if (!useSpringJoint) heldObject.isKinematic = false;
        heldObject.velocity = cam.transform.forward * launchForce;

        heldObject = null;

        DebugLogManager.Print("Launch object thrown! Would make a cool sound or something.", DebugLogManager.OutputType.NOT_MY_JOB);
    }

    // Perform relevant effects (depending if its already marked as launchable or not) on the heldObject then pull it towards launchPullPoint
    IEnumerator PullObject(Rigidbody selectedObject, bool cutObject)
    {
        heldObject = selectedObject;
        heldObject.useGravity = false;

        // If the object already exists, we want to move it up then towards the player. Else we want to do a cool breaking effect
        if (!cutObject)
        {
            // Move the object into the air
            yield return MoveObjectUp();

            // Wait x seconds before pulling it to the player
            yield return new WaitForSeconds(Random.Range(minHoverTime, maxHoverTime));
        }
        else
        {
            yield return BreakMeshEffect();
        }

        // Move the object towards launchPullPoint
        StartCoroutine(PullObjectToPlayer());
    }

    // Move the object up and apply some rotation
    IEnumerator MoveObjectUp()
    {
        Vector3 startPosition = heldObject.transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, startPosition.y + Random.Range(minFloatUpDistance, maxFloatUpDistance), startPosition.z);

        Quaternion startRotation = heldObject.transform.rotation;
        Quaternion targetRotation = new Quaternion(Random.Range(minFloatUpRotation, maxFloatUpRotation), Random.Range(minFloatUpRotation, maxFloatUpRotation),
                                                   Random.Range(minFloatUpRotation, maxFloatUpRotation), Random.Range(minFloatUpRotation, maxFloatUpRotation));

        float floatUpTimer = 0;
        float floatUpDuration = Random.Range(minFloatUpTime, maxFloatUpTime);
        while (floatUpTimer < floatUpDuration)
        {
            heldObject.transform.position = Vector3.Lerp(startPosition, targetPosition, floatUpTimer / floatUpDuration);
            heldObject.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, floatUpTimer / floatUpDuration);

            floatUpTimer += Time.deltaTime;
            yield return null;
        }

        heldObject.transform.position = targetPosition;
        heldObject.transform.rotation = targetRotation;
    }

    // Make it look like the mesh is breaking with some shader magic
    IEnumerator BreakMeshEffect()
    {
        // Effect would actually take place in MeshCutter -> CutAndReturnRandomMesh

        yield return new WaitForSeconds(Random.Range(minBreakTime, maxBreakTime));
    }

    // Move the object towards launchPullPoint via a lerp, then setup the object for 'carrying'
    IEnumerator PullObjectToPlayer()
    {
        Vector3 startPosition = heldObject.transform.position;

        float time = 0;
        float duration = CalculatePulledObjectLerpTime();
        while (time < duration)
        {
            float x = Mathf.SmoothStep(startPosition.x, launchPullPoint.transform.position.x, time / duration);
            float y = Mathf.SmoothStep(startPosition.y, launchPullPoint.transform.position.y, time / duration);
            float z = Mathf.SmoothStep(startPosition.z, launchPullPoint.transform.position.z, time / duration);
            heldObject.transform.position = Vector3.Lerp(startPosition, new Vector3(x, y, z), time / duration);
            // heldObject.transform.position = Vector3.Lerp(startPosition, launchPullPoint.transform.position, time / duration); I don't know which is better!

            time += Time.deltaTime;
            yield return null;
        }

        heldObject.transform.position = launchPullPoint.transform.position;
        heldObject.transform.parent = launchPullPoint.transform;

        /* Ok so here's the thing: Spring joint does not play nice with the levitation stuff because it uses rigidbody movement (at least I think thats the problem..)
           I really need to move onto other assignments, so my not-very-good-because-im-well-tired solution is just to allow the designer to disable the spring joint movement.
           With more time I think I could:
            - 1) Understand why the problem is really happening. If I was better at math I probally could use cap the springs max distance / velocity in a nice way
            - 2) If rigidbodies really are the problem rewrite the LevitationAbility to use transform movement instead and just mimic forces 'well enough'
            - 3) Have a nice system for transitioning between spring and sine wave movment
        */

        if (!useSpringJoint)
        {
            heldObject.isKinematic = true;
            StartCoroutine(MakeObjectWobble());
        }
        else
        {
            launchPullPoint.connectedBody = heldObject;
        }
    }

    // The downside with using lerp is that whatever the distance, the time of travel is the same. Therefore, we need to change the lerp time to take distance into account 
    float CalculatePulledObjectLerpTime()
    {
        float distance = Vector3.Distance(heldObject.transform.position, launchPullPoint.transform.position);
        return Mathf.Max(0.1f, distance / Random.Range(minPullSpeed, maxPullSpeed));
    }

    // Make the object wobble around in the air once it has reached launchPullPoint using a sine wave
    IEnumerator MakeObjectWobble()
    {
        while (true)
        {
            ObjectBob.SineWaveBob(heldObject.gameObject, transform, wobblePosSpeed, wobblePosAmount, wobbleRotSpeed, wobbleRotAmount);

            yield return null;
        }
    }
}

/** 
   Note: Objects which are already marked as launchable do not have their collisions with the player disabled after they are thrown.
   This is intentional! The idea being that objects in the scene marked as launchable are part of the world, so would therefore
   want to have collisions. Smaller objects which would be annoying to collide with but are marked as launchable would have their
   collisions disabled already.
**/

/***
    Originally I had this code inside PullObjectToPlayer. It worked great.. until you wanted to move the object towards the player at high speed. 
    Now I have a solution with lerp. Honestly I like the way lerp feels more, I scrapped the sine wave stuff for the 'feel' as well, but if its ever needed 
    here is the old rigidbody code:

    [Tooltip("How fast the object will be pulled")]
    [SerializeField] float pullForce;

    [Tooltip("The maximum velocity which an object can be pulled")]
    [SerializeField] float pulledObjectMaxVelocity;

    // Pull the object towards launchPullPoint
    IEnumerator PullObjectToPlayer()
    {
        float selectedObjectSize = heldObject.GetComponent<MeshRenderer>().bounds.size.y / 2;  // Needed so we know when to stop the coroutine

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

                heldObject.velocity = Vector3.zero;
                heldObject.angularVelocity = Vector3.zero;

                StartCoroutine(MakeObjectWobble());

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
***/