using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrackerScript : MonoBehaviour
{
    public List<ObjectScript> humans;
    public int playerCount;
    public int humanCount;
    
   // public ObjectScript objectScript;
    
    // Start is called before the first frame update
    void Start()
    {
        


        check();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void check()
    {
        foreach (ObjectScript script in humans)
        {
            if (script.isPlayer == true)
            {
                playerCount += 1;
                Debug.Log(playerCount);
            }
            if (script.isPlayer == false)
            {
                humanCount += 1;
                Debug.Log(humanCount);
            }

        }
    }
}
