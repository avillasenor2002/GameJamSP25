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
    public float holdDelay = 0.15f;

    private Vector3Int currentGridPosition;
    private Vector3Int targetGridPosition;
    private bool isMoving;
    private float inputCooldown;

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
            inputCooldown -= Time.deltaTime;
            if (inputCooldown <= 0f)
            {
                HandleInput();
            }
        }

        MovePlayer();
    }

    void HandleInput()
    {
        Vector3Int direction = Vector3Int.zero;
        if (Input.GetKey(KeyCode.W)) direction = Vector3Int.up;
        else if (Input.GetKey(KeyCode.S)) direction = Vector3Int.down;
        else if (Input.GetKey(KeyCode.A)) direction = Vector3Int.left;
        else if (Input.GetKey(KeyCode.D)) direction = Vector3Int.right;

        if (direction != Vector3Int.zero)
        {
            Vector3Int potentialPosition = currentGridPosition + direction;

            foreach (HumanNPC npc in FindObjectsOfType<HumanNPC>())
            {
                if (npc.GetCurrentGridPosition() == potentialPosition)
                {
                    npc.ActivateObject();
                }
            }

            bool playerCanMove = CanMoveTo(potentialPosition) && !IsChainBlocked(potentialPosition, direction);

            if (playerCanMove)
            {
                isMoving = true;
                targetGridPosition = potentialPosition;
                inputCooldown = holdDelay;
            }

            foreach (HumanNPC npc in FindObjectsOfType<HumanNPC>())
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

        foreach (HumanNPC npc in FindObjectsOfType<HumanNPC>())
        {
            if (npc.GetCurrentGridPosition() == gridPosition && npc.WillBlockTile(gridPosition))
                return false;
        }

        return true;
    }

    private bool IsChainBlocked(Vector3Int position, Vector3Int direction)
    {
        foreach (HumanNPC npc in FindObjectsOfType<HumanNPC>())
        {
            if (npc.IsActive() && npc.GetCurrentGridPosition() == position)
            {
                Vector3Int nextPos = position + direction;
                if (!npc.CanMoveTo(nextPos) || IsChainBlocked(nextPos, direction))
                {
                    return true;
                }
            }
        }

        return false;
    }

    void CenterOnTile(Vector3Int gridPos)
    {
        Vector3 center = tilemap.CellToWorld(gridPos) + tilemap.cellSize / 2f;
        transform.position = center;
    }

    public Vector3Int GetCurrentGridPosition() => currentGridPosition;
    public Vector3Int GetTargetGridPosition() => targetGridPosition;
    public bool IsMoving() => isMoving;
    public bool WillBlockTile(Vector3Int targetPos) => !IsTileWalkable(targetPos);
}
