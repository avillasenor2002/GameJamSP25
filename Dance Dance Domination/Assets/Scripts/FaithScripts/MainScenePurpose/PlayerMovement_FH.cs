using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement_FH : MonoBehaviour
{
    public Tilemap tilemap;
    public TilemapDataAssigner_FH tileDataAssigner;
    public List<int> walkableTileIDs;
    public float moveSpeed = 5f;

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
        if (!isMoving)
        {
            HandleInput();
        }

        MovePlayer();
    }

    void HandleInput()
    {
        Vector3Int direction = Vector3Int.zero;
        if (Input.GetKeyDown(KeyCode.W)) direction = Vector3Int.up;
        else if (Input.GetKeyDown(KeyCode.S)) direction = Vector3Int.down;
        else if (Input.GetKeyDown(KeyCode.A)) direction = Vector3Int.left;
        else if (Input.GetKeyDown(KeyCode.D)) direction = Vector3Int.right;

        if (direction != Vector3Int.zero)
        {
            Vector3Int potentialPosition = currentGridPosition + direction;

            // Try to activate NPCs in front of player
            HumanNPC_FH[] npcs = FindObjectsOfType<HumanNPC_FH>();
            foreach (HumanNPC_FH npc in npcs)
            {
                if (npc.GetCurrentGridPosition() == potentialPosition)
                {
                    npc.ActivateObject();
                }
            }

            // Check if player can move
            bool playerCanMove = CanMoveTo(potentialPosition);

            if (playerCanMove)
            {
                isMoving = true;
                targetGridPosition = potentialPosition;
            }

            // Regardless of player success, allow NPCs to try to move
            foreach (HumanNPC_FH npc in npcs)
            {
                if (npc.IsActive() && !npc.IsMoving())
                {
                    npc.TryFollowMove(direction);
                }
            }
        }
    }

    void MovePlayer()
    {
        if (isMoving)
        {
            Vector3 targetWorldPos = tilemap.CellToWorld(targetGridPosition) + tilemap.cellSize / 2f;
            transform.position = Vector3.MoveTowards(transform.position, targetWorldPos, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetWorldPos) < 0.01f)
            {
                currentGridPosition = targetGridPosition;
                CenterOnTile(currentGridPosition);
                isMoving = false;
            }
        }
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

    public bool CanMoveTo(Vector3Int gridPosition)
    {
        if (!IsTileWalkable(gridPosition)) return false;

        foreach (HumanNPC_FH npc in FindObjectsOfType<HumanNPC_FH>())
        {
            if (npc.GetCurrentGridPosition() == gridPosition && npc.WillBlockTile(gridPosition))
                return false;
        }

        return true;
    }

    private bool IsChainBlocked(Vector3Int position, Vector3Int direction)
    {
        foreach (HumanNPC_FH npc in FindObjectsOfType<HumanNPC_FH>())
        {
            if (npc.IsActive() && npc.GetCurrentGridPosition() == position)
            {
                Vector3Int nextPos = position + direction;

                // NPC must be able to move forward, and the rest of the chain must too
                if (!npc.CanMoveTo(nextPos) || IsChainBlocked(nextPos, direction))
                {
                    return true;
                }
            }
        }

        return !IsTileWalkable(position);
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

    public Vector3Int GetTargetGridPosition()
    {
        return targetGridPosition;
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public bool WillBlockTile(Vector3Int targetPos)
    {
        return !IsTileWalkable(targetPos);
    }
}
