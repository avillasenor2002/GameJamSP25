﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HumanNPC_FH : MonoBehaviour
{
    public Tilemap tilemap;
    public TilemapDataAssigner_FH tileDataAssigner;

    public TempoBarManager tempoBarManager;


    public static List<HumanNPC_FH> activeNPCs; 
    //If player collides to inactive NPCs, it will activate it, and add into this list. (It's info)

    public List<int> walkableTileIDs;
    public float moveSpeed = 5f;
    public bool isActive = false;

    private Vector3Int currentGridPosition;
    private Vector3Int targetGridPosition;
    private bool isMoving;

    void Start()
    {
        if (activeNPCs == null)
        {
            activeNPCs = new List<HumanNPC_FH>();
        }
        tempoBarManager = FindObjectOfType<TempoBarManager>();

        // Find Tilemap if not assigned
        if (tilemap == null)
        {
            tilemap = FindObjectOfType<Tilemap>();
            if (tilemap == null)
            {
                Debug.LogError("HumanNPC: Tilemap not found in the scene!");
                return;
            }
        }

        // Find TileDataAssigner if not assigned
        if (tileDataAssigner == null)
        {
            tileDataAssigner = FindObjectOfType<TilemapDataAssigner_FH>();
            if (tileDataAssigner == null)
            {
                Debug.LogError("HumanNPC: TilemapDataAssigner not found in the scene!");
                return;
            }
        }

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
            if (!activeNPCs.Contains(this))
            {
                activeNPCs.Add(this);
            }
        }
    }

    public static void RemoveLastNPC()
    {
        if (activeNPCs.Count == 0 || activeNPCs == null)
        {
            return;
        }

        HumanNPC_FH lastNPC = activeNPCs[activeNPCs.Count - 1];
        activeNPCs.Remove(lastNPC);
        Destroy(lastNPC.gameObject);   
    }


    public void TryFollowMove(Vector3Int direction)
    {
        Vector3Int nextPosition = currentGridPosition + direction;

        // Attempt to activate any inactive NPCs in the next tile
        foreach (HumanNPC_FH npc in activeNPCs)
        {
            if (!npc.IsActive() && npc.GetCurrentGridPosition() == nextPosition)
            {
                npc.ActivateObject();
            }
        }

        // Check if there's an NPC in the next tile that can't move in the same direction
        HumanNPC_FH npcInNextTile = null;
        foreach (HumanNPC_FH npc in activeNPCs)
        {
            if (npc != this && npc.IsActive() && npc.GetCurrentGridPosition() == nextPosition)
            {
                //npcInNextTile = npc;
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
            foreach (HumanNPC_FH npc in activeNPCs)
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

        // Block other NPCs if they're occupying the space and won't move
        foreach (HumanNPC_FH npc in FindObjectsOfType<HumanNPC_FH>())
        {
            if (npc != this && npc.GetCurrentGridPosition() == gridPosition && npc.WillBlockTile(gridPosition))
            {
                return false;
            }
        }

        // Block if trying to move into the player’s position
        PlayerMovement_FH player = FindObjectOfType<PlayerMovement_FH>();
        if (player != null)
        {
            Vector3Int playerCurrent = player.GetCurrentGridPosition();
            Vector3Int playerTarget = player.GetTargetGridPosition();

            // If player is not moving, or this NPC is trying to step where the player is/was
            if ((!player.IsMoving() && playerCurrent == gridPosition) ||
                (player.IsMoving() && playerTarget == gridPosition))
            {
                return false;
            }
        }

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
        foreach (HumanNPC_FH npc in FindObjectsOfType<HumanNPC_FH>())
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
