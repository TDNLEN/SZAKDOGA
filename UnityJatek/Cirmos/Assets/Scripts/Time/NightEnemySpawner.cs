using System.Collections.Generic;
using UnityEngine;

public class NightEnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    public GameObject zombiePrefab;
    public GameObject skeletonPrefab;

    [Header("Player")]
    public Transform player;

    [Header("Spawn Distances")]
    public float minSpawnDistance = 5f;
    public float maxSpawnDistance = 15f;

    [Header("Spawn Intervals")]
    public float zombieSpawnInterval = 5f;
    public float skeletonSpawnInterval = 10f;

    [Header("Limits")]
    public int maxAliveNightEnemies = 10;

    private float zombieTimer;
    private float skeletonTimer;

    private readonly List<GameObject> spawnedThisNight = new List<GameObject>();

    private void OnEnable()
    {
        NightEvents.OnDayStarted += HandleDayStarted;
        NightEvents.OnNightStarted += HandleNightStarted;
    }

    private void OnDisable()
    {
        NightEvents.OnDayStarted -= HandleDayStarted;
        NightEvents.OnNightStarted -= HandleNightStarted;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        ResetTimers();

        if (GameTime.Instance == null || !GameTime.Instance.IsNight)
        {
            CleanupDeadEntries();
            spawnedThisNight.Clear();
        }
    }

    private void Update()
    {
        if (GameTime.Instance == null) return;
        if (!GameTime.Instance.IsNight) return;
        if (player == null) return;

        CleanupDeadEntries();

        if (GetAliveEnemyCount() >= maxAliveNightEnemies)
            return;

        zombieTimer += Time.deltaTime;
        skeletonTimer += Time.deltaTime;

        if (zombiePrefab != null &&
            zombieTimer >= zombieSpawnInterval &&
            GetAliveEnemyCount() < maxAliveNightEnemies)
        {
            zombieTimer = 0f;
            SpawnEnemy(zombiePrefab);
        }

        if (skeletonPrefab != null &&
            skeletonTimer >= skeletonSpawnInterval &&
            GetAliveEnemyCount() < maxAliveNightEnemies)
        {
            skeletonTimer = 0f;
            SpawnEnemy(skeletonPrefab);
        }
    }

    private void HandleNightStarted()
    {
        ResetTimers();
    }

    private void HandleDayStarted()
    {
        for (int i = 0; i < spawnedThisNight.Count; i++)
        {
            if (spawnedThisNight[i] != null)
                Destroy(spawnedThisNight[i]);
        }

        spawnedThisNight.Clear();
        ResetTimers();
    }

    private void ResetTimers()
    {
        zombieTimer = 0f;
        skeletonTimer = 0f;
    }

    private void SpawnEnemy(GameObject prefab)
    {
        if (prefab == null) return;
        if (player == null) return;
        if (GetAliveEnemyCount() >= maxAliveNightEnemies) return;

        Vector3 spawnPos = GetRandomSpawnPositionAroundPlayer();
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
        spawnedThisNight.Add(enemy);
    }

    private Vector3 GetRandomSpawnPositionAroundPlayer()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;

        if (dir == Vector2.zero)
            dir = Vector2.right;

        float dist = Random.Range(minSpawnDistance, maxSpawnDistance);
        Vector3 pos = player.position + new Vector3(dir.x, dir.y, 0f) * dist;
        pos.z = 0f;

        return pos;
    }

    private int GetAliveEnemyCount()
    {
        CleanupDeadEntries();
        return spawnedThisNight.Count;
    }

    private void CleanupDeadEntries()
    {
        for (int i = spawnedThisNight.Count - 1; i >= 0; i--)
        {
            if (spawnedThisNight[i] == null)
                spawnedThisNight.RemoveAt(i);
        }
    }
}