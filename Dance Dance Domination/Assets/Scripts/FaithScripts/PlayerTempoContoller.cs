using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTempoContoller : MonoBehaviour
{
    public TempoBarManager TempoBarManager;
    //public Transform failZone;
    public Transform targetZone;
    public AudioSource AudioSource;

    public float bpm;
    public float beatThreshold;
    public float changePitchInterval;

    private float secondsPerBeat;
    private float songStartTime;
    private float nextChangeTime;

    public float minBPM = 80f;
    public float maxBPM = 160f;


    private void Start()
    {
        bpm = 130f;
        secondsPerBeat = 60f / bpm;
        songStartTime = Time.time;
    }

    private void Update()
    {

        if (Time.time >= nextChangeTime)
        {
            ChangeTempoAndTarget(); 
            nextChangeTime = Time.time + changePitchInterval;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            bool isSuccess = CheckHitTiming();
            TempoBarManager.OnHit(isSuccess);
        }
    }

    void ChangeTempoAndTarget()
    {
        bpm = Random.Range(minBPM, maxBPM); 
        AudioSource.pitch = bpm / 120f; 
        UpdateBeatTiming();

        Vector3 newTargetPos = new Vector3(Random.Range(-2f, 2f), targetZone.position.y, targetZone.position.z);
        targetZone.position = newTargetPos;

        Debug.Log($" 새 BPM: {bpm}, 목표 영역 이동: {targetZone.position}");
    }


    bool CheckHitTiming()
    {
        //see howmuch time passed
        float songTime = Time.time - songStartTime;
        float nearestBeatTime = Mathf.Round(songTime / secondsPerBeat) * secondsPerBeat;
        float difference = Mathf.Abs(songTime - nearestBeatTime);


        if (difference <= beatThreshold)
        {
            Debug.Log("true");
            return true;
        }
        else
        {
            Debug.Log("false");
            return false;
        }
    }

    void UpdateBeatTiming()
    {
        secondsPerBeat = 60f / bpm; 
        songStartTime = Time.time;
    }

}
