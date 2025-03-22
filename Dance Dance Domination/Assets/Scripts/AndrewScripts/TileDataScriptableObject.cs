using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileDataScriptableObject", menuName = "Tile Data/Tile Data Scriptable Object")]
public class TileDataScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class TileData
    {
        public int tileType; // Represents the type of the tile (index in palette)
        public string tileName; // Name associated with the tile
    }

    public List<TileData> tileDataList; // List to store tile data
}
