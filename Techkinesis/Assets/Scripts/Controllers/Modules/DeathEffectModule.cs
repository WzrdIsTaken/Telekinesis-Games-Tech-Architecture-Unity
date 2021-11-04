using UnityEngine;

// 'Explosive' death effect for any object that is made up of multiple parts

public class DeathEffectModule : MonoBehaviour
{
    #region Variables editable in the inspector (for a designer)

    [Tooltip("The minimum force that will be applied on the horizontal axis")]
    [SerializeField] float minHorizontalForce;

    [Tooltip("The maximum force that will be applied on the horizontal axis")]
    [SerializeField] float maxHorizontalForce;

    [Tooltip("The minimum force that will be applied on the vertical axis")]
    [SerializeField] float minVerticalForce;

    [Tooltip("The maximum force that will be applied on the vertical axis")]
    [SerializeField] float maxVerticalForce;

    #endregion

    // Searches for the Armature transform (where humanoid bodies limbs are held) then adds the explosive effect
    public void HumanoidDeath (Animator animator)
    {
        animator.enabled = false;

        foreach (Transform child in transform.Find("Armature").GetComponentsInChildren<Transform>())
        {
            AddRigidbodyAndForce(child.gameObject);
        }

        Destroy(gameObject, 10f);  // TODO: Could have a cool fade out effect. 10 seconds is just an arbitrary number
    }

    // Add rigidbodies to all the parts of the object, then shoot them all in different directions! Expensive but fun 
    void AddRigidbodyAndForce(GameObject gO)
    {
        float xForce = Random.Range(minHorizontalForce, maxHorizontalForce);
        float yForce = Random.Range(minVerticalForce, maxVerticalForce);
        float zForce = Random.Range(minHorizontalForce, maxHorizontalForce);

        gO?.AddComponent<Rigidbody>().AddForce(xForce, yForce, zForce, ForceMode.Impulse);
    }
}