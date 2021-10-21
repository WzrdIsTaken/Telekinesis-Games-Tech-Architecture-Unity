using UnityEngine;

// Manages the creation of mesh cutouts

public static class MeshCutoutCreator
{
    public static GameObject CreateMesh(Vector3 position, float force, Material[] materials=null)
    {
        MeshCutout mesh = new GameObject("MeshCutout").AddComponent<MeshCutout>();
        mesh.GenerateCutout(CreateMeshSettings(force));

        if (materials != null) mesh.GetComponent<MeshRenderer>().sharedMaterials = materials;

        mesh.transform.position = position;
        return mesh.gameObject;
    }

    static MeshCutoutSettings CreateMeshSettings(float force)
    {
        // The force will influence the size / noise of the shape
        // Maybe can have a more complex settings creation (eg with layers of noise, some sort of loop might be best) in the future

        MeshCutoutSettings meshCutoutSettings = ScriptableObject.CreateInstance<MeshCutoutSettings>();
        meshCutoutSettings.meshRadius = 1 * force;        // Mesh radius
        meshCutoutSettings.meshQualityReduction = 0.9f;   // Mesh quality reduction 
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