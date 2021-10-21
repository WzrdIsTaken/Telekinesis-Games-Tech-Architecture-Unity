using UnityEngine;
using UnityMeshSimplifier;

// The base for creating a mesh cutout

public class MeshCutout : MonoBehaviour
{
    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;

    public MeshCutoutSettings settings;

    MeshCutoutGenerator meshCutoutGenerator;

    [HideInInspector]
    public bool meshCutoutSettingsFoldout;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    MeshCutoutFace[] meshFaces;

    public void GenerateCutout()                               // Used in the editor, called from the 'Generate Cutout' button
    {
        Initialize();
        GenerateMesh();
    }

    public void GenerateCutout(MeshCutoutSettings _settings)   // Used at runtime, called from MeshCutoutCreator
    {
        settings = _settings;

        Initialize();
        GenerateMesh();
        CombineMeshes();
        SimplifyMesh();
        GenerateCollider();
    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate) GenerateCutout();
    }

    // Step 1 - Create the meshes that will make up the cutout | Credit: Sebastian Lague (https://bit.ly/3v8XATN)
    void Initialize()
    {
        meshCutoutGenerator = new MeshCutoutGenerator(settings);

        if (meshFilters == null || meshFilters.Length == 0) meshFilters = new MeshFilter[6];
        meshFaces = new MeshCutoutFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }

            meshFaces[i] = new MeshCutoutFace(meshCutoutGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    // Step 2 - Apply noise to the mesh faces
    void GenerateMesh()
    {
        foreach (MeshCutoutFace face in meshFaces) face.ConstructMesh();
    }

    // Step 3 - Combine the 6 individual meshes into one 
    void CombineMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            Destroy(meshFilters[i].gameObject);
        }

        gameObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
        MeshFilter cutoutFilter = gameObject.AddComponent<MeshFilter>();

        cutoutFilter.mesh = new Mesh();
        cutoutFilter.mesh.CombineMeshes(combine);
    }

    // Step 4 - Simplify the mesh to make boolean operations play nice
    void SimplifyMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh sourceMesh = meshFilter.sharedMesh;

        MeshSimplifier meshSimplifier = new MeshSimplifier();
        meshSimplifier.Initialize(sourceMesh);

        meshSimplifier.SimplifyMesh(settings.meshQualityReduction);
        meshFilter.sharedMesh = meshSimplifier.ToMesh();
    }

    // Step 5 - Add a collider needed for CSG operations
    void GenerateCollider()
    {
        gameObject.AddComponent<MeshCollider>().convex = true;

        // I don't know what issues settings convex to be true will cause. 
        // If its something major, can just create a convex collider for cutting and a non convex one to add the rigidbody to
        // "Non-convex MeshCollider with non-kinematic Rigidbody is no longer supported since Unity 5."
    }
}