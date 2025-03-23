using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scores : MonoBehaviour
{

    public Scores instance;

    //total Score
    public int totalScore;

    //number of humans added
    public int currentScore;
    //score added when fusion happens.
    public int fusionBonusScore;


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
    }

    // Start is called before the first frame update

    //Get the counts from the... idk um.... the list.

    //Once the fusion happens, add extra bonus point :-3.




}
