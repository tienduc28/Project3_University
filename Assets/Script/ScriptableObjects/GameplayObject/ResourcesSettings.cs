using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Data", menuName = "Resource Data")]
public class ResourcesSettings : ScriptableObject
{
    public Object prefab;
    public float minNoiseValue;
    public float maxNoiseValue;
    [Range(0f, 1f)]
    public float spawnChance;
    public int checkRange;
    int halfCheckRange
    { get { return checkRange/2; } }

    public bool isNoiseInRange(float noiseValue)
    {
        return noiseValue >= minNoiseValue && noiseValue <= maxNoiseValue;
    }

    
}
