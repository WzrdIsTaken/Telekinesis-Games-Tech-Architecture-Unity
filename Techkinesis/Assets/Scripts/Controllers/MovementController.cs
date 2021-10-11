using UnityEngine;

// Base class for anything that can move

public class MovementController : MonoBehaviour
{
    [SerializeField, RequireInterface(typeof(IInputProvider))] ScriptableObject inputProvider;
    IInputProvider InputProvider => inputProvider as IInputProvider;

    void Start()
    {
        InputProvider.OnJump += Jump;
    }

    void Update()
    {
        InputState inputState = InputProvider.GetState();

        print(inputState);
    }

    void Jump()
    {

    }
}