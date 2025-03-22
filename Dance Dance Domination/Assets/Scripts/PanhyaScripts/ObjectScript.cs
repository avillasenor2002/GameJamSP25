using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScript : MonoBehaviour
{
    public bool isPlayer;
    
    public PlayerTrackerScript pscript;

    // Start is called before the first frame update
    void Start()
    {
        pscript = FindObjectOfType<PlayerTrackerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //collision is how i tested state changing and the counter updating
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (tag == "human")
        {
            transform.gameObject.tag = "alien";
            pscript.check();
        }
        
    }
}
