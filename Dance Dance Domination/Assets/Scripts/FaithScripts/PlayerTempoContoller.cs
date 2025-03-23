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

    private float secondsPerBeat;
    private float nextChangeTime;

    public float minBPM = 80f;
    public float maxBPM = 160f;

    private TempoBarShake negativeVisFeedback;
    public GameObject negativeVisSprite;

    public GameObject gray;



    private void Start()
    {
        tempoZone = GetComponentInChildren<TempoZone>();

        bpm = 130f;
        secondsPerBeat = 60f / bpm;
        nextChangeTime = Time.time + changePitchInterval;
        negativeVisFeedback = GetComponentInParent<TempoBarShake>();
        negativeVisSprite.SetActive(false);
    }

    private void Update()
    {
        if (Time.time >= nextChangeTime)
        {
            ChangeTempoAndTarget(); 
            nextChangeTime += changePitchInterval;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool isSuccess = CheckHitTiming();
            TempoBarManager.OnHit(isSuccess);
        }
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

    void ChangeTempoAndTarget()
    {
        bpm = Mathf.Round(Random.Range(minBPM, maxBPM) / 10) * 10;

        AudioSource.pitch = bpm / 120f;
        UpdateBeatTiming();

        BoxCollider2D grayCollider = gray.GetComponent<BoxCollider2D>();
        Bounds bounds = grayCollider.bounds;

        float grayMinX = bounds.min.x;
        float grayMaxX = bounds.max.x;

        float newX = Random.Range(grayMinX, grayMaxX);

        Vector3 newTargetPos = new Vector3(newX, tempoZone.transform.position.y, tempoZone.transform.position.z);
        tempoZone.transform.position = newTargetPos;

        StartCoroutine(SmoothMoveTarget(newTargetPos, 0.5f));
     }


    bool CheckHitTiming()
    {
        //see howmuch time passed
        float songTime = AudioSource.time;
        float nearestBeatTime = Mathf.Floor(songTime / secondsPerBeat) * secondsPerBeat;
        float difference = Mathf.Abs(songTime - nearestBeatTime);

        if (difference <= beatThreshold)
        {
            //StartCoroutine(FlashGreen());
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
