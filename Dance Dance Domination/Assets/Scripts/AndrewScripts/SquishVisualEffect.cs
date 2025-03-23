using System.Collections;
using UnityEngine;

public class SquishVisualEffect : MonoBehaviour
{
    [Header("Sprite & Animation")]
    public Transform targetSprite;
    public SpriteRenderer spriteRenderer;
    public Sprite activeSprite1;
    public Sprite activeSprite2;

    [Header("Bounce Settings")]
    public float bpm = 120f;
    public float activeBounceHeight = 0.05f;
    public float idleBounceHeight = 0.01f;
    public float squashAmount = 0.05f;
    public float moveBounceHeight = 0.15f;
    public float beatBounceDuration = 0.12f;

    private HumanNPC npc;
    private PlayerMovement player;
    private Vector3 initialPos;
    private Vector3 initialScale;
    private bool isMovingBounce = false;
    private bool toggleSprite = false;

    private static float lastGlobalBeatTime = 0f;
    private static float beatInterval => 60f / globalBPM;
    private static float globalBPM = 120f;

    void Start()
    {
        initialPos = targetSprite.localPosition;
        initialScale = targetSprite.localScale;

        npc = GetComponent<HumanNPC>();
        player = GetComponent<PlayerMovement>();

        globalBPM = bpm; // Shared static for syncing if needed
    }

    public void OnBeat()
    {
        if (isMovingBounce) return;

        bool isActive = (player != null) || (npc != null && npc.IsActive());
        float bounceHeight = isActive ? activeBounceHeight : idleBounceHeight;
        float squash = 1 - squashAmount;
        float stretch = 1 + squashAmount;

        // Apply bounce
        targetSprite.localPosition = initialPos + new Vector3(0, bounceHeight, 0);
        targetSprite.localScale = new Vector3(initialScale.x * squash, initialScale.y * stretch, initialScale.z);
        StartCoroutine(ResetBounce(beatBounceDuration));

        // Flip sprite every 1 beat (based on global beat count)
        if (isActive && Time.frameCount % 1 == 0 && activeSprite1 != null && activeSprite2 != null)
        {
            toggleSprite = !toggleSprite;
            spriteRenderer.sprite = toggleSprite ? activeSprite1 : activeSprite2;
        }
    }

    IEnumerator ResetBounce(float delay)
    {
        yield return new WaitForSeconds(delay);
        targetSprite.localPosition = initialPos;
        targetSprite.localScale = initialScale;
    }

    public void TriggerMoveBounce()
    {
        if (!isMovingBounce)
        {
            StartCoroutine(MoveBounce());
        }
    }

    IEnumerator MoveBounce()
    {
        isMovingBounce = true;

        float time = 0f;
        float half = 0.1f;
        while (time < half)
        {
            float progress = time / half;
            targetSprite.localPosition = initialPos + new Vector3(0, Mathf.Lerp(0, moveBounceHeight, progress), 0);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;
        while (time < half)
        {
            float progress = time / half;
            targetSprite.localPosition = initialPos + new Vector3(0, Mathf.Lerp(moveBounceHeight, 0, progress), 0);
            time += Time.deltaTime;
            yield return null;
        }

        targetSprite.localPosition = initialPos;
        isMovingBounce = false;
    }
}
