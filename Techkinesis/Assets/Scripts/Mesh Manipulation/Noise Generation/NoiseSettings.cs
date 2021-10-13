using UnityEngine;

// Holds data needed for the creation of noise | Credit: Sebastian Lague (https://bit.ly/3v8XATN)

[System.Serializable]
public class NoiseSettings
{
    [Range(1, 8)] public int numLayers = 1;

    public float strength = 1;
    public float baseRoughness = 1;
    public float roughness = 2;
    public float persistence = 0.5f;
    public Vector3 centre;
    public float minValue;

    public NoiseSettings(int _numLayers, float _strength, float _baseRoughness, float _roughness, float _persistence, Vector3 _centre, float _minValue)
    {
        numLayers = _numLayers;

        strength = _strength;
        baseRoughness = _baseRoughness;
        roughness = _roughness;
        persistence = _persistence;
        centre = _centre;
        minValue = _minValue;
    }
}