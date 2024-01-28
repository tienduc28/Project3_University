using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainLoader : MonoBehaviour
{
    public int minWorldSize = 100;
    int chunkPerDimension;
    Vector2 topLeftChunkCoord;
    Vector2 bottomRightChunkCoord;

    const float chunkUpdateDistanceThreshold = 25f;
    float sqrChunkUpdateDistanceThreshold
    { get { return chunkUpdateDistanceThreshold * chunkUpdateDistanceThreshold; } }

    public Material mapMaterial;
    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public int colliderLodIndex;
    public LodInfo[] lodInfoList;

    float chunkSize;
    int chunkVisibleInViewDistance;
    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    List<TerrainChunk> visibleChunkList = new List<TerrainChunk>();

    public Transform viewer;
    Vector2 viewerPosition;
    Vector2 viewerOldPosition;

    float[,] fallOffMap;
    private void Start()
    {
        chunkSize = meshSettings.meshWorldSize;
        chunkPerDimension = Mathf.CeilToInt(minWorldSize / chunkSize);
        int topLeftXChunkCoor = (chunkPerDimension - 1) / -2;
        int topLeftYChunkCoor = (chunkPerDimension - 1) / 2; ;
        int bottomRightXChunkCoor = topLeftXChunkCoor + chunkPerDimension - 1;
        int bottomRightYChunkCoor = topLeftYChunkCoor - chunkPerDimension + 1; 
        topLeftChunkCoord = new Vector2(topLeftXChunkCoor, topLeftYChunkCoor);
        bottomRightChunkCoord = new Vector2(bottomRightXChunkCoor, bottomRightYChunkCoor);

        fallOffMap = FalloffGenerator.generateFalloffMap((int)(chunkPerDimension * (meshSettings.verticesPerLine - 3) + 3), heightMapSettings.fallOffRange, heightMapSettings.fallOffOffset);
        GenerateWorld();

        float viewDistance = lodInfoList[lodInfoList.Length - 1].visibleDistanceThreshold;
        chunkVisibleInViewDistance = Mathf.CeilToInt(viewDistance / chunkSize);
        viewerOldPosition = viewerPosition;

        UpdateVisibleChunk();
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.transform.position.x, viewer.transform.position.z);

        if (viewerPosition != viewerOldPosition)
        {
            foreach (TerrainChunk chunk in visibleChunkList)
            {
                chunk.UpdateCollisionMesh();
            }
        }

        if ((viewerPosition - viewerOldPosition).sqrMagnitude >= sqrChunkUpdateDistanceThreshold )
        {
            viewerOldPosition = viewerPosition;
            UpdateVisibleChunk();
        }
    }

    bool isChunkInWorld(Vector2 coord)
    {
        if (coord.x >= topLeftChunkCoord.x && coord.x <= bottomRightChunkCoord.x && coord.y >= bottomRightChunkCoord.y && coord.y <= topLeftChunkCoord.y)
            return true;
        else
            return false;
    }

    public void UpdateVisibleChunk()
    {
        for (int i = 0; i < visibleChunkList.Count; i++)
            visibleChunkList[i].SetVisisble(false);
        visibleChunkList.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunkVisibleInViewDistance; yOffset <= chunkVisibleInViewDistance; yOffset++)
            for (int xOffset = -chunkVisibleInViewDistance; xOffset <= chunkVisibleInViewDistance; xOffset++)
            {
                Vector2 chunkCoord = new Vector2(xOffset + currentChunkCoordX, yOffset + currentChunkCoordY);
                if (isChunkInWorld(chunkCoord))
                {
                    if (terrainChunkDictionary.ContainsKey(chunkCoord))
                        terrainChunkDictionary[chunkCoord].UpdateChunk();
                    else
                    {
                        TerrainChunk terrainChunk = new TerrainChunk(chunkCoord, meshSettings, heightMapSettings, transform, viewer, mapMaterial, lodInfoList, colliderLodIndex, FalloffGenerator.GetFallOffMapAtCoord(fallOffMap, topLeftChunkCoord, chunkCoord, meshSettings.verticesPerLine));
                        terrainChunkDictionary.Add(chunkCoord, terrainChunk);
                        terrainChunk.OnTerrainChunkVisible += AddChunkToVisibleList;
                        terrainChunk.LoadHeightMap();
                    }
                }
            }
    }

    public void AddChunkToVisibleList(TerrainChunk terrainChunk)
    {
        visibleChunkList.Add(terrainChunk);
    }

    public void GenerateWorld()
    {
        for (int i = 0; i < chunkPerDimension; i++)
            for (int j = 0; j < chunkPerDimension; j++)
            {
                Vector2 chunkCoord = new Vector2(topLeftChunkCoord.x + i, topLeftChunkCoord.y - j);
                TerrainChunk terrainChunk = new TerrainChunk(chunkCoord, meshSettings, heightMapSettings, transform, viewer, mapMaterial, lodInfoList, colliderLodIndex, FalloffGenerator.GetFallOffMapAtCoord(fallOffMap, topLeftChunkCoord, chunkCoord, meshSettings.verticesPerLine));
                terrainChunkDictionary.Add(chunkCoord, terrainChunk);
                terrainChunk.OnTerrainChunkVisible += AddChunkToVisibleList;
                terrainChunk.LoadWholeChunk();
            }
    }
}

[System.Serializable]
public struct LodInfo
{
    [Range(0, MeshSettings.numberOfSupportedLod - 1)]
    public int lod;
    public float visibleDistanceThreshold;

    public float sqrVisibleDistanceThreshold
    { get { return visibleDistanceThreshold * visibleDistanceThreshold; } }
}