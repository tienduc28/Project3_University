using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesSpawner : MonoBehaviour
{
    public static ResourcesSpawner instance;
    public NoiseSettings noiseSettings;
    public ResourcesSettings[] resourcesSettings;

    const float verticalOffset = 5.0f;

    private void Awake()
    {
        instance = FindObjectOfType<ResourcesSpawner>();
    }
    public static void SpawnResources(TerrainChunk terrainChunk)
    {
        int noiseMapSize = terrainChunk.heightMap.values.GetLength(0) - 2;
        float[,] noise = NoiseGenerator.GenerateNoiseMap(noiseMapSize, noiseMapSize, terrainChunk.sampleCenter, instance.noiseSettings);

        for (int i = 0; i < noiseMapSize; i += 10)
        {
            for (int j = 0; j < noiseMapSize; j += 10) 
            {
                for (int k = 0; k < instance.resourcesSettings.GetLength(0); k++)
                    if (instance.resourcesSettings[k].isNoiseInRange(noise[i, j]))
                    {
                        RandomlySpawn(instance.resourcesSettings[k], terrainChunk, new Vector2(i, j));
                    }

            }
        }
    }

    public static void RandomlySpawn(ResourcesSettings resources, TerrainChunk terrainChunk, Vector2 indexes)
    {
        Debug.Log(resources.name);
        int horizontalIndexFrom = (int)Mathf.Max(indexes.x - resources.checkRange / 2, 0);
        int horizontalIndexTo = (int)Mathf.Min(indexes.x + resources.checkRange / 2, terrainChunk.heightMap.values.GetLength(0) - 2);
        int verticalIndexFrom = (int)Mathf.Max(indexes.y - resources.checkRange / 2, 0);
        int verticalIndexTo = (int)Mathf.Min(indexes.y + resources.checkRange / 2, terrainChunk.heightMap.values.GetLength(0) - 2);
        for (int i = horizontalIndexFrom; i < horizontalIndexTo; i++)
        {
            for (int j = verticalIndexFrom; j < verticalIndexTo; j++)
            {
                if (UtilFunc.Roll(resources.spawnChance))
                {
                    Vector3 spawnPosition = GetPositionFromIndex(terrainChunk.heightMap.values, new Vector2(i, j), terrainChunk.chunkPosition, terrainChunk.chunkUnscaleSize, terrainChunk.chunkSize);
                    Instantiate(resources.prefab, spawnPosition, Quaternion.identity, terrainChunk.meshObject.transform);
                }
            }
        }
    }

    static Vector3 GetPositionFromIndex(float[,] heightMap, Vector2 indices, Vector2 chunkPosition, float chunkUnscaleSize, float chunkSize)
    {
        float positionX = (indices.x - chunkUnscaleSize / 2) / chunkUnscaleSize * chunkSize + chunkPosition.x;
        float positionZ = -(indices.y - chunkUnscaleSize / 2) / chunkUnscaleSize * chunkSize + chunkPosition.y;
        Vector3 checkPosition = new Vector3(positionX, heightMap[(int)indices.x + 1, (int)indices.y + 1] + verticalOffset / 2, positionZ);
        RaycastHit hit;
        if (Physics.Raycast(checkPosition, Vector3.down, out hit, verticalOffset))
            return hit.point;
        else
            return Vector3.zero;
    }
}
