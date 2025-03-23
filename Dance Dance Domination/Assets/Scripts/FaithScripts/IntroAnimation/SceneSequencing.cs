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
    public void OnPressLoadMainGame()
    {
        SceneManager.LoadScene("Andrew", LoadSceneMode.Single);
    }

    //From main game to the end screen
    //Once the timer ends
    public void EndSceneLoad()
    {
        SceneManager.LoadScene("EndScene", LoadSceneMode.Single);
    }
}
