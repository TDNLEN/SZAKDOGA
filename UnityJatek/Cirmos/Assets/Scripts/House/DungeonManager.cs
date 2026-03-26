using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;

    [Header("Refs")]
    public Transform player;
    public GameObject dungeonRoot;
    public Transform dungeonPlayerSpawn;
    public Transform exitPoint;

    [Header("Dungeon Spawn Points")]
    public Transform[] itemSpawnPoints;
    public Transform[] enemySpawnPoints;

    [Header("Prefabs")]
    public GameObject[] itemPrefabs;
    public GameObject[] enemyPrefabs;

    [Header("Counts")]
    public int itemsPerRun = 2;
    public int enemiesPerRun = 3;

    private Vector3 returnPosition;
    private readonly List<GameObject> spawnedDungeonObjects = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.Find("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        if (dungeonRoot != null)
            dungeonRoot.SetActive(false);
    }

    public void EnterDungeonFromHouse(HouseEntrance house)
    {
        if (player == null || dungeonRoot == null || dungeonPlayerSpawn == null) return;

        returnPosition = player.position;

        ClearDungeon();

        dungeonRoot.SetActive(true);
        player.position = dungeonPlayerSpawn.position;

        SpawnDungeonItems();
        SpawnDungeonEnemies();
    }

    public void ExitDungeon()
    {
        if (player == null) return;

        ClearDungeon();

        if (dungeonRoot != null)
            dungeonRoot.SetActive(false);

        player.position = returnPosition;
    }

    private void SpawnDungeonItems()
    {
        if (itemPrefabs == null || itemPrefabs.Length == 0) return;
        if (itemSpawnPoints == null || itemSpawnPoints.Length == 0) return;

        List<int> freeIndexes = new List<int>();
        for (int i = 0; i < itemSpawnPoints.Length; i++)
            freeIndexes.Add(i);

        int spawnCount = Mathf.Min(itemsPerRun, itemSpawnPoints.Length);

        for (int i = 0; i < spawnCount; i++)
        {
            int pick = Random.Range(0, freeIndexes.Count);
            int spawnIndex = freeIndexes[pick];
            freeIndexes.RemoveAt(pick);

            GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            if (prefab == null) continue;

            GameObject obj = Instantiate(prefab, itemSpawnPoints[spawnIndex].position, Quaternion.identity);
            spawnedDungeonObjects.Add(obj);
        }
    }

    private void SpawnDungeonEnemies()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;
        if (enemySpawnPoints == null || enemySpawnPoints.Length == 0) return;

        List<int> freeIndexes = new List<int>();
        for (int i = 0; i < enemySpawnPoints.Length; i++)
            freeIndexes.Add(i);

        int spawnCount = Mathf.Min(enemiesPerRun, enemySpawnPoints.Length);

        for (int i = 0; i < spawnCount; i++)
        {
            int pick = Random.Range(0, freeIndexes.Count);
            int spawnIndex = freeIndexes[pick];
            freeIndexes.RemoveAt(pick);

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            if (prefab == null) continue;

            GameObject obj = Instantiate(prefab, enemySpawnPoints[spawnIndex].position, Quaternion.identity);
            spawnedDungeonObjects.Add(obj);
        }
    }

    private void ClearDungeon()
    {
        for (int i = spawnedDungeonObjects.Count - 1; i >= 0; i--)
        {
            if (spawnedDungeonObjects[i] != null)
                Destroy(spawnedDungeonObjects[i]);
        }

        spawnedDungeonObjects.Clear();
    }
}
