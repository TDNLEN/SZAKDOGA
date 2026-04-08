using System.Collections.Generic;
using UnityEngine;

public class RailGenerator : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Rail Settings")]
    public Sprite railSprite;          
    public float tileWidth = 1f;      
    public float railY = 0f;        
    public int loadRadius = 20;        

    private Dictionary<int, GameObject> spawnedRails = new Dictionary<int, GameObject>();

    private void Update()
    {
        if (player == null || railSprite == null) return;

        int playerTileX = Mathf.RoundToInt(player.position.x / tileWidth);

        for (int x = playerTileX - loadRadius; x <= playerTileX + loadRadius; x++)
        {
            if (!spawnedRails.ContainsKey(x))
            {
                SpawnRail(x);
            }
        }

        List<int> toRemove = new List<int>();
        foreach (var kvp in spawnedRails)
        {
            if (Mathf.Abs(kvp.Key - playerTileX) > loadRadius + 2)
            {
                toRemove.Add(kvp.Key);
            }
        }

        foreach (int x in toRemove)
        {
            Destroy(spawnedRails[x]);
            spawnedRails.Remove(x);
        }
    }

    private void SpawnRail(int x)
    {
        GameObject railGO = new GameObject($"Rail_{x}");
        railGO.transform.parent = transform;
        railGO.transform.position = new Vector3(x * tileWidth, railY, 0f);

        var sr = railGO.AddComponent<SpriteRenderer>();
        sr.sprite = railSprite;

        sr.sortingLayerName = "Rails";
        sr.sortingOrder = -5;   

        spawnedRails.Add(x, railGO);
    }


}
