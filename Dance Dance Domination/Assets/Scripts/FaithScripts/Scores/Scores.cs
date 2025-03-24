using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scores : MonoBehaviour
{

    public static Scores instance;
    public NPCTracker npcTracker;

    //number of humans added
    public int currentScore;
    //score added when fusion happens.
    public int fusionBonusScore;
    private int lastNPCCount = 0;

    public TextMeshProUGUI scoreUI;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentScore = 0;
        fusionBonusScore = 1000;
    }

    private void Update()
    {
        scoreUI.text = "Score: "+ currentScore;

        int currentNPCCount = npcTracker.CountActiveNPCs();
        int newlyAdded = currentNPCCount - lastNPCCount;

        if (newlyAdded > 0)
        {
            currentScore += newlyAdded * 100;
        }

        lastNPCCount = currentNPCCount;

        if (npcTracker.fusioned)
        {
            currentScore += fusionBonusScore;
            npcTracker.fusioned = false;
        }

    }
}
