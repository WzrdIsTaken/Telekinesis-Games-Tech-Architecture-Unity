using UnityEngine;
using UnityMeshSimplifier;

// The base for creating a mesh cutout | Credit: Sebastian Lague (https://bit.ly/3v8XATN)

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
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

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

    void GenerateMesh()
    {
        foreach (MeshCutoutFace face in meshFaces) face.ConstructMesh();
    }

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

    void SimplifyMesh()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        Mesh sourceMesh = meshFilter.sharedMesh;

        MeshSimplifier meshSimplifier = new MeshSimplifier();
        meshSimplifier.Initialize(sourceMesh);

        meshSimplifier.SimplifyMesh(settings.meshQualityReduction);
        meshFilter.sharedMesh = meshSimplifier.ToMesh();
    }

    void GenerateCollider()
    {
        gameObject.AddComponent<MeshCollider>();
    }
}