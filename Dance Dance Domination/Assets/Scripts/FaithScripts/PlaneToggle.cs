using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneToggle : MonoBehaviour
{

    public GameObject planeObject;
    public void OnPressScoreShow()
    {
        if (planeObject != null)
        {
            bool isActive = planeObject.activeSelf;
            planeObject.SetActive(!isActive);
        }
    }
}
