using UnityEngine;
using UnityEngine.Tilemaps;

public class StaticHazard : MonoBehaviour
{
    private Tilemap tilemap;
    private Vector3Int hazardGridPosition;

    void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("StaticHazard: No tilemap found in the scene!");
            return;
        }

        // Lock this hazard to its tile grid position
        hazardGridPosition = tilemap.WorldToCell(transform.position);
        CenterOnTile(hazardGridPosition);
    }

    void Update()
    {
        // Constantly check for HumanNPCs on the same tile
        HumanNPC[] npcs = FindObjectsOfType<HumanNPC>();
        foreach (HumanNPC npc in npcs)
        {
            if (npc != null && npc.GetCurrentGridPosition() == hazardGridPosition)
            {
                Destroy(npc.gameObject);
                Debug.Log($"StaticHazard destroyed {npc.name}");
            }
        }
    }

    void CenterOnTile(Vector3Int gridPos)
    {
        Vector3 center = tilemap.CellToWorld(gridPos) + tilemap.cellSize / 2f;
        transform.position = center;
    }
}
