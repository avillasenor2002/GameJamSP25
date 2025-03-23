using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquishVisualEffect : MonoBehaviour
{
    public Transform targetSprite; // Assign the child sprite in Inspector
    public float idleBounceSpeed = 4f;
    public float idleBounceHeight = 0.05f;
    public float moveBounceHeight = 0.2f;
    public float bounceDuration = 0.15f;

    private Vector3 initialLocalPos;
    private bool isMovingBounce = false;

    void Start()
    {
        if (targetSprite == null)
        {
            Debug.LogWarning($"{name} has no targetSprite assigned to BounceVisualEffect.");
            enabled = false;
            return;
        }

        initialLocalPos = targetSprite.localPosition;
    }

    void Update()
    {
        bool isMoving = false;

        var player = GetComponent<PlayerMovement>();
        var npc = GetComponent<HumanNPC>();

        if (player != null) isMoving = player.IsMoving();
        if (npc != null && npc.IsActive()) isMoving = npc.IsMoving();

        if (!isMovingBounce)
        {
            IdleBounce();
        }

        if (isMoving && !isMovingBounce)
        {
            StartCoroutine(DoMoveBounce());
        }
    }

    void IdleBounce()
    {
        float yOffset = Mathf.Abs(Mathf.Sin(Time.time * idleBounceSpeed)) * idleBounceHeight;
        targetSprite.localPosition = initialLocalPos + new Vector3(0, yOffset, 0);
    }

    IEnumerator DoMoveBounce()
    {
        isMovingBounce = true;

        float elapsed = 0f;
        float halfDuration = bounceDuration / 2f;

        // Bounce up
        while (elapsed < halfDuration)
        {
            float progress = elapsed / halfDuration;
            float yOffset = Mathf.Lerp(0, moveBounceHeight, progress);
            targetSprite.localPosition = initialLocalPos + new Vector3(0, yOffset, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Bounce back to original position
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            float progress = elapsed / halfDuration;
            float yOffset = Mathf.Lerp(moveBounceHeight, 0, progress);
            targetSprite.localPosition = initialLocalPos + new Vector3(0, yOffset, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        targetSprite.localPosition = initialLocalPos;
        isMovingBounce = false;
    }
}
