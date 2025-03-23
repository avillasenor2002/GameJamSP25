using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSequencing : MonoBehaviour
{
    public static SceneSequencing instance;
    public GameObject planeObject;

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

    public void OnPressLoadTitleScene()
    {
        SceneManager.LoadScene("TitleScreen");
    }

    //From Title Scene to main game
    public void OnPressLoadMainGame()
    {
        SceneManager.LoadScene("Andrew");
    }

    //From main game to the end screen
    //Once the timer ends
    public void EndSceneLoad()
    {
        SceneManager.LoadScene("EndScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }



}
