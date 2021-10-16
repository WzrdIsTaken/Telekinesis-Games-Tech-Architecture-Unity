using UnityEngine;

// Handles some basic settings to make nagivation of the scene easier

public class SettingsManager : MonoBehaviour
{
    [SerializeField] bool lockCursor;

    void Start()
    {
        // Cursor
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;
    }
}