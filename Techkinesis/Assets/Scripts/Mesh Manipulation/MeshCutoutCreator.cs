using UnityEngine;

// Manages the creation of mesh cutouts

public static class MeshCutoutCreator
{
    // Returns a created mesh gameobject
    public static GameObject CreateMesh(Vector3 position, float force, Material[] materials=null)
    {
        MeshCutout mesh = new GameObject("MeshCutout").AddComponent<MeshCutout>();
        mesh.GenerateCutout(CreateMeshSettings(force));

        // If no materials then the mesh will only be used for a cutout, therefore it doesn't need to be visable
        if (materials != null) mesh.GetComponent<MeshRenderer>().sharedMaterials = materials;
        else mesh.GetComponent<MeshRenderer>().enabled = false;

        mesh.transform.position = position;
        return mesh.gameObject;
    }

    // Create the MeshCutcutSettings used to control the shape of a mesh cutout
    static MeshCutoutSettings CreateMeshSettings(float force)
    {
        // The force will influence the size / noise of the shape
        // Maybe can have a more complex settings creation (eg with layers of noise, some sort of loop might be best) in the future

        MeshCutoutSettings meshCutoutSettings = ScriptableObject.CreateInstance<MeshCutoutSettings>();
        meshCutoutSettings.meshRadius = 1 * force;        // Mesh radius
        meshCutoutSettings.meshQualityReduction = 0.9f;   // Mesh quality reduction (1 = more reduction)
        meshCutoutSettings.noiseLayers = new MeshCutoutSettings.NoiseLayer[]
        {
            new MeshCutoutSettings.NoiseLayer
            (
                true,                                     // Enabled
                false,                                    // Use first layer as mask
                new NoiseSettings
                (
                    1,                                    // Number of layers
                    1,                                    // Strength
                    1,                                    // Base Roughness
                    2,                                    // Roughness
                    0.5f,                                 // Persistence
                    Vector3.zero,                         // Centre
                    0.1f                                  // MinValue
                )
            )
        };

        return meshCutoutSettings;
    }
}