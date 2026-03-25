using System.Collections.Generic;
using UnityEngine;

public class InfiniteGround : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Tiles")]
    public Sprite[] groundSprites;
    public Vector2 tileSize = new Vector2(1f, 1f);

    [Header("Rendering")]
    public Material litGroundMaterial;
    public string sortingLayerName = "Default";
    public int sortingOrder = -100;

    [Header("Chunk")]
    public int chunkSize = 16;
    public int loadRadius = 2;
    public int unloadExtra = 1;

    private Dictionary<Vector2Int, GameObject> spawnedChunks = new Dictionary<Vector2Int, GameObject>();

    private void Update()
    {
        if (player == null) return;
        if (groundSprites == null || groundSprites.Length == 0) return;

        Vector2 playerPos = player.position;
        Vector2Int playerChunk = new Vector2Int(
            Mathf.FloorToInt(playerPos.x / (chunkSize * tileSize.x)),
            Mathf.FloorToInt(playerPos.y / (chunkSize * tileSize.y))
        );

        for (int x = -loadRadius; x <= loadRadius; x++)
        {
            for (int y = -loadRadius; y <= loadRadius; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunk.x + x, playerChunk.y + y);
                if (!spawnedChunks.ContainsKey(chunkCoord))
                {
                    SpawnChunk(chunkCoord);
                }
            }
        }

        List<Vector2Int> chunkToRemove = new List<Vector2Int>();

        foreach (var del in spawnedChunks)
        {
            Vector2Int coord = del.Key;

            int distX = Mathf.Abs(coord.x - playerChunk.x);
            int distY = Mathf.Abs(coord.y - playerChunk.y);

            if (distX > loadRadius + unloadExtra || distY > loadRadius + unloadExtra)
            {
                chunkToRemove.Add(coord);
            }
        }

        foreach (var del in chunkToRemove)
        {
            Destroy(spawnedChunks[del]);
            spawnedChunks.Remove(del);
        }
    }

    private void SpawnChunk(Vector2Int chunkCoord)
    {
        GameObject chunkGO = new GameObject($"Chunk_{chunkCoord.x}_{chunkCoord.y}");
        chunkGO.transform.parent = transform;

        Vector2 origin = new Vector2(
            chunkCoord.x * chunkSize * tileSize.x,
            chunkCoord.y * chunkSize * tileSize.y
        );

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2 pos = origin + new Vector2(x * tileSize.x, y * tileSize.y);

                GameObject tile = new GameObject($"tile_{x}_{y}");
                tile.transform.parent = chunkGO.transform;
                tile.transform.position = pos;

                SpriteRenderer sr = tile.AddComponent<SpriteRenderer>();
                sr.sprite = groundSprites[Random.Range(0, groundSprites.Length)];
                sr.sortingLayerName = sortingLayerName;
                sr.sortingOrder = sortingOrder;

                if (litGroundMaterial != null)
                    sr.material = litGroundMaterial;
            }
        }

        spawnedChunks.Add(chunkCoord, chunkGO);
    }
}