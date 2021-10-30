using UnityEngine;
using UnityEngine.UI;

// Handles some basic settings to make nagivation of the scene easier. Can be changed at runtime.

public class SettingsManager : MonoBehaviour
{
    [Header("References")]  // No touching!
    [SerializeField] Image crosshair; 

    [Header("Cursor")]
    [SerializeField] bool lockCursor;        // Should the cursor be locked?

    [Header("Crosshair")]
    [SerializeField] Color crosshairColour;  // What colour the crosshair is

    void OnValidate()
    {
        // Cursor
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;

        // Crosshair
        crosshair.color = crosshairColour;
    }
}