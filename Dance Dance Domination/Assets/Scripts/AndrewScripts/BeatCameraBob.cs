using System.Collections;
using UnityEngine;

public class BeatCameraBob : MonoBehaviour
{
    public float bobAmount = 0.1f;        // How far the object moves up/down
    public float bobDuration = 0.15f;     // How quickly it moves up/down

    private Vector3 originalPosition;
    private Coroutine bobCoroutine;

    void Start()
    {
        originalPosition = transform.localPosition;

        // Auto-subscribe to global beat
        PlayerTempoContoller.OnGlobalBeat += OnBeat;
    }

    void OnDestroy()
    {
        // Clean up event subscription
        PlayerTempoContoller.OnGlobalBeat -= OnBeat;
    }

    void OnBeat()
    {
        if (bobCoroutine != null)
            StopCoroutine(bobCoroutine);

        bobCoroutine = StartCoroutine(Bob());
    }

    IEnumerator Bob()
    {
        // Move down slightly
        Vector3 targetPos = originalPosition - new Vector3(0, bobAmount, 0);
        float elapsed = 0f;

        while (elapsed < bobDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, targetPos, elapsed / bobDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPos;

        // Move back up
        elapsed = 0f;
        while (elapsed < bobDuration)
        {
            transform.localPosition = Vector3.Lerp(targetPos, originalPosition, elapsed / bobDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
}
