using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TimedHazardSpawner : MonoBehaviour
{
    public GameObject warningIndicatorPrefab;
    public GameObject timedHazardPrefab;
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

                GameObject warning = Instantiate(warningIndicatorPrefab, worldPos, Quaternion.identity);
                HazardIndicatorBlinker blinker = warning.GetComponent<HazardIndicatorBlinker>();
                if (blinker != null)
                {
                    blinker.InitializeBlink(warningDuration, audioSource, warningBeep);
                }

                StartCoroutine(SpawnHazardAfterDelay(worldPos, warningDuration, warning));
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    IEnumerator SpawnHazardAfterDelay(Vector3 worldPos, float delay, GameObject warning)
    {
        yield return new WaitForSeconds(delay);

        if (warning != null)
        {
            Destroy(warning);
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
