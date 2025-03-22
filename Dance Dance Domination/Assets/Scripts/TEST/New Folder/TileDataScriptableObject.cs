using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileDataScriptableObject", menuName = "Tilemap/Tile Data")]
public class TileDataScriptableObject : ScriptableObject
{
    [System.Serializable]
    public class TileDataInfo
    {
        public int tileType;   // Unique type or index for the tile (e.g., 1 for grass)
    }

    public List<TileDataInfo> tileDataList = new List<TileDataInfo>();

    // Method to populate the tileDataList from the Level Sequence Int string
    public void PopulateTileDataFromSequence(string levelSequence)
    {
        // Clear the existing tile data list to start fresh
        tileDataList.Clear();

        // Iterate over each character in the Level Sequence string
        foreach (char c in levelSequence)
        {
            // Convert the character (which is a digit) to an integer
            int tileType = int.Parse(c.ToString());

            // Add the parsed tile type to the tileDataList
            tileDataList.Add(new TileDataInfo { tileType = tileType });
        }
    }
}