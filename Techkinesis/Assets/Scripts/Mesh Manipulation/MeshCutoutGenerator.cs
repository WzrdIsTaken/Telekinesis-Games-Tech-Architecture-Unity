using UnityEngine;

// Creates noise for the distortion of mesh cutouts | Credit: Sebastian Lague (https://bit.ly/3v8XATN)

public class MeshCutoutGenerator
{
    MeshCutoutSettings settings;
    NoiseFilter[] noiseFilters;

    public MeshCutoutGenerator(MeshCutoutSettings _settings)
    {
        settings = _settings;
        noiseFilters = new NoiseFilter[settings.noiseLayers.Length];
        for (int i = 0; i < noiseFilters.Length; i++)
        {
            noiseFilters[i] = new NoiseFilter(settings.noiseLayers[i].noiseSettings);
        }
    }

    public Vector3 CalculatePointOnPlanet(Vector3 pointOnUnitSphere)
    {
        float firstLayerValue = 0;
        float elevation = 0;

        if (noiseFilters.Length > 0)
        {
            firstLayerValue = noiseFilters[0].Evaluate(pointOnUnitSphere);
            if (settings.noiseLayers[0].enabled) elevation = firstLayerValue;
        }

        for (int i = 1; i < noiseFilters.Length; i++)
        {
            if (settings.noiseLayers[i].enabled)
            {
                float mask = (settings.noiseLayers[i].useFirstLayerAsMask) ? firstLayerValue : 1;
                elevation += noiseFilters[i].Evaluate(pointOnUnitSphere) * mask;
            }
        }
        return pointOnUnitSphere * settings.meshRadius * (1 + elevation);
    }
}