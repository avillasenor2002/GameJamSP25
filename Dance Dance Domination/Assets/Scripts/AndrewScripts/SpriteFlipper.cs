using System.Collections.Generic;
using UnityEngine;

public class SpriteFlipper : MonoBehaviour
{
    private static bool globalFacingLeft = false;
    private static bool directionChanged = false;

    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();
    private HumanNPC npcScript;
    private PlayerMovement playerScript;
    private bool isNPC = false;
    private bool isPlayer = false;

    void Awake()
    {
        GetComponentsInChildren(true, spriteRenderers);

        npcScript = GetComponent<HumanNPC>();
        playerScript = GetComponent<PlayerMovement>();

        isNPC = npcScript != null;
        isPlayer = playerScript != null;
    }

    void Update()
    {
        // Only player listens to input and sets the global direction
        if (isPlayer)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (!globalFacingLeft)
                {
                    globalFacingLeft = true;
                    directionChanged = true;
                }
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (globalFacingLeft)
                {
                    globalFacingLeft = false;
                    directionChanged = true;
                }
            }
        }

        // Flip this object's sprites if direction changed and valid to flip
        if (directionChanged)
        {
            if (CanFlip())
            {
                FlipSprites(globalFacingLeft);
            }
        }
    }

    void LateUpdate()
    {
        // Reset the flag after all sprites have responded
        if (directionChanged && isPlayer)
        {
            directionChanged = false;
        }
    }

    private bool CanFlip()
    {
        if (isNPC && !npcScript.IsActive())
            return false;

        return true;
    }

    private void FlipSprites(bool faceLeft)
    {
        foreach (var sr in spriteRenderers)
        {
            sr.flipX = faceLeft;
        }
    }
}
