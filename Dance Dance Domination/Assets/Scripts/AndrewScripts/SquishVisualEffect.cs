using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquishVisualEffect : MonoBehaviour
{
    public Transform targetSprite; // Assign the child sprite in Inspector
    public SpriteRenderer spriteRenderer; // Assign the SpriteRenderer of the sprite
    public Sprite activeSprite1; // Assigned in Inspector
    public Sprite activeSprite2; // Assigned in Inspector

    public float idleBounceSpeed = 4f;
    public float idleBounceHeight = 0.05f;
    public float moveBounceHeight = 0.2f;
    public float bounceDuration = 0.15f;

    public float activeBounceSpeed = 0.1f; // Fast bounce when active

    private Vector3 initialLocalPos;
    private bool isMovingBounce = false;

    private float spriteSwitchTimer = 0f;
    private bool toggleSprite = false;

    private HumanNPC npc;

    void Start()
    {
        if (targetSprite == null)
        {
            Debug.LogWarning($"{name} has no targetSprite assigned to SquishVisualEffect.");
            enabled = false;
            return;
        }

        npc = GetComponent<HumanNPC>();
        initialLocalPos = targetSprite.localPosition;
    }

    void Update()
    {
        bool isMoving = false;

        var player = GetComponent<PlayerMovement>();

        if (player != null) isMoving = player.IsMoving();
        if (npc != null && npc.IsActive()) isMoving = npc.IsMoving();

        if (!isMovingBounce)
        {
            if (npc != null && npc.IsActive())
            {
                ActiveBounce();
                SpriteFlipEffect();
            }
            else
            {
                IdleBounce();
            }
        }

        if (isMoving && !isMovingBounce)
        {
            StartCoroutine(DoMoveBounce(npc != null && npc.IsActive()));
        }
    }

    void IdleBounce()
    {
        float yOffset = Mathf.Abs(Mathf.Sin(Time.time * idleBounceSpeed)) * idleBounceHeight;
        targetSprite.localPosition = initialLocalPos + new Vector3(0, yOffset, 0);
    }

    void ActiveBounce()
    {
        float yOffset = Mathf.Abs(Mathf.Sin(Time.time * (1f / activeBounceSpeed))) * idleBounceHeight;
        targetSprite.localPosition = initialLocalPos + new Vector3(0, yOffset, 0);
    }

    void SpriteFlipEffect()
    {
        if (spriteRenderer == null || activeSprite1 == null || activeSprite2 == null) return;

        spriteSwitchTimer += Time.deltaTime;
        if (spriteSwitchTimer >= activeBounceSpeed)
        {
            toggleSprite = !toggleSprite;
            spriteRenderer.sprite = toggleSprite ? activeSprite1 : activeSprite2;
            spriteSwitchTimer = 0f;
        }
    }

    IEnumerator DoMoveBounce(bool fastBounce = false)
    {
        isMovingBounce = true;

        float bounceHeight = moveBounceHeight;
        float duration = fastBounce ? bounceDuration * 0.6f : bounceDuration;
        float halfDuration = duration / 2f;

        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            float progress = elapsed / halfDuration;
            float yOffset = Mathf.Lerp(0, bounceHeight, progress);
            targetSprite.localPosition = initialLocalPos + new Vector3(0, yOffset, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            float progress = elapsed / halfDuration;
            float yOffset = Mathf.Lerp(bounceHeight, 0, progress);
            targetSprite.localPosition = initialLocalPos + new Vector3(0, yOffset, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        targetSprite.localPosition = initialLocalPos;
        isMovingBounce = false;
    }
}
