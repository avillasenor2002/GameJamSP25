using System.Collections;
using UnityEngine;

public class HazardIndicatorBlinker : MonoBehaviour
{
    private SpriteRenderer[] spriteRenderers;
    private float blinkDuration;
    private AudioSource audioSource;
    private AudioClip beepClip;

    private bool initialized = false;

    public void InitializeBlink(float duration, AudioSource source, AudioClip clip)
    {
        blinkDuration = duration;
        audioSource = source;
        beepClip = clip;

        if (!initialized)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            initialized = true;
        }

        StartCoroutine(BlinkRoutine());
    }

    private void Start()
    {
        // Ensure sprite renderers are set even if InitializeBlink is called late
        if (!initialized)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            initialized = true;
        }
    }

    private IEnumerator BlinkRoutine()
    {
        float timeElapsed = 0f;
        float blinkRate = 0.3f;

        while (timeElapsed < blinkDuration)
        {
            // Speed up blink rate as time passes
            blinkRate = Mathf.Lerp(0.3f, 0.05f, timeElapsed / blinkDuration);

            // Toggle all sprites
            foreach (var sr in spriteRenderers)
            {
                sr.enabled = !sr.enabled;
            }

            // Play beep sound
            if (audioSource != null && beepClip != null)
            {
                audioSource.PlayOneShot(beepClip);
            }

            yield return new WaitForSeconds(blinkRate);
            timeElapsed += blinkRate;
        }

        // Ensure they stay on at the end
        foreach (var sr in spriteRenderers)
        {
            sr.enabled = true;
        }
    }
}
