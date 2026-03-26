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

    private readonly List<GameObject> spawnedHouses = new List<GameObject>();

    private void Start()
    {
        SpawnHouses();
    }

    private void SpawnHouses()
    {
        if (housePrefabs == null || housePrefabs.Length == 0) return;

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
            spawnedHouses.Add(house);
            spawned++;
        }
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
            float yOffset = Random.Range(minDistanceFromRail, maxDistanceFromRail);
            float y = railY + side * yOffset;

            Vector2 center = new Vector2(x, y);
            Vector2 checkSize = size + Vector2.one * extraSpacing;

            Collider2D hit = Physics2D.OverlapBox(center, checkSize, 0f, obstacleMask);
            if (hit != null)
                continue;

            spawnPos = new Vector3(x, y, 0f);
            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 a = new Vector3(minX, railY, 0f);
        Vector3 b = new Vector3(maxX, railY, 0f);
        Gizmos.DrawLine(a, b);
    }
}