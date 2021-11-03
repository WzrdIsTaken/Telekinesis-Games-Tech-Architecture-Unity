using UnityEngine;

// Allows for custom gizmos to be drawn. 

public class GizmoManager : MonoBehaviour
{
    [Tooltip("Toggles whether custom gizmos will be drawn")]
    [SerializeField] bool drawCustomGizmos;

    [Space]
    [Tooltip("Enter the tag and filepath of any custom gizmos you want to draw. " +
             "Gizmo images must be put in Assets/Gizmos")]
    [SerializeField] TagToFile[] gizmoIcons;

    void OnDrawGizmos()
    {
        if (!drawCustomGizmos) return;

        // This is well slow but idk a better way to do it right now
        foreach (TagToFile tagToFile in gizmoIcons)
        {
            GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tagToFile.tag);
            foreach (GameObject gO in objectsWithTag)
            {
                Gizmos.DrawIcon(gO.transform.position, tagToFile.fileName, true);
            }
        }
    }

    // Using a struct over a dictionary because we want this to be accessible in the inspector for a designer
    [System.Serializable]
    struct TagToFile
    {
        public string tag;
        public string fileName;
    }
}