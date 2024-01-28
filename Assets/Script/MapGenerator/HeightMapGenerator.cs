using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(int width, int height, Vector2 sampleCenter, HeightMapSettings heightMapSetting, float[,] fallOffMap)
    {
        AnimationCurve threadSafe_heightCurve = new AnimationCurve(heightMapSetting.heightCurve.keys);
        float[,] values = NoiseGenerator.GenerateNoiseMap(width, height, sampleCenter, heightMapSetting.noiseSettings);
        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

       for (int i = 0; i < width; i++)
            for (int j = 0; j < height; j++)
            {
                if (heightMapSetting.enableFallOff)
                    values[i, j] = Mathf.Clamp01(values[i, j] - fallOffMap[i, j]);
                values[i, j] = threadSafe_heightCurve.Evaluate(values[i, j]) * heightMapSetting.heightMultiplier;
                if (values[i, j] < minValue) minValue = values[i, j];
                if (values[i, j] > maxValue) maxValue = values[i, j];
            }

       return new HeightMap(values, minValue, maxValue);

    }
}



public struct HeightMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}