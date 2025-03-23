using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEntryCall : MonoBehaviour
{
    public EntryEnter entryEnter;

    // Start is called before the first frame update
    void Start()
    {
        if(entryEnter != null)
        {
            entryEnter.ShowEntryUI();
        }
    }
}
