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
        if (isMoving)
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

    public void TryFollowMove(Vector3Int direction)
    {
        Vector3Int nextPosition = currentGridPosition + direction;

        // Attempt to activate any inactive NPCs in the next tile
        foreach (HumanNPC npc in FindObjectsOfType<HumanNPC>())
        {
            if (!npc.IsActive() && npc.GetCurrentGridPosition() == nextPosition)
            {
                npc.ActivateObject();
            }
        }

        // Check if there's an NPC in the next tile that can't move in the same direction
        HumanNPC npcInNextTile = null;
        foreach (HumanNPC npc in FindObjectsOfType<HumanNPC>())
        {
            if (npc != this && npc.IsActive() && npc.GetCurrentGridPosition() == nextPosition)
            {
                npcInNextTile = npc;
                break;
            }
        }

        if (IsChainBlocked(nextPosition, direction))
        {
            return;
        }

        // Proceed if current NPC can move to target tile
        if (CanMoveTo(nextPosition))
        {
            isMoving = true;
            targetGridPosition = nextPosition;

            // Continue group propagation
            foreach (HumanNPC npc in FindObjectsOfType<HumanNPC>())
            {
                if (npc != this && npc.IsActive() && !npc.IsMoving())
                {
                    npc.TryFollowMove(direction);
                }
            }
        }
    }




    void MoveObject()
    {
        Vector3 worldTarget = tilemap.CellToWorld(targetGridPosition) + tilemap.cellSize / 2f;
        transform.position = Vector3.MoveTowards(transform.position, worldTarget, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, worldTarget) < 0.01f)
        {
            currentGridPosition = targetGridPosition;
            CenterOnTile(currentGridPosition);
            isMoving = false;
        }
    }

    public bool CanMoveTo(Vector3Int gridPosition)
    {
        if (!IsTileWalkable(gridPosition)) return false;

        foreach (HumanNPC npc in FindObjectsOfType<HumanNPC>())
        {
            if (npc != this && npc.GetCurrentGridPosition() == gridPosition && npc.WillBlockTile(gridPosition))
                return false;
        }

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null && player.GetCurrentGridPosition() == gridPosition && player.WillBlockTile(gridPosition))
            return false;

        return true;
    }

    bool IsTileWalkable(Vector3Int gridPosition)
    {
        int x = gridPosition.x - tilemap.cellBounds.xMin;
        int y = gridPosition.y - tilemap.cellBounds.yMin;

        if (x < 0 || y < 0 || x >= tileDataAssigner.gridWidth || y >= tileDataAssigner.gridHeight)
            return false;

        int tileID = tileDataAssigner.tileDataGrid[x, y].tileType;
        return walkableTileIDs.Contains(tileID);
    }

    void CenterOnTile(Vector3Int gridPos)
    {
        Vector3 center = tilemap.CellToWorld(gridPos) + tilemap.cellSize / 2f;
        transform.position = center;
    }

    public Vector3Int GetCurrentGridPosition()
    {
        return currentGridPosition;
    }

    public Vector3Int NPCGetCurrentGridPosition()
    {
        return currentGridPosition;
    }

    public bool IsActive() => isActive;

    public bool IsMoving() => isMoving;

    public bool WillBlockTile(Vector3Int targetPos)
    {
        return !IsTileWalkable(targetPos);
    }

    private bool IsChainBlocked(Vector3Int position, Vector3Int direction)
    {
        foreach (HumanNPC npc in FindObjectsOfType<HumanNPC>())
        {
            if (npc != this && npc.IsActive() && npc.GetCurrentGridPosition() == position)
            {
                Vector3Int nextPos = position + direction;

                // If this NPC cannot move, or the next NPC in line is blocked
                if (!npc.CanMoveTo(nextPos) || npc.IsChainBlocked(nextPos, direction))
                {
                    return true;
                }
            }
        }

        // Also check if a wall or non-walkable tile is directly in front
        return !IsTileWalkable(position);
    }


}
