using System.Collections;
using UnityEngine;

public class TitleScreenBeatCameraBob : MonoBehaviour
{
    [Header("Bob Animation")]
    public float bobAmount = 0.1f;        // How far the camera moves down
    public float bobDuration = 0.15f;     // Duration of downward/upward movement

    [Header("Beat Timing")]
    public float bpm = 100f;              // Beats per minute
    private float secondsPerBeat;

    private Vector3 originalPosition;
    private Coroutine bobCoroutine;

    void Start()
    {
        originalPosition = transform.localPosition;
        secondsPerBeat = 60f / bpm;

        // Start local beat loop
        StartCoroutine(BeatLoop());
    }

    IEnumerator BeatLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsPerBeat);
            TriggerBeat();
        }
    }

    void TriggerBeat()
    {
        if (bobCoroutine != null)
            StopCoroutine(bobCoroutine);

        bobCoroutine = StartCoroutine(Bob());
    }

    IEnumerator Bob()
    {
        Vector3 targetPos = originalPosition - new Vector3(0, bobAmount, 0);
        float elapsed = 0f;

        while (elapsed < bobDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, targetPos, elapsed / bobDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPos;

        elapsed = 0f;
        while (elapsed < bobDuration)
        {
            transform.localPosition = Vector3.Lerp(targetPos, originalPosition, elapsed / bobDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    // Optional: live update BPM
    public void SetBPM(float newBPM)
    {
        bpm = newBPM;
        secondsPerBeat = 60f / bpm;
    }
}
