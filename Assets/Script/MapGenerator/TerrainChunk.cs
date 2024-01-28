using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk
{
    const float colliderGenerationDistanceThreshold = 10f;
    float sqrColliderGenerationDistanceThreshold
    { get { return colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold; } }

    public event Action<TerrainChunk> OnTerrainChunkVisible;
    Transform viewer;
    MeshSettings meshSettings;
    HeightMapSettings heightMapSettings;
    Bounds bounds;
    public GameObject meshObject
    { get; private set; }
    public Vector2 sampleCenter
    { get; private set; }
    public float chunkSize
    { get; private set; }
    public float chunkUnscaleSize
    { get; private set; }
    public Vector2 chunkPosition
    { get; private set; }
    public HeightMap heightMap
    { get; private set; }

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    bool heightMapReceived;

    LodInfo[] lodInfoList;
    LodMesh[] lodMeshes;
    int colliderLodIndex;
    int preLodIndex = -1;
    float maxViewDistance;
    float sqrMaxViewDistance;
    float[,] fallOffMap;
    bool hasColliderSet;
    bool hasResourcesSpawned = false;
    public TerrainChunk(Vector2 chunkCoord, MeshSettings meshSettings, HeightMapSettings heightMapSettings, Transform parent, Transform viewer, Material mapMaterial, LodInfo[] lodInfoList, int colliderLodIndex, float[,] fallOffMap)
    {
        this.meshSettings = meshSettings;
        this.heightMapSettings = heightMapSettings;
        this.viewer = viewer;
        this.lodInfoList = lodInfoList;
        this.colliderLodIndex = colliderLodIndex;
        this.fallOffMap = fallOffMap;

        chunkUnscaleSize = meshSettings.meshUnscaledSize;
        chunkSize = meshSettings.meshWorldSize;
        chunkPosition = chunkCoord * meshSettings.meshWorldSize;
        sampleCenter = chunkPosition / meshSettings.scale;
        bounds = new Bounds(chunkPosition, Vector2.one * meshSettings.meshWorldSize);

        meshObject = new GameObject("Terrain Chunk");
        meshObject.layer = LayerMask.NameToLayer("Ground");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = mapMaterial;
        meshObject.transform.position = new Vector3(chunkPosition.x, 0, chunkPosition.y);
        meshObject.SetActive(false);
        meshObject.transform.parent = parent;

        lodMeshes = new LodMesh[lodInfoList.Length];
        for (int i = 0; i < lodMeshes.Length; i++)
        {
            lodMeshes[i] = new LodMesh(lodInfoList[i].lod);
            lodMeshes[i].onMeshReceiveCallback += UpdateChunk;
            if (i == colliderLodIndex)
                lodMeshes[i].onMeshReceiveCallback += UpdateCollisionMesh;
        }

        maxViewDistance = lodInfoList[lodInfoList.Length - 1].visibleDistanceThreshold;
        sqrMaxViewDistance = maxViewDistance * maxViewDistance;

    }

    Vector2 viewerPosition
    { get { return new Vector2(viewer.position.x, viewer.position.z); } }

    public void LoadHeightMap()
    {
        DataThreadRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.verticesPerLine, meshSettings.verticesPerLine, sampleCenter, heightMapSettings, fallOffMap), OnHeightMapReceived);
    }

    public void OnHeightMapReceived(object heightMapObject)
    {
        heightMap = (HeightMap)heightMapObject;
        heightMapReceived = true;
        UpdateChunk();
    }

    public void UpdateChunk()
    { 
        if (heightMapReceived)
        {
            float sqrDistanceToViewer = bounds.SqrDistance(viewerPosition);
            if (sqrDistanceToViewer <= sqrMaxViewDistance)
                SetVisisble(true);
            else
                SetVisisble(false);

            if (isChunkVisible())
            {
                if (OnTerrainChunkVisible != null)
                    OnTerrainChunkVisible(this);
                int lodIndex = 0;
                for (int i = 0; i < lodInfoList.Length - 1; i++)
                {
                    if (sqrDistanceToViewer > lodInfoList[i].sqrVisibleDistanceThreshold)
                        lodIndex = i + 1;
                    else
                        break;
                }

                if (lodIndex != preLodIndex)
                {
                    if (lodMeshes[lodIndex].hasMesh)
                    {
                        preLodIndex = lodIndex;
                        meshFilter.mesh = lodMeshes[lodIndex].mesh;
                    }
                    else if (!lodMeshes[lodIndex].hasRequestedMesh)
                        lodMeshes[lodIndex].RequestMeshData(heightMap, meshSettings);
                }
            }
        }
    }

    public void UpdateCollisionMesh()
    {
        if (!hasColliderSet)
        {
            float sqrDistanceToViewer = bounds.SqrDistance(viewerPosition);

            if (sqrDistanceToViewer < lodInfoList[colliderLodIndex].sqrVisibleDistanceThreshold)
                if (!lodMeshes[colliderLodIndex].hasRequestedMesh)
                    lodMeshes[colliderLodIndex].RequestMeshData(heightMap, meshSettings);

            //if (sqrDistanceToViewer < sqrColliderGenerationDistanceThreshold)
            //{
                if (lodMeshes[colliderLodIndex].hasMesh)
                {
                    meshCollider.sharedMesh = lodMeshes[colliderLodIndex].mesh;
                    hasColliderSet = true;
                    ResourcesSpawner.SpawnResources(this);
                }
            //}
            SetupSingleton.instance.surface.BuildNavMesh();
        }
    }

    public void SetVisisble(bool visisble)
    {
        if (!isChunkVisible())
        {
            meshObject.SetActive(visisble);
            //SetupSingleton.instance.surface.BuildNavMesh();
        }
    }

    public bool isChunkVisible()
    {
        return meshObject.activeSelf;
    }

    public void LoadWholeChunk()
    {
        DataThreadRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.verticesPerLine, meshSettings.verticesPerLine, sampleCenter, heightMapSettings, fallOffMap), OnHeightMapReceivedOnLoadWholeChunk);
    }

    public void OnHeightMapReceivedOnLoadWholeChunk(object heightMapObject)
    {
        heightMap = (HeightMap)heightMapObject;
        heightMapReceived = true;

        for (int  i = 0; i < lodInfoList.Length; i++)
        {
            if (!lodMeshes[i].hasRequestedMesh)
            {
                lodMeshes[i].RequestMeshData(heightMap, meshSettings);
                lodMeshes[i].hasRequestedMesh = true;
            }
        }
    }
}

public class LodMesh
{
    public Mesh mesh;
    public bool hasRequestedMesh;
    public bool hasMesh;
    int lod;
    public event System.Action onMeshReceiveCallback;

    public LodMesh(int lod)
    {
        this.lod = lod;
    }

    public void RequestMeshData(HeightMap heightMap, MeshSettings meshSettings)
    {
        hasRequestedMesh = true;
        DataThreadRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
    }

    void OnMeshDataReceived(object meshDataObject)
    {
        mesh = ((MeshData)meshDataObject).CreateMesh();
        hasMesh = true;
        onMeshReceiveCallback();
    }
}
