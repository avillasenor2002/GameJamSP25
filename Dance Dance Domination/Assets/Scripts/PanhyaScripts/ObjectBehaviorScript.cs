using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBehaviorScript : MonoBehaviour
{
    public ObjectScript script;
    public PlayerTrackerScript pscript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if (other.gameObject.CompareTag("human"))
        {
            if (script.collide == true)
            {
                script.isPlayer = true;
                
                pscript.check();
                pscript.humanCount -= 1;
            }
        }
    }
}
