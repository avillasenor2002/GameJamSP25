using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrackerScript : MonoBehaviour
{
    public List<ObjectScript> humans;
    public int playerCount;
    public int humanCount;
    public GameObject[] npcs;
    public GameObject[] player;

    // public ObjectScript objectScript;

    // Start is called before the first frame update
    void Start()
    {

        npcs = GameObject.FindGameObjectsWithTag("human");
        player = GameObject.FindGameObjectsWithTag("alien");

        check();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void check()
    {
        npcs = GameObject.FindGameObjectsWithTag("human");
        player = GameObject.FindGameObjectsWithTag("alien");
        humanCount = npcs.Length;
        playerCount = player.Length;

        //foreach (ObjectScript script in humans)
        //{
        //    if (script.isPlayer == true)
        //    {
        //        playerCount += 1;
        //        Debug.Log(playerCount);
        //    }
        //    if (script.isPlayer == false)
        //    {
        //        humanCount += 1;
        //        Debug.Log(humanCount);
        //    }

        //}
    }
}
