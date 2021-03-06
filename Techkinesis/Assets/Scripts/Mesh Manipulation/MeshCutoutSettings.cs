using UnityEngine;

// Holds data needed for the creation of a mesh cutout | Credit: Sebastian Lague (https://bit.ly/3v8XATN)

[CreateAssetMenu(fileName = "MeshCutoutSettings", menuName = "ScriptableObjects/Debugging/MeshCutoutSettings", order = 1)] 
public class MeshCutoutSettings : ScriptableObject
{
    public float meshRadius = 1, meshQualityReduction = 0.5f;
    public NoiseLayer[] noiseLayers;

    public MeshCutoutSettings(float _meshRadius, float _meshQualityReduction, NoiseLayer[] _noiseLayers)
    {
        meshRadius = _meshRadius;
        meshQualityReduction = _meshQualityReduction;
        noiseLayers = _noiseLayers;
    }

    [System.Serializable]
    public class NoiseLayer
    {
        public bool enabled = true;
        public bool useFirstLayerAsMask;
        public NoiseSettings noiseSettings;

        public NoiseLayer(bool _enabled, bool _useFirstLayerAsMask, NoiseSettings _noiseSettings)
        {
            enabled = _enabled;
            useFirstLayerAsMask = _useFirstLayerAsMask;
            noiseSettings = _noiseSettings;
        }
    }
}