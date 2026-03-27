using UnityEngine;

public class DungeonSpawnedObject : MonoBehaviour
{
    public enum SpawnKind
    {
        Item,
        Enemy
    }

    public SpawnKind kind;

    public bool collectedByPlayer = false;
    public bool permanentlyRemoved = false;

    public string houseId;
    public string prefabName;
    public int spawnPointIndex = -1;
}