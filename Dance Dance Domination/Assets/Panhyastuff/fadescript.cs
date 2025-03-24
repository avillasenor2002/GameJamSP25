using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadescript : MonoBehaviour
{
    public float time;
    public AudioSource evilSource;
    public AudioClip evilClip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= 3)
        {
            evilSource.clip = (evilClip);
            evilSource.Play();
        }
    }


}
