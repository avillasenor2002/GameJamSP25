using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDataAssigner : MonoBehaviour
{
    public Tilemap tilemap;
    public TileDataScriptableObject TileDataSO; // ScriptableObject for tile data
    public TileBase[] validTiles; // Array of valid tiles to check for

    public int gridWidth;
    public int gridHeight;
    public TileData[,] tileDataGrid;

    private void Start()
    {
        // Count the tiles on the tilemap and dynamically adjust grid size
        CountTilesAndGenerateGrid();
    }

    // Count the tiles already placed on the tilemap and set grid dimensions
    public void CountTilesAndGenerateGrid()
    {
        if (tilemap == null)
        {
            Debug.LogError("Tilemap not assigned!");
            return;
        }

        BoundsInt bounds = tilemap.cellBounds;
        gridWidth = bounds.size.x;
        gridHeight = bounds.size.y;

        Debug.Log($"Tilemap Bounds: Width={gridWidth}, Height={gridHeight}");

        tileDataGrid = new TileData[gridWidth, gridHeight];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3Int tilePosition = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                TileBase currentTile = tilemap.GetTile(tilePosition);

                int tileType = 0; // Default to non-walkable
                if (currentTile != null)
                {
                    for (int i = 0; i < validTiles.Length; i++)
                    {
                        if (currentTile == validTiles[i])
                        {
                            tileType = i + 1;
                            break;
                        }
                    }
                }

                tileDataGrid[x, y] = new TileData { tileType = tileType, tileName = currentTile != null ? currentTile.name : "Empty" };

                //Debug.Log($"Tile [{x}, {y}] - Position: {tilePosition} - TileType: {tileType}");
            }
        }
    }

    [System.Serializable]
    public class TileData
    {
        public int tileType;  // Numeric tile identifier
        public string tileName;
    }
}
