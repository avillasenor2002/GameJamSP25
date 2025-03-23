using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TimedHazardSpawner : MonoBehaviour
{
    public GameObject warningIndicatorPrefab;
    public GameObject timedHazardPrefab;
    public GameObject hazardDropperPrefab; // NEW: Falling visual

    public Tilemap tilemap;
    public TilemapDataAssigner tileDataAssigner;

    public float spawnInterval = 5f;
    public float warningDuration = 1.5f;
    public float hazardLifetime = 1f;

    [Header("Dynamic Hazard Control")]
    public int maxHazardsPerWave = 1;
    private bool stopSpawning = false;

    [Header("Audio")]
    public AudioClip warningBeep;
    public AudioSource audioSource;

    public void StopSpawning()
    {
        stopSpawning = true;
    }

    private void Start()
    {
        StartCoroutine(SpawnHazardsLoop());
    }

    IEnumerator SpawnHazardsLoop()
    {
        while (!stopSpawning)
        {
            for (int i = 0; i < maxHazardsPerWave; i++)
            {
                Vector3Int spawnPos = GetValidSpawnTile();
                if (spawnPos == Vector3Int.zero) continue;

                Vector3 worldPos = tilemap.GetCellCenterWorld(spawnPos);

                // Spawn the warning indicator
                GameObject warning = Instantiate(warningIndicatorPrefab, worldPos, Quaternion.identity);

                HazardIndicatorBlinker blinker = warning.GetComponent<HazardIndicatorBlinker>();
                if (blinker != null)
                {
                    blinker.InitializeBlink(warningDuration, audioSource, warningBeep);
                }

                // Wait for warning duration, then do the fast drop and spawn hazard
                StartCoroutine(HandleDropAndSpawn(worldPos, warning));
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator HandleDropAndSpawn(Vector3 worldPos, GameObject warning)
    {
        yield return new WaitForSeconds(warningDuration);

        if (warning != null)
            Destroy(warning);

        // Fast drop effect right before hazard appears
        if (hazardDropperPrefab != null)
        {
            Vector3 startPos = worldPos + new Vector3(0, 3f, 0);
            GameObject dropper = Instantiate(hazardDropperPrefab, startPos, Quaternion.identity);
            HazardDropperEffect effect = dropper.GetComponent<HazardDropperEffect>();

            if (effect != null)
            {
                float dropDuration = 0.2f;
                effect.Initialize(startPos, worldPos, dropDuration, hazardLifetime);
                yield return new WaitForSeconds(dropDuration);
            }
        }

        GameObject hazard = Instantiate(timedHazardPrefab, worldPos, Quaternion.identity);
        Destroy(hazard, hazardLifetime);
    }

    Vector3Int GetValidSpawnTile()
    {
        BoundsInt bounds = tilemap.cellBounds;
        int attempts = 200;

        for (int i = 0; i < attempts; i++)
        {
            int x = Random.Range(0, tileDataAssigner.gridWidth);
            int y = Random.Range(0, tileDataAssigner.gridHeight);
            Vector3Int pos = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);

            if (tileDataAssigner.tileDataGrid[x, y].tileType == 0 && !IsOccupied(pos))
            {
                return pos;
            }
        }

        return Vector3Int.zero;
    }

    bool IsOccupied(Vector3Int pos)
    {
        foreach (HumanNPC npc in FindObjectsOfType<HumanNPC>())
        {
            if (npc.GetCurrentGridPosition() == pos)
                return true;
        }

        return false;
    }
}
