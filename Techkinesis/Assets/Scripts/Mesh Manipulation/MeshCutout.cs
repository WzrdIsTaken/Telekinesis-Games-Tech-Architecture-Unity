using UnityEngine;
using System.Collections;

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
    MeshCutoutFace[] terrainFaces;

    public void GenerateCutout()
    {
        Initialize();
        GenerateMesh();
    }

    public void GenerateCutout(MeshCutoutSettings _settings, float lifeTime)
    {
        settings = _settings;

        Initialize();
        GenerateMesh();
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
        terrainFaces = new MeshCutoutFace[6];

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

            terrainFaces[i] = new MeshCutoutFace(meshCutoutGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
        }
    }

    void GenerateMesh()
    {
        foreach (MeshCutoutFace face in terrainFaces) face.ConstructMesh();
    }

    IEnumerator DestroyCutout(float lifeTime)
    {
        // Shrink the mesh's radius before it gets destroyed. The house rebuilds itself.

        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }
}