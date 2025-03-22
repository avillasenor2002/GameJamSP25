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
    public bool isActive = false; // Initially inactive

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
        // For group movement, active NPCs follow via FollowMove()
        MoveObject();
    }

    // Called by the PlayerMovement script to have this NPC follow the group.
    public void FollowMove(Vector3Int offset)
    {
        if (!isMoving)
        {
            Vector3Int potentialPosition = currentGridPosition + offset;

            // Activate any other NPC on that tile.
            HumanNPC[] npcs = FindObjectsOfType<HumanNPC>();
            foreach (HumanNPC npc in npcs)
            {
                if (npc != this && npc.GetCurrentGridPosition() == potentialPosition)
                {
                    npc.ActivateObject();
                }
            }

            // Check if movement is allowed:
            // Ignore occupancy for group members while GroupIsMoving is true.
            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            bool playerBlocks = false;
            if (player != null && player.GetCurrentGridPosition() == potentialPosition)
            {
                if (!player.IsMovingStatus)
                    playerBlocks = true;
            }

            // Set moving status first so occupancy checks ignore self.
            bool previousMoving = isMoving;
            isMoving = true;
            if (IsTileWalkable(potentialPosition) && !IsTileOccupied(potentialPosition) && !playerBlocks)
            {
                targetGridPosition = potentialPosition;
            }
            else
            {
                isMoving = previousMoving;
                Debug.Log($"{gameObject.name} cannot move to {potentialPosition}: Blocked or unwalkable.");
            }
        }
    }

    void MoveObject()
    {
        if (isMoving)
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
    }

    bool IsTileWalkable(Vector3Int gridPosition)
    {
        if (tileDataAssigner == null || tileDataAssigner.tileDataGrid == null)
        {
            Debug.LogError("TileDataAssigner is not properly set!");
            return false;
        }
        int x = gridPosition.x - tilemap.cellBounds.xMin;
        int y = gridPosition.y - tilemap.cellBounds.yMin;
        if (x < 0 || x >= tileDataAssigner.gridWidth || y < 0 || y >= tileDataAssigner.gridHeight)
        {
            Debug.Log($"Out of bounds: {gridPosition}");
            return false;
        }
        int tileID = tileDataAssigner.tileDataGrid[x, y].tileType;
        bool canMove = walkableTileIDs.Contains(tileID);
        return canMove;
    }

    bool IsTileOccupied(Vector3Int gridPosition)
    {
        // Check all NPCs.
        HumanNPC[] npcs = FindObjectsOfType<HumanNPC>();
        foreach (HumanNPC npc in npcs)
        {
            if (npc != this && npc.GetCurrentGridPosition() == gridPosition)
            {
                // Block if the other NPC is stationary.
                if (!npc.IsMovingStatus)
                    return true;
            }
        }
        // Check if the player occupies the tile; ignore if the player is moving.
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null && player.GetCurrentGridPosition() == gridPosition)
        {
            if (!player.IsMovingStatus)
                return true;
        }
        return false;
    }

    public Vector3Int GetCurrentGridPosition()
    {
        return currentGridPosition;
    }

    public Vector3Int NPCGetCurrentGridPosition()
    {
        return currentGridPosition;
    }

    public bool IsMovingStatus
    {
        get { return isMoving; }
    }

    void CenterOnTile(Vector3Int gridPosition)
    {
        Vector3 tileCenterPosition = tilemap.CellToWorld(gridPosition) + tilemap.cellSize / 2f;
        transform.position = tileCenterPosition;
    }

    public void ActivateObject()
    {
        if (!isActive)
        {
            isActive = true;
            Debug.Log($"{gameObject.name} activated.");
        }
    }
}
