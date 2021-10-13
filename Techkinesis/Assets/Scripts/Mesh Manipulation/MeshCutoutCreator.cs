using UnityEngine;

// Actually creates the meshcutout 5head

public class MeshCutoutCreator : MonoBehaviour
{
    void Start()
    {
        CreateMesh(Vector3.zero, 100, 1);
    }

    public void CreateMesh(Vector3 position, float lifeTime, float force)
    {
        MeshCutout mesh = new GameObject("MeshCutout").AddComponent<MeshCutout>();
        mesh.GenerateCutout(CreateMeshSettings(force), lifeTime);
    }

    MeshCutoutSettings CreateMeshSettings(float force)
    {
        // The force will influence the size / noise of the shape
        // Maybe can have a more complex settings creation (eg with layers of noise, some sort of loop might be best) in the future

        MeshCutoutSettings meshCutoutSettings = new MeshCutoutSettings
        (
            1,                                       // Mesh radius
            new MeshCutoutSettings.NoiseLayer[]
            {
                new MeshCutoutSettings.NoiseLayer
                (
                    true,                            // Enabled
                    false,                           // Use first layer as mask
                    new NoiseSettings
                    (
                        1,                           // Number of layers
                        1,                           // Strength
                        1,                           // Base Roughness
                        2,                           // Roughness
                        0.5f,                        // Persistence
                        Vector3.zero,                // Centre
                        0.1f                         // MinValue
                    )
                )
            }
        );

        return meshCutoutSettings;
    }
}