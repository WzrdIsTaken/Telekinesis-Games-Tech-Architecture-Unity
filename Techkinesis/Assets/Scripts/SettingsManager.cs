using UnityEngine;
using UnityEngine.UI;

// Handles some basic settings to make nagivation of the scene easier. Can be changed at runtime.

public class SettingsManager : MonoBehaviour
{
    [Header("References"), Tooltip("No touching!")]
    [SerializeField] Image crosshair;

    [Header("Cursor"), Tooltip("Should the cursor be locked?")]
    [SerializeField] bool lockCursor;

    [Header("Crosshair"), Tooltip("What colour the crosshair should be?")]
    [SerializeField] Color crosshairColour;

    [Header("Show Debug Messages"), Tooltip("Should messages from DebugLogManager be outputted?")]
    [SerializeField] bool outputDebugMessages;

    void OnValidate()
    {
        // Cursor
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;

        // Crosshair
        crosshair.color = crosshairColour;

        // Messages
        DebugLogManager.SetOutputMessages(outputDebugMessages);
    }
}