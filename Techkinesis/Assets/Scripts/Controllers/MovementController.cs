using UnityEngine;

// Base class for anything that can move

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public abstract class MovementController : MonoBehaviour
{
    // TODO
    // I can't be asked to think of any shared functionality right now as its not in scope, so imma just remove everything that was here and move it all into PlayerController.
    // If when making the NPC controller (just needs to shoot / move a little really) and notice some easy code repetition then can think about moving stuff back over.

    protected CharacterController controller;  // I am using a CharacterController because it supports walking up stairs / slopes out of the box 
    protected Animator animator;

    public virtual void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }
}