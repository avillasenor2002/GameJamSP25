using System.Collections;
using UnityEngine;

public class PlayerSquishVisualEffect : MonoBehaviour
{
    [Header("Sprite & Animation")]
    public Transform targetSprite;          // Main sprite to bounce
    public Transform secondarySprite;       // NEW: Second sprite to bounce alongside
    public SpriteRenderer spriteRenderer1;
    public SpriteRenderer spriteRenderer2;
    public Sprite activeSprite1A;
    public Sprite activeSprite1B;
    public Sprite activeSprite2A;
    public Sprite activeSprite2B;

    [Header("Bounce Settings")]
    public float activeBounceHeight = 0.05f;
    public float idleBounceHeight = 0.01f;
    public float squashAmount = 0.05f;
    public float moveBounceHeight = 0.15f;
    public float beatBounceDuration = 0.12f;

    private HumanNPC npc;
    private PlayerMovement player;
    private Vector3 initialPos;
    private Vector3 initialScale;
    private Vector3 secondaryInitialPos;

    private bool isMovingBounce = false;
    private bool toggleSprite = false;
    private int beatCounter = 0;

    void Start()
    {
        initialPos = targetSprite.localPosition;
        initialScale = targetSprite.localScale;

        if (secondarySprite != null)
            secondaryInitialPos = secondarySprite.localPosition;

        npc = GetComponent<HumanNPC>();
        player = GetComponent<PlayerMovement>();

        PlayerTempoContoller.OnGlobalBeat += OnGlobalBeat;
    }

    void OnDestroy()
    {
        PlayerTempoContoller.OnGlobalBeat -= OnGlobalBeat;
    }

    void OnGlobalBeat()
    {
        if (isMovingBounce) return;

        bool isActive = (player != null) || (npc != null && npc.IsActive());
        float bounceHeight = isActive ? activeBounceHeight : idleBounceHeight;
        float squash = 1 - squashAmount;
        float stretch = 1 + squashAmount;

        // Bounce both sprites
        targetSprite.localPosition = initialPos + new Vector3(0, bounceHeight, 0);
        targetSprite.localScale = new Vector3(initialScale.x * squash, initialScale.y * stretch, initialScale.z);

        if (secondarySprite != null)
            secondarySprite.localPosition = secondaryInitialPos + new Vector3(0, bounceHeight, 0);

        StartCoroutine(ResetBounce(beatBounceDuration));

        // Sprite flip logic
        if (isActive)
        {
            beatCounter++;
            if (beatCounter % 2 == 0)
            {
                toggleSprite = !toggleSprite;

                if (spriteRenderer1 != null && activeSprite1A != null && activeSprite1B != null)
                    spriteRenderer1.sprite = toggleSprite ? activeSprite1A : activeSprite1B;

                if (spriteRenderer2 != null && activeSprite2A != null && activeSprite2B != null)
                    spriteRenderer2.sprite = toggleSprite ? activeSprite2A : activeSprite2B;
            }
        }
    }

    IEnumerator ResetBounce(float delay)
    {
        yield return new WaitForSeconds(delay);

        targetSprite.localPosition = initialPos;
        targetSprite.localScale = initialScale;

        if (secondarySprite != null)
            secondarySprite.localPosition = secondaryInitialPos;
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
            float height = Mathf.Lerp(0, moveBounceHeight, progress);

            targetSprite.localPosition = initialPos + new Vector3(0, height, 0);
            if (secondarySprite != null)
                secondarySprite.localPosition = secondaryInitialPos + new Vector3(0, height, 0);

            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;
        while (time < half)
        {
            float progress = time / half;
            float height = Mathf.Lerp(moveBounceHeight, 0, progress);

            targetSprite.localPosition = initialPos + new Vector3(0, height, 0);
            if (secondarySprite != null)
                secondarySprite.localPosition = secondaryInitialPos + new Vector3(0, height, 0);

            time += Time.deltaTime;
            yield return null;
        }

        targetSprite.localPosition = initialPos;
        if (secondarySprite != null)
            secondarySprite.localPosition = secondaryInitialPos;

        isMovingBounce = false;
    }
}
