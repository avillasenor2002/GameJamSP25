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
    private int beatCounter = 0;

    void Start()
    {
        initialPos = targetSprite.localPosition;
        initialScale = targetSprite.localScale;

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

        targetSprite.localPosition = initialPos + new Vector3(0, bounceHeight, 0);
        targetSprite.localScale = new Vector3(initialScale.x * squash, initialScale.y * stretch, initialScale.z);
        StartCoroutine(ResetBounce(beatBounceDuration));

        if (isActive)
        {
            beatCounter++;
            if (activeSprite1 != null && activeSprite2 != null && beatCounter % 2 == 0)
            {
                toggleSprite = !toggleSprite;
                spriteRenderer.sprite = toggleSprite ? activeSprite1 : activeSprite2;
            }
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
