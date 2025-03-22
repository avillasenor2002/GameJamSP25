using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapDataManager : MonoBehaviour
{
    public Tilemap tilemap;
public TileDataScriptableObject TileDataSO; // ScriptableObject for aboveground tiles

[System.Serializable]
public class TileInfo
{
    public TileBase tile;                    // The tile from the palette
    public string tileName;                  // Name associated with this tile
}

public List<TileInfo> groundTilePalette; // Aboveground tile sprites and names
public TileData[,] tileDataGrid;
public int gridWidth = 10;
public int gridHeight = 9;

private bool isAboveground = true;           // Flag to track current configuration
public TileDataScriptableObject abovetileDataSO; // Reference to your TileDataScriptableObject
public TileDataScriptableObject undertileDataSO; // Reference to your TileDataScriptableObject

private void Start()
{
    // Call this method to populate tile data with the level sequence string
    string levelSequenceExample = "112222222114060606031400000003140006000314006600031400000003140606060314060606031155555551";
    //abovetileDataSO.PopulateTileDataFromSequence(levelSequenceA);
    tileDataGrid = new TileData[gridWidth, gridHeight];
    InitializeTileData();
    //UpdateTilemapSprites();

    //341111411130001000113000100011311111100130001141113000100011300010001135050505053656456556
}

private void OnEnable()
{
    // Ensure the tilemap is updated whenever this manager is enabled
    //UpdateTilemapSprites();
}

public void InitializeTileData()
{
    // Initialize tile data with the default (aboveground) configuration
    ApplyTileDataFromScriptableObject(TileDataSO, groundTilePalette);
}

private void ApplyTileDataFromScriptableObject(TileDataScriptableObject tileDataSO, List<TileInfo> tilePalette)
{
    int tileIndex = 0;

    for (int y = gridHeight - 1; y >= 0; y--) // Start from the top row
    {
        for (int x = 0; x < gridWidth; x++)   // Move left to right
        {
            if (tileIndex < tileDataSO.tileDataList.Count)
            {
                var tileInfo = tileDataSO.tileDataList[tileIndex];
                tileDataGrid[x, y] = new TileData
                {
                    tileType = tileInfo.tileType,
                    tileName = tilePalette[tileInfo.tileType].tileName
                };
                tileIndex++;
            }
        }
    }
}

/*public void SwitchToAboveground()
{
    isAboveground = true;
    ApplyTileDataFromScriptableObject(abovegroundTileDataSO, abovegroundTilePalette);
    UpdateTilemapSprites();
}*/

/*public void SwitchToBelowground()
{
    isAboveground = false;
    ApplyTileDataFromScriptableObject(belowgroundTileDataSO, belowgroundTilePalette);
    UpdateTilemapSprites();
}*/

/*public void UpdateTilemapSprites()
{
    var currentPalette = isAboveground ? abovegroundTilePalette : belowgroundTilePalette;

    for (int x = 0; x < gridWidth; x++)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            Vector3Int tilePosition = new Vector3Int(x, y, 0);
            int tileType = tileDataGrid[x, y].tileType;

            if (tileType >= 0 && tileType < currentPalette.Count)
            {
                tilemap.SetTile(tilePosition, currentPalette[tileType].tile);
            }
        }
    }
}*/

/*public void StoreTileData()
{
    var currentPalette = isAboveground ? groundTilePalette : belowgroundTilePalette;

    // Store current tile data to remember any changes
    for (int x = 0; x < gridWidth; x++)
    {
        for (int y = 0; y < gridHeight; y++)
        {
            Vector3Int position = new Vector3Int(x, y, 0);
            TileBase currentTile = tilemap.GetTile(position);
            int tileTypeIndex = currentPalette.FindIndex(tile => tile.tile == currentTile);
            if (tileTypeIndex != -1)
            {
                tileDataGrid[x, y].tileType = tileTypeIndex;
            }
        }
    }
}*/

[System.Serializable]
public class TileData
{
    public int tileType;       // Unique type for each tile
    public string tileName;    // Name of each tile
}
}
