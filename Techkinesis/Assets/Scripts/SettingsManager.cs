using UnityEngine;
using UnityEngine.UI;

// Handles some basic settings to make nagivation of the scene easier

public class SettingsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Image crosshair;

    [Header("Cursor")]
    [SerializeField] bool lockCursor;

    [Header("Crosshair")]
    [SerializeField] Color crosshairColour;

    void OnValidate()
    {
        // Cursor
        Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockCursor;

        // Crosshair
        crosshair.color = crosshairColour;
    }
}