using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public Tilemap tilemap;
    public TilemapDataManager tileDataManager;
    public TilemapDataManager alternateTileDataManager;

    public List<int> abovegroundWalkableTileIDs;
    public List<int> undergroundWalkableTileIDs;
    public float moveSpeed = 5f;

    private List<int> currentWalkableTileIDs;
    public Vector3Int currentGridPosition;
    private Vector3Int targetGridPosition;
    private bool isMoving;
    private bool isUnderground;
    public bool isfinish;

    // Reference to the LevelSequence script
    //public LevelSequenceManager levelSequence;

    void Start()
    {
        currentGridPosition = tilemap.WorldToCell(transform.position);
        CenterOnTile(currentGridPosition);
        targetGridPosition = currentGridPosition;

        currentWalkableTileIDs = abovegroundWalkableTileIDs;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleTilemapManager();
        }

        if (!isMoving)
        {
            HandleInput();
        }

        MovePlayer();

        // Check if the player has reached the Y position of 8.5
        CheckYPosition();
    }

    void CheckYPosition()
    {
        // If the player reaches a Y position of 8.5, trigger the level end sequence
        if (transform.position.y >= 8.5f && isfinish != true)
        {
            //levelSequence.HandleLevelTransition();
            //levelSequence.LoadNextLevel();
            isfinish = true;
        }
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

            if (IsTileWalkable(potentialPosition))
            {
                targetGridPosition = potentialPosition;
                isMoving = true;
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

                HandleInput();
            }
        }
    }

    void ToggleTilemapManager()
    {
        isUnderground = !isUnderground;

        if (isUnderground)
        {
            tileDataManager.gameObject.SetActive(false);
            alternateTileDataManager.gameObject.SetActive(true);
            tileDataManager = alternateTileDataManager;
            currentWalkableTileIDs = undergroundWalkableTileIDs;
        }
        else
        {
            alternateTileDataManager.gameObject.SetActive(false);
            tileDataManager.gameObject.SetActive(true);
            currentWalkableTileIDs = abovegroundWalkableTileIDs;
        }
    }

    bool IsTileWalkable(Vector3Int gridPosition)
    {
        if (gridPosition.x >= 0 && gridPosition.x < tileDataManager.gridWidth &&
            gridPosition.y >= 0 && gridPosition.y < tileDataManager.gridHeight)
        {
            int tileID = tileDataManager.tileDataGrid[gridPosition.x, gridPosition.y].tileType;
            return currentWalkableTileIDs.Contains(tileID);
        }
        return false;
    }

    void CenterOnTile(Vector3Int gridPosition)
    {
        Vector3 tileCenterPosition = tilemap.CellToWorld(gridPosition) + tilemap.cellSize / 2f;
        transform.position = tileCenterPosition;
    }

    public void OnRelocate(Vector3 newPosition)
    {
        Debug.Log("Player is being relocated to position: " + newPosition);
        currentGridPosition = tilemap.WorldToCell(transform.position);
        CenterOnTile(currentGridPosition);
        targetGridPosition = currentGridPosition;
    }
}
