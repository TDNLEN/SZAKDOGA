using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public static DungeonManager Instance;

    [System.Serializable]
    public class DungeonObjectState
    {
        public DungeonSpawnedObject.SpawnKind kind;
        public string prefabName;
        public int spawnPointIndex;
        public bool removed;
    }

    [System.Serializable]
    public class HouseDungeonState
    {
        public string houseId;
        public bool initialized = false;
        public List<DungeonObjectState> objects = new List<DungeonObjectState>();
    }

    [Header("Refs")]
    public Transform player;
    public GameObject dungeonRoot;
    public Transform dungeonPlayerSpawn;
    public Transform exitPoint;

    [Header("Dungeon Spawn Points")]
    public Transform[] itemSpawnPoints;
    public Transform[] enemySpawnPoints;

    [Header("Fallback Prefabs")]
    public GameObject[] itemPrefabs;
    public GameObject[] enemyPrefabs;

    [Header("Door Audio")]
    public AudioSource audioSource;
    public AudioClip doorSound;
    [Range(0f, 1f)] public float doorVolume = 1f;

    private Vector3 returnPosition;

    private readonly List<DungeonSpawnedObject> activeDungeonObjects = new List<DungeonSpawnedObject>();
    private readonly Dictionary<string, HouseDungeonState> savedStates = new Dictionary<string, HouseDungeonState>();

    private string currentHouseId = null;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
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

    public void EnterDungeonFromHouse(HouseEntrance house, DungeonConfig config)
    {
        if (player == null)
        {
            Debug.LogError("DungeonManager: Player nincs beállítva.");
            return;
        }

        if (dungeonRoot == null)
        {
            Debug.LogError("DungeonManager: DungeonRoot nincs beállítva.");
            return;
        }

        if (dungeonPlayerSpawn == null)
        {
            Debug.LogError("DungeonManager: DungeonPlayerSpawn nincs beállítva.");
            return;
        }

        if (config == null)
        {
            Debug.LogError("DungeonManager: A ház dungeonConfig mezõje nincs beállítva.");
            return;
        }

        HouseUniqueId idComp = house.GetHouseUniqueId();
        if (idComp == null || string.IsNullOrEmpty(idComp.uniqueId))
        {
            Debug.LogError("DungeonManager: A háznak nincs HouseUniqueId-ja!");
            return;
        }

        Debug.Log("Belépett ház ID: " + idComp.uniqueId);
        currentHouseId = idComp.uniqueId;
        returnPosition = player.position;

        void TeleportAction()
        {
            ClearActiveDungeonSceneOnly();

            dungeonRoot.SetActive(true);
            player.position = dungeonPlayerSpawn.position;

            if (!savedStates.ContainsKey(currentHouseId))
            {
                HouseDungeonState newState = CreateInitialState(config, currentHouseId);
                savedStates.Add(currentHouseId, newState);
            }

            LoadHouseState(savedStates[currentHouseId]);

            if (MusicManager.Instance != null)
                MusicManager.Instance.SetDungeonState(true);
        }

        PlayDoorSound();

        if (ScreenFader.Instance != null)
            ScreenFader.Instance.FadeOutIn(TeleportAction, 0.6f, 0.15f, 0.6f);
        else
            TeleportAction();
    }

    public void ExitDungeon()
    {
        if (player == null)
        {
            Debug.LogError("Player nincs beállítva a DungeonManagerben!");
            return;
        }

        void TeleportBackAction()
        {
            SaveCurrentDungeonState();

            ClearActiveDungeonSceneOnly();

            if (dungeonRoot != null)
                dungeonRoot.SetActive(false);

            player.position = returnPosition;

            if (MusicManager.Instance != null)
                MusicManager.Instance.SetDungeonState(false);
        }

        PlayDoorSound();

        if (ScreenFader.Instance != null)
            ScreenFader.Instance.FadeOutIn(TeleportBackAction, 0.6f, 0.15f, 0.6f);
        else
            TeleportBackAction();
    }
    public void MarkDungeonObjectRemoved(DungeonSpawnedObject marker)
    {
        if (marker == null) return;
        if (string.IsNullOrEmpty(marker.houseId)) return;

        if (!savedStates.TryGetValue(marker.houseId, out HouseDungeonState state))
            return;

        for (int i = 0; i < state.objects.Count; i++)
        {
            DungeonObjectState saved = state.objects[i];

            if (saved.kind == marker.kind &&
                saved.prefabName == marker.prefabName &&
                saved.spawnPointIndex == marker.spawnPointIndex)
            {
                saved.removed = true;
                break;
            }
        }

        activeDungeonObjects.Remove(marker);
    }
    private void PlayDoorSound()
    {
        if (audioSource == null || doorSound == null)
            return;

        audioSource.PlayOneShot(doorSound, doorVolume);
    }

    private HouseDungeonState CreateInitialState(DungeonConfig config, string houseId)
    {
        HouseDungeonState state = new HouseDungeonState();
        state.houseId = houseId;
        state.initialized = true;

        List<int> itemIndexes = new List<int>();
        for (int i = 0; i < itemSpawnPoints.Length; i++)
            itemIndexes.Add(i);

        int itemCount = Mathf.Min(config.itemCount, itemSpawnPoints.Length);
        for (int i = 0; i < itemCount; i++)
        {
            int pick = Random.Range(0, itemIndexes.Count);
            int spawnIndex = itemIndexes[pick];
            itemIndexes.RemoveAt(pick);

            GameObject prefab = config.possibleItemPrefabs[Random.Range(0, config.possibleItemPrefabs.Length)];
            if (prefab == null) continue;

            state.objects.Add(new DungeonObjectState
            {
                kind = DungeonSpawnedObject.SpawnKind.Item,
                prefabName = prefab.name,
                spawnPointIndex = spawnIndex,
                removed = false
            });
        }

        List<int> enemyIndexes = new List<int>();
        for (int i = 0; i < enemySpawnPoints.Length; i++)
            enemyIndexes.Add(i);

        int enemyCount = Mathf.Min(config.enemyCount, enemySpawnPoints.Length);
        for (int i = 0; i < enemyCount; i++)
        {
            int pick = Random.Range(0, enemyIndexes.Count);
            int spawnIndex = enemyIndexes[pick];
            enemyIndexes.RemoveAt(pick);

            GameObject prefab = config.possibleEnemyPrefabs[Random.Range(0, config.possibleEnemyPrefabs.Length)];
            if (prefab == null) continue;

            state.objects.Add(new DungeonObjectState
            {
                kind = DungeonSpawnedObject.SpawnKind.Enemy,
                prefabName = prefab.name,
                spawnPointIndex = spawnIndex,
                removed = false
            });
        }

        return state;
    }

    private void LoadHouseState(HouseDungeonState state)
    {
        foreach (DungeonObjectState objState in state.objects)
        {
            if (objState.removed) continue;

            GameObject prefab = FindPrefabByName(objState.kind, objState.prefabName);
            if (prefab == null)
            {
                Debug.LogWarning("Nem található prefab: " + objState.prefabName);
                continue;
            }

            Transform spawnPoint = null;

            if (objState.kind == DungeonSpawnedObject.SpawnKind.Item)
            {
                if (objState.spawnPointIndex < 0 || objState.spawnPointIndex >= itemSpawnPoints.Length) continue;
                spawnPoint = itemSpawnPoints[objState.spawnPointIndex];
            }
            else
            {
                if (objState.spawnPointIndex < 0 || objState.spawnPointIndex >= enemySpawnPoints.Length) continue;
                spawnPoint = enemySpawnPoints[objState.spawnPointIndex];
            }

            if (spawnPoint == null) continue;

            Vector3 spawnPos = spawnPoint.position;
            if (objState.kind == DungeonSpawnedObject.SpawnKind.Item)
                spawnPos += Vector3.up * 0.1f;

            GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

            DungeonSpawnedObject marker = obj.GetComponent<DungeonSpawnedObject>();
            if (marker == null)
                marker = obj.AddComponent<DungeonSpawnedObject>();

            marker.kind = objState.kind;
            marker.houseId = state.houseId;
            marker.prefabName = objState.prefabName;
            marker.spawnPointIndex = objState.spawnPointIndex;
            marker.collectedByPlayer = false;
            marker.permanentlyRemoved = false;

            activeDungeonObjects.Add(marker);
        }
    }

    private void SaveCurrentDungeonState()
    {
        if (string.IsNullOrEmpty(currentHouseId)) return;
        if (!savedStates.ContainsKey(currentHouseId)) return;

        HouseDungeonState state = savedStates[currentHouseId];

        for (int i = 0; i < state.objects.Count; i++)
        {
            DungeonObjectState saved = state.objects[i];
            bool foundAlive = false;

            for (int j = 0; j < activeDungeonObjects.Count; j++)
            {
                DungeonSpawnedObject marker = activeDungeonObjects[j];
                if (marker == null) continue;

                if (marker.kind == saved.kind &&
                    marker.prefabName == saved.prefabName &&
                    marker.spawnPointIndex == saved.spawnPointIndex &&
                    marker.houseId == state.houseId)
                {
                    foundAlive = true;

                    if (marker.kind == DungeonSpawnedObject.SpawnKind.Item && marker.collectedByPlayer)
                        saved.removed = true;

                    break;
                }
            }

            if (!foundAlive)
                saved.removed = true;
        }
    }

    private void ClearActiveDungeonSceneOnly()
    {
        for (int i = activeDungeonObjects.Count - 1; i >= 0; i--)
        {
            DungeonSpawnedObject marker = activeDungeonObjects[i];

            if (marker == null)
            {
                activeDungeonObjects.RemoveAt(i);
                continue;
            }

            if (marker.kind == DungeonSpawnedObject.SpawnKind.Item && marker.collectedByPlayer)
            {
                activeDungeonObjects.RemoveAt(i);
                continue;
            }

            Destroy(marker.gameObject);
            activeDungeonObjects.RemoveAt(i);
        }
    }

    private GameObject FindPrefabByName(DungeonSpawnedObject.SpawnKind kind, string prefabName)
    {
        GameObject[] source = kind == DungeonSpawnedObject.SpawnKind.Item ? itemPrefabs : enemyPrefabs;

        for (int i = 0; i < source.Length; i++)
        {
            if (source[i] != null && source[i].name == prefabName)
                return source[i];
        }

        return null;
    }
}
