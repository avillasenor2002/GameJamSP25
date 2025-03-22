using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RhythmManager : MonoBehaviour
{
    //AudioSource Managing (Pitch Change)
    //Few states where the pitch will change.
    //Three states for now. it will randomly change.

    //currently, correctThreshold is 1.3, mid 1.5. It will fukin depend on what kinda beat sample I have. FUKKK

    public AudioSource audioSource;
    public float bpm;
    public float correctThreshold;
    public float midThreshold;

    private float nextBeatTime;



}
