using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// A very !(generic / powerful / well done) class but its out of scope / only needed to aid in a visual showcase of the abilities in the demo scene

public class DemoNPCController : MovementController<ControllerDataBase>, IProjectileInteraction  // Can just use ControllerDataBase here. No need to make a DemoNPCController data class
{
    [Header("Behavior")]
    [Tooltip("What the NPC will do in the demo scene")] 
    [SerializeField] NPCBehaviour npcBehaviour;
    enum NPCBehaviour { DO_NOTHING, SHOOT, MOVE };

    [Header("Shooting - Only required if NPCBehaviour is SHOOT")]
    [Tooltip("The layer that the player is on")]
    [SerializeField] LayerMask playerLayer;

    [Tooltip("Where the bullet will come from. The direction of this is where the bullet will shoot")]
    [SerializeField] Transform shootPoint;

    [Tooltip("The bullet prefab that the enemy will shoot")]
    [SerializeField] GameObject bullet;

    [Tooltip("How fast the bullet will travel")]
    [SerializeField] float bulletVelocity;

    [Tooltip("How fast the enemy will shoot")]
    [SerializeField] float shootTime;

    [Tooltip("How much damage the bullet will deal")]
    [SerializeField] int damage;

    [Header("Movement - Only required if NPCBehaviour is MOVE")]
    [Tooltip("Where the NPC will walk. Note: The NPC's start position is automatically added to the array")]  
    [SerializeField] List<Transform> movePoints;

    [Tooltip("How fast the enemy will move between points")]
    [SerializeField] float moveSpeed;

    [Tooltip("What state the move animation will be. 1 = faster, 0 = slower")]
    [SerializeField, Range(0, 1)] float animationSpeed;

    DemoSceneManager demoSceneManager;

    public override void Start()
    {
        base.Start();

        demoSceneManager = FindObjectOfType<DemoSceneManager>();

        switch (npcBehaviour)
        {
            case NPCBehaviour.DO_NOTHING:
                break;
            case NPCBehaviour.SHOOT:
                if (shootPoint == null)
                {
                    Debug.LogError(gameObject.name + " - If the NPCBehaviour is set to SHOOT then shootPoint cannot be null!");
                }

                StartCoroutine(Shoot());
                break;
            case NPCBehaviour.MOVE:
                if (movePoints.Count == 0)
                {
                    Debug.LogError(gameObject.name + " - If the NPCBehaviour is set to MOVE then movePoints cannot be empty!");
                }

                StartCoroutine(Move());
                break;
            default:
                Debug.LogError("The NPCBehaviour " + npcBehaviour.ToString() + " does not exist!");
                break;
        }
    }

    IEnumerator Shoot()
    {
        float timer = 0;

        while (true)
        {
            timer += Time.deltaTime;

            if (timer > shootTime)
            {
                GameObject projectile = Instantiate(bullet, shootPoint.position, Quaternion.identity);
                projectile.GetComponent<Projectile>().Setup(playerLayer, damage, true);

                projectile.transform.LookAt(shootPoint);
                projectile.GetComponent<Rigidbody>().AddForce(shootPoint.transform.forward * bulletVelocity, ForceMode.Force);

                timer = 0;
            }

            yield return null;
        }
    }

    IEnumerator Move()
    {
        // Add the positions to the actualMovePoints (because the movePoint transforms are parented)
        // If we don't do this check the the transform will be added again everytime the npc is respawned. Won't make a difference, but still..
        if (!movePoints.Contains(transform))  
        {
            movePoints.Insert(0, transform);
        }
        Vector3[] actualMovePoints = movePoints.Select(point => point.transform.position).ToArray();

        // The actual move loop
        int currentTarget = 1;
        Vector3 targetPosition = actualMovePoints[currentTarget];
        
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            transform.transform.LookAt(targetPosition);
            animator.SetFloat("speedPercent", animationSpeed);

            if (Vector3.Distance(transform.position, targetPosition) == 0)
            {
                currentTarget = currentTarget + 1 < actualMovePoints.Length ? currentTarget + 1 : 0;
                targetPosition = actualMovePoints[currentTarget];
            }

            yield return null;
        }
    }

    public override void TakeDamage(int damage)
    {
        if (healthModule.Damage(damage))
        {
            StopAllCoroutines();
            demoSceneManager.ResetNPC(this);
            GetComponent<DeathEffectModule>().HumanoidDeath(animator);
        }
    }
}