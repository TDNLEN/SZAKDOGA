using UnityEngine;

[CreateAssetMenu(fileName = "DungeonConfig", menuName = "Game/Dungeon Config")]
public class DungeonConfig : ScriptableObject
{
    [Header("Loot")]
    public GameObject[] possibleItemPrefabs;
    public int itemCount = 2;

    [Header("Enemies")]
    public GameObject[] possibleEnemyPrefabs;
    public int enemyCount = 3;
}
