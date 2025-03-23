using System.Collections;
using UnityEngine;

public class HazardDropperEffect : MonoBehaviour
{
    public Sprite landedSprite;
    public AudioClip impactSound;
    public AudioSource audioSource;

    private SpriteRenderer spriteRenderer;

    public void Initialize(Vector3 startPos, Vector3 endPos, float dropDuration, float fadeOutDuration)
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(DropAndFade(startPos, endPos, dropDuration, fadeOutDuration));
    }

    IEnumerator DropAndFade(Vector3 startPos, Vector3 endPos, float dropDuration, float fadeOutDuration)
    {
        float timer = 0f;
        transform.position = startPos;

        // Drop to target position
        while (timer < dropDuration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, timer / dropDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;

        // On impact
        if (landedSprite != null)
            spriteRenderer.sprite = landedSprite;

        if (audioSource != null && impactSound != null)
            audioSource.PlayOneShot(impactSound);

        // Fade out
        Color originalColor = spriteRenderer.color;
        timer = 0f;

        while (timer < fadeOutDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }
}
