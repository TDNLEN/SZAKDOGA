using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public GameObject buildingPrefab;  
    public float spacing = 1000f;    
    public int spawnCount = 10;       
    public float buildingY = 0f;      

    private void Start()
    {
        SpawnBuildings();
    }

    private void SpawnBuildings()
    {
        if (buildingPrefab == null)
        {
            Debug.LogError("Nincs beállítva a buildingPrefab!");    
            return;
        }

        for (int i = 1; i <= spawnCount; i++)
        {
            float x = i * spacing;
            Vector3 pos = new Vector3(x, buildingY, 0f);

            Instantiate(buildingPrefab, pos, Quaternion.identity);
        }
    }
}
