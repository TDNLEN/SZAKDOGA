using System;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteHouseSpawner : MonoBehaviour
{
    [Serializable]
    public class SpawnedHouseData
    {
        public string uniqueId;
        public int prefabIndex;
        public Vector3 position;
        public Vector2 footprintSize;
        public bool generated;
    }

    [Header("Refs")]
    public Transform player;

    [Header("House Prefabs")]
    public GameObject[] housePrefabs;

    [Header("Chunk Settings")]
    public float chunkWidth = 40f;
    public int loadRadius = 3;
    public int unloadExtra = 1;

    [Header("Rail")]
    public float railY = 0f;
    public float minDistanceFromRail = 5f;
    public float maxDistanceFromRail = 15f;

    [Header("Spawn Chance")]
    [Range(0f, 1f)] public float houseChancePerChunk = 0.75f;
    public int maxAttemptsPerChunk = 20;

    [Header("Overlap")]
    public LayerMask obstacleMask;
    public float extraSpacing = 1f;

    private readonly Dictionary<Vector2Int, SpawnedHouseData> chunkHouseData = new Dictionary<Vector2Int, SpawnedHouseData>();
    private readonly Dictionary<Vector2Int, GameObject> activeChunkHouses = new Dictionary<Vector2Int, GameObject>();

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;
        if (housePrefabs == null || housePrefabs.Length == 0) return;

        Vector2Int playerChunk = GetChunkCoord(player.position.x);

        for (int x = -loadRadius; x <= loadRadius; x++)
        {
            Vector2Int chunk = new Vector2Int(playerChunk.x + x, 0);

            EnsureChunkDataExists(chunk);
            EnsureChunkInstantiated(chunk);
        }

        List<Vector2Int> toRemove = new List<Vector2Int>();

        foreach (var kvp in activeChunkHouses)
        {
            Vector2Int chunk = kvp.Key;
            int distX = Mathf.Abs(chunk.x - playerChunk.x);

            if (distX > loadRadius + unloadExtra)
                toRemove.Add(chunk);
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            Vector2Int chunk = toRemove[i];

            if (activeChunkHouses.TryGetValue(chunk, out GameObject houseObj))
            {
                if (houseObj != null)
                    Destroy(houseObj);

                activeChunkHouses.Remove(chunk);
            }
        }
    }

    private Vector2Int GetChunkCoord(float worldX)
    {
        return new Vector2Int(Mathf.FloorToInt(worldX / chunkWidth), 0);
    }

    private void EnsureChunkDataExists(Vector2Int chunk)
    {
        if (chunkHouseData.ContainsKey(chunk))
            return;

        SpawnedHouseData data = GenerateHouseDataForChunk(chunk);
        chunkHouseData.Add(chunk, data);
    }

    private void EnsureChunkInstantiated(Vector2Int chunk)
    {
        if (!chunkHouseData.TryGetValue(chunk, out SpawnedHouseData data))
            return;

        if (!data.generated)
            return;

        if (activeChunkHouses.ContainsKey(chunk))
            return;

        GameObject prefab = housePrefabs[data.prefabIndex];
        if (prefab == null) return;

        GameObject house = Instantiate(prefab, data.position, Quaternion.identity);

        HouseUniqueId id = house.GetComponent<HouseUniqueId>();
        if (id == null)
            id = house.AddComponent<HouseUniqueId>();

        id.uniqueId = data.uniqueId;

        activeChunkHouses.Add(chunk, house);
    }

    private SpawnedHouseData GenerateHouseDataForChunk(Vector2Int chunk)
    {
        SpawnedHouseData data = new SpawnedHouseData();
        data.generated = false;

        int seed = chunk.x * 73856093 ^ 19349663;
        System.Random rng = new System.Random(seed);

        double roll = rng.NextDouble();
        if (roll > houseChancePerChunk)
            return data;

        for (int attempt = 0; attempt < maxAttemptsPerChunk; attempt++)
        {
            int prefabIndex = rng.Next(0, housePrefabs.Length);
            GameObject prefab = housePrefabs[prefabIndex];
            if (prefab == null) continue;

            Vector2 size = Vector2.one;
            HouseSpawnConfig config = prefab.GetComponentInChildren<HouseSpawnConfig>(true);
            if (config != null)
                size = config.footprintSize;

            float chunkMinX = chunk.x * chunkWidth;
            float chunkMaxX = chunkMinX + chunkWidth;

            float usableMinX = chunkMinX + size.x * 0.5f;
            float usableMaxX = chunkMaxX - size.x * 0.5f;

            if (usableMaxX <= usableMinX)
                continue;

            float rand01x = (float)rng.NextDouble();
            float x = Mathf.Lerp(usableMinX, usableMaxX, rand01x);

            int side = rng.NextDouble() < 0.5 ? -1 : 1;

            float minYOffset = minDistanceFromRail + size.y * 0.5f;
            float maxYOffset = maxDistanceFromRail + size.y * 0.5f;
            float rand01y = (float)rng.NextDouble();
            float yOffset = Mathf.Lerp(minYOffset, maxYOffset, rand01y);

            float y = railY + side * yOffset;

            Vector2 center = new Vector2(x, y);
            Vector2 checkSize = size + Vector2.one * extraSpacing;

            bool blocked = false;

            if (obstacleMask.value != 0)
            {
                Collider2D hit = Physics2D.OverlapBox(center, checkSize, 0f, obstacleMask);
                if (hit != null)
                    blocked = true;
            }

            if (!blocked)
            {
                Bounds candidate = new Bounds(center, new Vector3(checkSize.x, checkSize.y, 1f));

                foreach (var kvp in chunkHouseData)
                {
                    SpawnedHouseData other = kvp.Value;
                    if (!other.generated) continue;
                    if (kvp.Key == chunk) continue;

                    Bounds otherBounds = new Bounds(other.position, new Vector3(other.footprintSize.x + extraSpacing, other.footprintSize.y + extraSpacing, 1f));
                    if (otherBounds.Intersects(candidate))
                    {
                        blocked = true;
                        break;
                    }
                }
            }

            if (blocked)
                continue;

            data.generated = true;
            data.prefabIndex = prefabIndex;
            data.position = new Vector3(x, y, 0f);
            data.footprintSize = size;
            data.uniqueId = $"house_chunk_{chunk.x}_{chunk.y}";

            return data;
        }

        return data;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-1000f, railY, 0f), new Vector3(1000f, railY, 0f));
    }
}