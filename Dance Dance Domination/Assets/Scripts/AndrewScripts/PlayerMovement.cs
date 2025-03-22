using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public Tilemap tilemap;
    public TilemapDataAssigner tileDataAssigner; // Reference to the TilemapDataAssigner
    public List<int> walkableTileIDs;            // Walkable tile types (numeric IDs)
    public float moveSpeed = 5f;                 // Movement speed

    // Static flag to indicate a group move in progress.
    public static bool GroupIsMoving = false;

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

            // Activate any NPC on the target tile.
            HumanNPC[] npcs = FindObjectsOfType<HumanNPC>();
            foreach (HumanNPC npc in npcs)
            {
                if (npc.NPCGetCurrentGridPosition() == potentialPosition)
                {
                    npc.ActivateObject();
                }
            }

            // Check if movement is allowed.
            if (IsTileWalkable(potentialPosition) && !IsTileOccupied(potentialPosition))
            {
                // Begin group movement.
                GroupIsMoving = true;
                targetGridPosition = potentialPosition;
                isMoving = true;

                // Instruct all active NPCs to follow with the same offset.
                foreach (HumanNPC npc in npcs)
                {
                    if (npc.isActive)
                    {
                        npc.FollowMove(direction);
                    }
                }
            }
            else
            {
                Debug.Log($"Cannot move to {potentialPosition}: Tile is not walkable or is occupied.");
            }
        }
    }

    bool IsTileOccupied(Vector3Int gridPosition)
    {
        // During a group move, ignore occupancy by group members.
        if (GroupIsMoving)
            return false;

        // Otherwise, check all NPCs and players.
        HumanNPC[] npcs = FindObjectsOfType<HumanNPC>();
        foreach (HumanNPC npc in npcs)
        {
            if (npc.GetCurrentGridPosition() == gridPosition)
            {
                // Block if NPC is not active (i.e. not in the group).
                if (!npc.isActive)
                    return true;
            }
        }

        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach (PlayerMovement player in players)
        {
            if (player != this && player.GetCurrentGridPosition() == gridPosition)
                return true;
        }
        return false;
    }

    void MovePlayer()
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
                // When the player finishes moving, end the group move.
                GroupIsMoving = false;
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
        Debug.Log($"Checking tile at [{x}, {y}] - TileID: {tileID} - Walkable: {canMove}");
        return canMove;
    }

    public Vector3Int GetCurrentGridPosition()
    {
        return currentGridPosition;
    }

    // Public property so others (like NPCs) can check if the player is moving.
    public bool IsMovingStatus
    {
        get { return isMoving; }
    }

    void CenterOnTile(Vector3Int gridPosition)
    {
        Vector3 tileCenterPosition = tilemap.CellToWorld(gridPosition) + tilemap.cellSize / 2f;
        transform.position = tileCenterPosition;
    }
}
