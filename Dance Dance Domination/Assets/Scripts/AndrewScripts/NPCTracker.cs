using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class NPCTracker : MonoBehaviour
{
    public Tilemap tilemap; // Assign the same tilemap used by the NPCs
    public KeyCode debugKey = KeyCode.T; // Press T to trigger the reassignment for testing

    void Update()
    {
        if (Input.GetKeyDown(debugKey))
        {
            Debug.Log("Debug key pressed - Evaluating NPCs...");
            HandleReassign();
        }
    }

    void HandleReassign()
    {
        HumanNPC[] allNPCs = FindObjectsOfType<HumanNPC>();
        List<HumanNPC> activeNPCs = new List<HumanNPC>();
        List<HumanNPC> inactiveNPCs = new List<HumanNPC>();

        foreach (HumanNPC npc in allNPCs)
        {
            if (npc.isActive)
            {
                activeNPCs.Add(npc);
            }
            else
            {
                inactiveNPCs.Add(npc);
            }
        }

        Debug.Log($"Active NPCs: {activeNPCs.Count} | Inactive NPCs: {inactiveNPCs.Count}");

        if (activeNPCs.Count == 0)
        {
            Debug.LogWarning("No active NPCs to deactivate.");
            return;
        }

        HumanNPC targetNPC = null;
        int lowestNearbyCount = int.MaxValue;

        foreach (HumanNPC npc in activeNPCs)
        {
            int nearby = CountNearbyNPCsAndPlayers(npc);
            if (nearby < lowestNearbyCount)
            {
                lowestNearbyCount = nearby;
                targetNPC = npc;
            }

            if (lowestNearbyCount == 0) break; // Perfect candidate, early exit
        }

        if (targetNPC != null)
        {
            targetNPC.isActive = false;
            Debug.Log($"NPC {targetNPC.gameObject.name} was set to inactive (Nearby Count = {lowestNearbyCount})");
        }
    }

    int CountNearbyNPCsAndPlayers(HumanNPC npc)
    {
        Vector3Int centerPos = npc.GetCurrentGridPosition();
        int count = 0;

        Vector3Int[] directions = new Vector3Int[]
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.left,
            Vector3Int.right
        };

        HumanNPC[] allNPCs = FindObjectsOfType<HumanNPC>();
        PlayerMovement[] allPlayers = FindObjectsOfType<PlayerMovement>();

        foreach (Vector3Int dir in directions)
        {
            Vector3Int checkPos = centerPos + dir;

            foreach (HumanNPC otherNPC in allNPCs)
            {
                if (otherNPC != npc && otherNPC.GetCurrentGridPosition() == checkPos)
                {
                    count++;
                    break;
                }
            }

            foreach (PlayerMovement player in allPlayers)
            {
                if (player.GetCurrentGridPosition() == checkPos)
                {
                    count++;
                    break;
                }
            }
        }

        return Mathf.Clamp(count, 0, 4);
    }
}
