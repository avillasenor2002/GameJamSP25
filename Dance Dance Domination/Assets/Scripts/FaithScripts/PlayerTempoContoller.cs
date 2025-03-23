using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTempoContoller : MonoBehaviour
{
    public TempoBarManager TempoBarManager;
    public TempoZone tempoZone;
    public AudioSource AudioSource;

    public float bpm;
    public float beatThreshold;
    public float changePitchInterval;
    public float secondsPerBeat;

    public static event System.Action OnGlobalBeat;

    private float nextChangeTime;
    private float nextBeatTime;

    public float minBPM = 80f;
    public float maxBPM = 160f;

    private TempoBarShake negativeVisFeedback;
    public GameObject negativeVisSprite;
    public GameObject gray;

    private void Start()
    {
        tempoZone = GetComponentInChildren<TempoZone>();
        bpm = 130f;

        UpdateBeatTiming();

        nextChangeTime = Time.time + changePitchInterval;
        nextBeatTime = Time.time + secondsPerBeat;

        negativeVisFeedback = GetComponentInParent<TempoBarShake>();
        negativeVisSprite.SetActive(false);
    }

    private void Update()
    {
        // Beat logic
        if (Time.time >= nextBeatTime)
        {
            nextBeatTime += secondsPerBeat;
            OnGlobalBeat?.Invoke();
        }

        // Tempo/pitch change logic
        if (Time.time >= nextChangeTime)
        {
            ChangeTempoAndTarget();
            nextChangeTime = Time.time + changePitchInterval;
        }

        // Manual tempo bar interaction
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool isSuccess = CheckHitTiming();
            TempoBarManager.OnHit(isSuccess);
        }
    }

    void ChangeTempoAndTarget()
    {
        bpm = Mathf.Round(Random.Range(minBPM, maxBPM) / 10) * 10;
        AudioSource.pitch = bpm / 120f;

        UpdateBeatTiming();
        nextBeatTime = Time.time + secondsPerBeat;

        // Reposition the tempo zone inside the gray bar
        BoxCollider2D grayCollider = gray.GetComponent<BoxCollider2D>();
        Bounds bounds = grayCollider.bounds;

        float grayMinX = bounds.min.x;
        float grayMaxX = bounds.max.x;
        float newX = Random.Range(grayMinX, grayMaxX);

        Vector3 newTargetPos = new Vector3(newX, tempoZone.transform.position.y, tempoZone.transform.position.z);
        StartCoroutine(SmoothMoveTarget(newTargetPos, 0.5f));
    }

    IEnumerator SmoothMoveTarget(Vector3 newTargetPos, float duration)
    {
        Vector3 startPos = tempoZone.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            tempoZone.transform.position = Vector3.Lerp(startPos, newTargetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tempoZone.transform.position = newTargetPos;
    }

    bool CheckHitTiming()
    {
        float songTime = AudioSource.time;
        float nearestBeatTime = Mathf.Floor(songTime / secondsPerBeat) * secondsPerBeat;
        float difference = Mathf.Abs(songTime - nearestBeatTime);

        if (difference <= beatThreshold)
        {
            return true;
        }
        else
        {
            StartCoroutine(FlashRed());
            negativeVisFeedback.ShakeCamera();
            return false;
        }
    }

    IEnumerator FlashGreen()
    {
        SpriteRenderer sr = negativeVisSprite.GetComponent<SpriteRenderer>();
        Color originColor = sr.color;

        sr.color = Color.green;
        negativeVisSprite.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        negativeVisSprite.SetActive(false);
        sr.color = originColor;
    }

    IEnumerator FlashRed()
    {
        negativeVisSprite.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        negativeVisSprite.SetActive(false);
    }

    void UpdateBeatTiming()
    {
        secondsPerBeat = 60f / bpm;
    }
}
