using UnityEngine;

// Allows for custom gizmos to be drawn

public class GizmoManager : MonoBehaviour
{
    [Tooltip("Toggles whether custom gizmos will be drawn")]
    [SerializeField] bool drawCustomGizmos;

    [Space]
    [Tooltip("Enter the tag and filepath of any custom gizmos you want to draw. " +  // Tag of the gameobject(s), filepath of the image (including the extension)
             "Gizmo images must be put in Assets/Gizmos")]
    [SerializeField] GizmoData[] gizmoIcons;

    void OnDrawGizmos()
    {
        if (!drawCustomGizmos) return;

        // This is well slow but idk a better way to do it right now
        foreach (GizmoData gizmoData in gizmoIcons)
        {
            if (!gizmoData.showGizmo) return;

            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(gizmoData.tag);
            foreach (GameObject gO in objectsWithTag)
            {
                Gizmos.DrawIcon(gO.transform.position, gizmoData.fileName, true);
            }
        }
    }

    // Using a struct over a dictionary because we want this to be accessible in the inspector for a designer. And now because we have more than 2 variables!
    [System.Serializable]
    struct GizmoData
    {
        [Tooltip("The tag that gameobjects have to have for the gizmo to display on")]
        public string tag;

        [Tooltip("The filepath of the gizmo icon. Must be in the gizmos folder. You don't need the root directory. " +
                 "Eg: Gizmos/CoolGizmoIcon.png -> CoolGizmoIcon.png ")]
        public string fileName;

        [Tooltip("Allows you to toggle on / off specific gizmos")]
        public bool showGizmo;
    }
}