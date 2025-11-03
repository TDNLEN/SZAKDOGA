using System.Collections.Generic;
using UnityEngine;

public class InfiniteGround : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Tiles")]
    public Sprite[] groundSprites;    // ide húzd be a 4 talaj sprite-ot
    public Vector2 tileSize = new Vector2(1f, 1f); // mekkora egy tile (sprite mérete)

    [Header("Chunk")]
    public int chunkSize = 16;        // 16x16 tile egy chunk
    public int loadRadius = 2;
    public int unloadExtra = 1;        // ennyi chunk távolságban töltünk
                                      

    // melyik chunk már létezik
    private Dictionary<Vector2Int, GameObject> spawnedChunks = new Dictionary<Vector2Int, GameObject>();

    private void Update()
    {
        if (player == null) return;

        // melyik chunkban van most a player
        Vector2 playerPos = player.position;
        Vector2Int playerChunk = new Vector2Int(
            Mathf.FloorToInt(playerPos.x / (chunkSize * tileSize.x)),
            Mathf.FloorToInt(playerPos.y / (chunkSize * tileSize.y))
        );

        // körbenézünk a loadRadius-on belül
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

        foreach(var del in spawnedChunks)
        {

            Vector2Int coord = del.Key;

            int distX = Mathf.Abs(coord.x - playerChunk.x);
            int distY = Mathf.Abs(coord.y - playerChunk.y);

            if(distX > loadRadius + unloadExtra || distY > loadRadius + unloadExtra)
            {
                chunkToRemove.Add(coord);
            }

        }
        foreach(var del in chunkToRemove)
        {
            Destroy(spawnedChunks[del]);
            spawnedChunks.Remove(del);
        }
    }

    private void SpawnChunk(Vector2Int chunkCoord)
    {
        // üres parent az átláthatóság kedvéért
        GameObject chunkGO = new GameObject($"Chunk_{chunkCoord.x}_{chunkCoord.y}");
        chunkGO.transform.parent = transform;

        // a chunk bal-alsó pozíciója
        Vector2 origin = new Vector2(
            chunkCoord.x * chunkSize * tileSize.x,
            chunkCoord.y * chunkSize * tileSize.y
        );

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                // világbeli pozíció
                Vector2 pos = origin + new Vector2(x * tileSize.x, y * tileSize.y);

                // csinálunk egy tile-t
                GameObject tile = new GameObject($"tile_{x}_{y}");
                tile.transform.parent = chunkGO.transform;
                tile.transform.position = pos;

                var sr = tile.AddComponent<SpriteRenderer>();
                sr.sprite = groundSprites[Random.Range(0, groundSprites.Length)];
                sr.sortingOrder = -100; // hogy mindenki alatt legyen
            }
        }

        spawnedChunks.Add(chunkCoord, chunkGO);
    }
}
