using UnityEngine;
using UnityEngine.Tilemaps;

public class TimedHazard : MonoBehaviour
{
    private Tilemap tilemap;
    private Vector3Int gridPosition;

    void Start()
    {
        tilemap = FindObjectOfType<Tilemap>();
        gridPosition = tilemap.WorldToCell(transform.position);
        CenterOnTile(gridPosition);
    }

    void Update()
    {
        HumanNPC[] npcs = FindObjectsOfType<HumanNPC>();
        foreach (HumanNPC npc in npcs)
        {
            if (npc.GetCurrentGridPosition() == gridPosition)
            {
                Destroy(npc.gameObject);
                Debug.Log($"TimedHazard destroyed {npc.name}");
            }
        }
    }

    void CenterOnTile(Vector3Int gridPos)
    {
        Vector3 center = tilemap.GetCellCenterWorld(gridPos);
        transform.position = center;
    }
}
