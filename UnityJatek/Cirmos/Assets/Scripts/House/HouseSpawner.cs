using System.Collections.Generic;
using UnityEngine;

public class HouseSpawner : MonoBehaviour
{
    [Header("House Prefabs")]
    public GameObject[] housePrefabs;

    [Header("Spawn Range X")]
    public float minX = -50f;
    public float maxX = 300f;

    [Header("Rail")]
    public float railY = 0f;
    public float minDistanceFromRail = 5f;
    public float maxDistanceFromRail = 15f;

    [Header("Count")]
    public int houseCount = 15;
    public int maxAttemptsPerHouse = 30;

    [Header("Overlap")]
    public LayerMask obstacleMask;
    public float extraSpacing = 1f;

    private readonly List<Bounds> reservedHouseAreas = new List<Bounds>();

    private void Start()
    {
        Debug.Log("HouseSpawner Start lefutott");

        if (housePrefabs == null || housePrefabs.Length == 0)
        {
            Debug.LogWarning("HouseSpawner: nincs beállítva house prefab!");
            return;
        }

        SpawnHouses();
    }

    private void SpawnHouses()
    {
        int spawned = 0;
        int globalAttempts = 0;
        int maxGlobalAttempts = houseCount * maxAttemptsPerHouse;

        while (spawned < houseCount && globalAttempts < maxGlobalAttempts)
        {
            globalAttempts++;

            GameObject prefab = housePrefabs[Random.Range(0, housePrefabs.Length)];
            if (prefab == null) continue;

            Vector3 pos;
            Vector2 size;

            bool found = TryFindPositionForHouse(prefab, out pos, out size);
            if (!found) continue;

            GameObject house = Instantiate(prefab, pos, Quaternion.identity);

            HouseUniqueId id = house.GetComponent<HouseUniqueId>();
            if (id == null)
                id = house.AddComponent<HouseUniqueId>();

            id.uniqueId = System.Guid.NewGuid().ToString();

            Bounds b = new Bounds(pos, new Vector3(size.x + extraSpacing, size.y + extraSpacing, 1f));
            reservedHouseAreas.Add(b);

            spawned++;
            Debug.Log("Ház spawnolva ide: " + pos + " | id: " + id.uniqueId);
        }

        Debug.Log("Összes spawnolt ház: " + spawned);
    }

    private bool TryFindPositionForHouse(GameObject prefab, out Vector3 spawnPos, out Vector2 size)
    {
        spawnPos = Vector3.zero;
        size = Vector2.one;

        HouseSpawnConfig config = prefab.GetComponentInChildren<HouseSpawnConfig>(true);
        if (config != null)
            size = config.footprintSize;

        for (int i = 0; i < maxAttemptsPerHouse; i++)
        {
            float x = Random.Range(minX, maxX);

            int side = Random.value < 0.5f ? -1 : 1;

            float minYOffset = minDistanceFromRail + size.y * 0.5f;
            float maxYOffset = maxDistanceFromRail + size.y * 0.5f;
            float yOffset = Random.Range(minYOffset, maxYOffset);

            float y = railY + side * yOffset;

            Vector2 center = new Vector2(x, y);
            Vector2 checkSize = size + Vector2.one * extraSpacing;

            Bounds candidate = new Bounds(center, new Vector3(checkSize.x, checkSize.y, 1f));
            bool overlapsSpawnedHouse = false;

            for (int j = 0; j < reservedHouseAreas.Count; j++)
            {
                if (reservedHouseAreas[j].Intersects(candidate))
                {
                    overlapsSpawnedHouse = true;
                    break;
                }
            }

            if (overlapsSpawnedHouse)
                continue;

            if (obstacleMask.value != 0)
            {
                Collider2D hit = Physics2D.OverlapBox(center, checkSize, 0f, obstacleMask);
                if (hit != null)
                    continue;
            }

            spawnPos = new Vector3(x, y, 0f);
            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(minX, railY, 0f), new Vector3(maxX, railY, 0f));
    }
}