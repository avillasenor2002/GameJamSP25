using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HumanNPC : MonoBehaviour
{
    public Tilemap tilemap;
    public TilemapDataAssigner tileDataAssigner;
    public List<int> walkableTileIDs;
    public float moveSpeed = 5f;
    public bool isActive = false;

    private Vector3Int currentGridPosition;
    private Vector3Int targetGridPosition;
    private bool isMoving;

    void Start()
    {
        currentGridPosition = tilemap.WorldToCell(transform.position);
        CenterOnTile(currentGridPosition);
        targetGridPosition = currentGridPosition;
    }

    void Update()
    {
        if (isActive && isMoving)
        {
            MoveObject();
        }
    }

    public void ActivateObject()
    {
        if (!isActive)
        {
            isActive = true;
            Debug.Log($"{gameObject.name} has been activated!");
        }
    }

    public void FollowMove(Vector3Int direction)
    {
        if (!isActive || isMoving) return;

        Vector3Int potentialPosition = currentGridPosition + direction;

        // Check if NPCs in front need to be activated or block path
        HumanNPC[] npcs = FindObjectsOfType<HumanNPC>();
        Vector3Int checkPosition = potentialPosition;

        while (true)
        {
            HumanNPC npcInPath = GetNPCAtPosition(checkPosition, npcs);
            if (npcInPath != null)
            {
                npcInPath.ActivateObject();

                Vector3Int nextPosition = checkPosition + direction;

                if (!npcInPath.CanMoveTo(nextPosition))
                {
                    Debug.Log($"{gameObject.name} is blocked by {npcInPath.name} who cannot move to {nextPosition}");
                    return;
                }

                checkPosition = nextPosition;
            }
            else break;
        }

        if (IsTileWalkable(potentialPosition))
        {
            targetGridPosition = potentialPosition;
            isMoving = true;
        }
    }

    void MoveObject()
    {
        Vector3 targetWorldPos = tilemap.CellToWorld(targetGridPosition) + tilemap.cellSize / 2f;
        transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetWorldPos) < 0.01f)
        {
            CenterOnTile(targetGridPosition);
            currentGridPosition = targetGridPosition;
            isMoving = false;
        }
    }

    bool IsTileWalkable(Vector3Int gridPosition)
    {
        if (tileDataAssigner == null || tileDataAssigner.tileDataGrid == null) return false;

        int x = gridPosition.x - tilemap.cellBounds.xMin;
        int y = gridPosition.y - tilemap.cellBounds.yMin;

        if (x < 0 || x >= tileDataAssigner.gridWidth || y < 0 || y >= tileDataAssigner.gridHeight)
            return false;

        int tileID = tileDataAssigner.tileDataGrid[x, y].tileType;
        return walkableTileIDs.Contains(tileID);
    }

    public bool CanMoveTo(Vector3Int gridPosition)
    {
        return IsTileWalkable(gridPosition);
    }

    public bool WillBlockTile(Vector3Int gridPosition)
    {
        // Returns true if this NPC is currently on the tile and cannot move (e.g., is against a wall)
        if (GetCurrentGridPosition() == gridPosition && !isMoving)
        {
            // Check if there's any direction that is walkable
            Vector3Int[] directions = new Vector3Int[]
            {
                Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right
            };

            foreach (Vector3Int dir in directions)
            {
                Vector3Int checkPos = currentGridPosition + dir;
                if (IsTileWalkable(checkPos))
                {
                    return false; // If any direction is walkable, don't block
                }
            }

            return true; // No available movement, this NPC will block
        }

        return false; // Not on the queried tile
    }

    HumanNPC GetNPCAtPosition(Vector3Int position, HumanNPC[] cached)
    {
        foreach (HumanNPC npc in cached)
        {
            if (npc != this && npc.GetCurrentGridPosition() == position)
            {
                return npc;
            }
        }
        return null;
    }

    public Vector3Int GetCurrentGridPosition()
    {
        return currentGridPosition;
    }

    public Vector3Int NPCGetCurrentGridPosition()
    {
        return currentGridPosition;
    }

    void CenterOnTile(Vector3Int gridPosition)
    {
        Vector3 tileCenterPosition = tilemap.CellToWorld(gridPosition) + tilemap.cellSize / 2f;
        transform.position = tileCenterPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ActivateObject();
        }
    }
}
