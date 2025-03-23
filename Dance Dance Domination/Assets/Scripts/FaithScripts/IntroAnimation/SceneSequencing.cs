using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSequencing : MonoBehaviour
{
    public static SceneSequencing instance;

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


    //From Title Scene to main game

    //From Title Scene to 

    //From main game to the end screen

    public void EndSceneLoad()
    {

    }


    //From 

}
