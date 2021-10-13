using UnityEngine;

// Uses the Noise class to create noise based off the given settings | Credit: Sebastian Lague (https://bit.ly/3v8XATN)

public class NoiseFilter
{
    NoiseSettings settings;
    Noise noise = new Noise();

    public NoiseFilter(NoiseSettings _settings)
    {
        settings = _settings;
    }

    public float Evaluate(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;

        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = noise.Evaluate(point * frequency + settings.centre);
            noiseValue += (v + 1) * .5f * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
        return noiseValue * settings.strength;
    }
}