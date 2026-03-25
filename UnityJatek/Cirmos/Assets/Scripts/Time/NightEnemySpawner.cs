using System.Collections.Generic;
using UnityEngine;

public class NightEnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject zombiePrefab;
    public GameObject skeletonPrefab;

    [Header("Spawn")]
    public Transform[] spawnPoints;
    public int zombiesPerNight = 5;
    public int skeletonsPerNight = 3;

    [Header("Optional random area spawn")]
    public bool useRandomAroundPlayer = false;
    public Transform player;
    public float minSpawnDistance = 12f;
    public float maxSpawnDistance = 22f;

    private readonly List<GameObject> spawnedThisNight = new List<GameObject>();

    private void OnEnable()
    {
        NightEvents.OnNightStarted += HandleNightStarted;
        NightEvents.OnDayStarted += HandleDayStarted;
    }

    private void OnDisable()
    {
        NightEvents.OnNightStarted -= HandleNightStarted;
        NightEvents.OnDayStarted -= HandleDayStarted;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (GameTime.Instance != null && GameTime.Instance.IsNight)
            HandleNightStarted();
    }

    private void HandleNightStarted()
    {
        SpawnGroup(zombiePrefab, zombiesPerNight);
        SpawnGroup(skeletonPrefab, skeletonsPerNight);
    }

    private void HandleDayStarted()
    {
        for (int i = 0; i < spawnedThisNight.Count; i++)
        {
            if (spawnedThisNight[i] != null)
                Destroy(spawnedThisNight[i]);
        }

        spawnedThisNight.Clear();
    }

    private void SpawnGroup(GameObject prefab, int count)
    {
        if (prefab == null || count <= 0) return;

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPos = GetSpawnPosition();
            GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
            spawnedThisNight.Add(enemy);
        }
    }

    private Vector3 GetSpawnPosition()
    {
        if (useRandomAroundPlayer && player != null)
        {
            Vector2 dir = Random.insideUnitCircle.normalized;
            float dist = Random.Range(minSpawnDistance, maxSpawnDistance);
            Vector3 pos = player.position + new Vector3(dir.x, dir.y, 0f) * dist;
            pos.z = 0f;
            return pos;
        }

        if (spawnPoints != null && spawnPoints.Length > 0)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];
            if (point != null)
                return point.position;
        }

        return transform.position;
    }
}