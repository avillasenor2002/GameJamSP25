using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class NPCTracker : MonoBehaviour
{
    public Tilemap tilemap;
    public TilemapDataAssigner tileDataAssigner;

    [Header("NPC Spawning")]
    public GameObject npcPrefab;
    public float minSpawnDelay = 3f;
    public float maxSpawnDelay = 10f;
    public int maxActiveNPCs = 20;
    public float delayBeforeActiveNPCRemoval = 3f;

    [Header("Cleanup Visual Effects")]
    public Image fadeImage;
    public float fadeDuration = 1f;
    public float floatSpeed = 2f;

    public bool fusioned;
    public bool cleanupInProgress = false;
    private bool spawningAllowed = true;

    public AudioControllingforSFX audioEvent;

    public List<GameObject> NPCPrefabs = new List<GameObject>();
    public Material whiteSilhouetteMaterial; // Material assigned in inspector

    private int fusionCount = 0;
    public Sprite newPlayerSprite; // Assign in Inspector

    private void Start()
    {
        fusioned = false;
        audioEvent = GetComponent<AudioControllingforSFX>();
        StartCoroutine(SpawnNPCsRandomly());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            HandleReassign();
        }

        if (!cleanupInProgress && CountActiveNPCs() >= maxActiveNPCs)
        {
            fusioned = true;
            StartCoroutine(CleanupAndDisableMovement());
        }
    }

    IEnumerator SpawnNPCsRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));

            if (!spawningAllowed) continue;

            Vector3Int spawnPos = FindValidSpawnTile();

            if (spawnPos != Vector3Int.zero)
            {
                Vector3 worldPos = tilemap.GetCellCenterWorld(spawnPos);
                Instantiate(NPCPrefabs[Random.Range(0, 4)], worldPos, Quaternion.identity);
            }
        }
    }

    Vector3Int FindValidSpawnTile()
    {
        BoundsInt bounds = tilemap.cellBounds;
        int attempts = 200;

        for (int i = 0; i < attempts; i++)
        {
            int x = Random.Range(0, tileDataAssigner.gridWidth);
            int y = Random.Range(0, tileDataAssigner.gridHeight);
            Vector3Int cellPos = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);

            if (IsTileWalkable(cellPos) && !IsTileOccupied(cellPos))
            {
                return cellPos;
            }
        }

        return Vector3Int.zero;
    }

    bool IsTileWalkable(Vector3Int pos)
    {
        int x = pos.x - tilemap.cellBounds.xMin;
        int y = pos.y - tilemap.cellBounds.yMin;

        if (x < 0 || y < 0 || x >= tileDataAssigner.gridWidth || y >= tileDataAssigner.gridHeight)
            return false;

        return tileDataAssigner.tileDataGrid[x, y].tileType == 0;
    }

    bool IsTileOccupied(Vector3Int pos)
    {
        foreach (HumanNPC npc in FindObjectsOfType<HumanNPC>())
        {
            if (npc.GetCurrentGridPosition() == pos)
                return true;
        }

        foreach (PlayerMovement player in FindObjectsOfType<PlayerMovement>())
        {
            if (player.GetCurrentGridPosition() == pos)
                return true;
        }

        return false;
    }

    void HandleReassign()
    {
        HumanNPC[] allNPCs = FindObjectsOfType<HumanNPC>();
        List<HumanNPC> active = new List<HumanNPC>();
        List<HumanNPC> inactive = new List<HumanNPC>();

        foreach (HumanNPC npc in allNPCs)
        {
            if (npc.isActive)
                active.Add(npc);
            else
                inactive.Add(npc);
        }

        if (active.Count == 0) return;

        HumanNPC targetNPC = null;
        int lowestCount = int.MaxValue;

        foreach (HumanNPC npc in active)
        {
            int count = CountNearby(npc);
            if (count < lowestCount)
            {
                lowestCount = count;
                targetNPC = npc;
            }

            if (lowestCount == 0) break;
        }

        if (targetNPC != null)
        {
            targetNPC.isActive = false;
        }
    }

    int CountNearby(HumanNPC npc)
    {
        Vector3Int pos = npc.GetCurrentGridPosition();
        int count = 0;
        Vector3Int[] dirs = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        foreach (Vector3Int dir in dirs)
        {
            Vector3Int check = pos + dir;

            foreach (HumanNPC other in FindObjectsOfType<HumanNPC>())
            {
                if (other != npc && other.GetCurrentGridPosition() == check)
                {
                    count++;
                    break;
                }
            }

            foreach (PlayerMovement p in FindObjectsOfType<PlayerMovement>())
            {
                if (p.GetCurrentGridPosition() == check)
                {
                    count++;
                    break;
                }
            }
        }

        return Mathf.Clamp(count, 0, 4);
    }

    public int CountActiveNPCs()
    {
        int count = 0;
        foreach (HumanNPC npc in FindObjectsOfType<HumanNPC>())
        {
            if (npc.isActive) count++;
        }
        return count;
    }

    IEnumerator CleanupAndDisableMovement()
    {
        audioEvent.MuteAllExceptFusion();
        cleanupInProgress = true;
        spawningAllowed = false;

        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        Vector3 playerCenter = player.transform.position;

        HumanNPC[] npcs = FindObjectsOfType<HumanNPC>();

        Dictionary<SpriteRenderer, Material> originalMaterials = new Dictionary<SpriteRenderer, Material>();

        // Active NPCs: replace material
        foreach (HumanNPC npc in npcs)
        {
            npc.enabled = false;
            if (npc.isActive)
            {
                foreach (SpriteRenderer sr in npc.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (sr != null && !originalMaterials.ContainsKey(sr))
                    {
                        originalMaterials[sr] = sr.material;
                        sr.material = whiteSilhouetteMaterial;
                    }
                }
            }
        }

        // Player: replace material
        player.enabled = false;
        foreach (SpriteRenderer sr in player.GetComponentsInChildren<SpriteRenderer>())
        {
            if (sr != null && !originalMaterials.ContainsKey(sr))
            {
                originalMaterials[sr] = sr.material;
                sr.material = whiteSilhouetteMaterial;
            }
        }

        StartCoroutine(FadeImage(true));

        foreach (var npc in npcs)
        {
            if (npc.isActive)
            {
                StartCoroutine(MoveToPosition(npc.transform, playerCenter));
            }
        }

        foreach (HumanNPC npc in npcs)
        {
            if (!npc.isActive)
                Destroy(npc.gameObject);
        }

        yield return new WaitForSeconds(delayBeforeActiveNPCRemoval);

        foreach (HumanNPC npc in FindObjectsOfType<HumanNPC>())
        {
            Destroy(npc.gameObject);
        }

        yield return new WaitForSeconds(0.2f);

        // Restore player and NPC materials
        foreach (var kvp in originalMaterials)
        {
            if (kvp.Key != null)
                kvp.Key.material = kvp.Value;
        }

        player.enabled = true;

        StartCoroutine(FadeImage(false));
        cleanupInProgress = false;
        spawningAllowed = true;
        audioEvent.RestoreAll();

        // Upgrade player visual after second fusion
        fusionCount++;
        if (fusionCount >= 2)
        {
            player.UpgradePlayerVisuals(); // This function should handle new sprite logic
        }
    }

    IEnumerator FadeImage(bool fadeIn)
    {
        if (fadeImage == null) yield break;

        float t = 0f;
        Color color = fadeImage.color;
        float startAlpha = fadeIn ? 0 : 1;
        float endAlpha = fadeIn ? 1 : 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float blend = Mathf.Clamp01(t / fadeDuration);
            color.a = Mathf.Lerp(startAlpha, endAlpha, blend);
            fadeImage.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadeImage.color = color;
    }

    IEnumerator MoveToPosition(Transform obj, Vector3 target)
    {
        while (Vector3.Distance(obj.position, target) > 0.05f)
        {
            obj.position = Vector3.MoveTowards(obj.position, target, floatSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
