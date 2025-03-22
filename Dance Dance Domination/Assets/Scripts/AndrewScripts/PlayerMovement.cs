using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public Tilemap tilemap;
    public TilemapDataAssigner tileDataAssigner;
    public List<int> walkableTileIDs;
    public float moveSpeed = 5f;

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

            HumanNPC[] npcs = FindObjectsOfType<HumanNPC>();

            // Activate any NPC in the target tile
            foreach (HumanNPC npc in npcs)
            {
                if (npc.NPCGetCurrentGridPosition() == potentialPosition)
                {
                    npc.ActivateObject();
                }
            }

            // ✅ Traverse through NPCs in a line to see if movement is possible
            Vector3Int checkPosition = potentialPosition;
            while (true)
            {
                HumanNPC npcAtPosition = GetNPCAtPosition(checkPosition);
                if (npcAtPosition != null)
                {
                    npcAtPosition.ActivateObject(); // Activate if inactive

                    Vector3Int nextCheck = checkPosition + direction;
                    if (!npcAtPosition.CanMoveTo(nextCheck))
                    {
                        Debug.Log("Player blocked by NPC chain (wall or unmovable).");
                        return;
                    }

                    checkPosition = nextCheck;
                }
                else break; // No more NPCs in the chain
            }

            // Final check: is initial tile walkable
            if (IsTileWalkable(potentialPosition))
            {
                GroupIsMoving = true;
                isMoving = true;
                targetGridPosition = potentialPosition;

                foreach (HumanNPC npc in npcs)
                {
                    if (npc.isActive)
                    {
                        npc.FollowMove(direction);
                    }
                }

                foreach (HumanNPC npc in npcs)
                {
                    if (npc.isActive && npc.WillBlockTile(currentGridPosition))
                    {
                        isMoving = false;
                        GroupIsMoving = false;
                        Debug.Log("Player blocked by NPC who cannot move.");
                        return;
                    }
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
                CenterOnTile(targetGridPosition);
                currentGridPosition = targetGridPosition;
                isMoving = false;
                GroupIsMoving = false;
            }
        }
    }

    bool IsTileWalkable(Vector3Int gridPosition)
    {
        if (tileDataAssigner == null || tileDataAssigner.tileDataGrid == null) return false;

        int x = gridPosition.x - tilemap.cellBounds.xMin;
        int y = gridPosition.y - tilemap.cellBounds.yMin;
        if (x < 0 || x >= tileDataAssigner.gridWidth || y < 0 || y >= tileDataAssigner.gridHeight) return false;

        int tileID = tileDataAssigner.tileDataGrid[x, y].tileType;
        return walkableTileIDs.Contains(tileID);
    }

    HumanNPC GetNPCAtPosition(Vector3Int position)
    {
        HumanNPC[] npcs = FindObjectsOfType<HumanNPC>();
        foreach (HumanNPC npc in npcs)
        {
            if (npc.NPCGetCurrentGridPosition() == position)
            {
                return npc;
            }
        }
        return null;
    }

    public Vector3Int GetCurrentGridPosition() => currentGridPosition;
    public bool IsMovingStatus => isMoving;

    void CenterOnTile(Vector3Int gridPosition)
    {
        Vector3 tileCenterPosition = tilemap.CellToWorld(gridPosition) + tilemap.cellSize / 2f;
        transform.position = tileCenterPosition;
    }
}
