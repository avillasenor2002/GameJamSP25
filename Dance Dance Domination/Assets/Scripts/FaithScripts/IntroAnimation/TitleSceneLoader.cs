using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneLoader : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.LoadScene("TitleScreen");
    }

}
