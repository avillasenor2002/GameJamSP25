using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehaviorScript : MonoBehaviour
{
    //this script does not do anything lol
    public ObjectScript script;
    public PlayerTrackerScript pscript;
    // Start is called before the first frame update
    void Start()
    {
        script = FindObjectOfType<ObjectScript>();
        pscript = FindObjectOfType<PlayerTrackerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
        //if (other.gameObject.CompareTag("human"))
        //{
        //    if (script.collide == true)
        //    {
                
                
        //        pscript.check();
                
        //    }
        //}
    }
}
