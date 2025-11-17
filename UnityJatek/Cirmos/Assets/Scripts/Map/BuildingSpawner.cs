using UnityEngine;

public class BuildingSpawner : MonoBehaviour
{
    public GameObject buildingPrefab;   // ide húzd be a kész épület prefab-ot
    public float spacing = 1000f;       // minden 1000 egységen legyen épület
    public int spawnCount = 10;         // mennyit hozzon létre elõre
    public float buildingY = 0f;        // milyen magasságban legyen (a sínekhez igazítva)

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
