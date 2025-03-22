using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempoBarManager : MonoBehaviour
{
    public static TempoBarManager instance;

    //Where the changing area (red block) is
    public Transform TempoZone;


    public float moveSpeed; //Default (negative value, gets away from the zone by default)
    public float successMoveSpeed; //When got it right
    public float failMoveSpeed; //When wrong (negative value)
    public float minX;
    public float maxX;

    private float velocity = 0f;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }

    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }


    void HandleMovement()
    {
        velocity -= moveSpeed * Time.deltaTime;

        transform.position += new Vector3(velocity * Time.deltaTime, 0, 0);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), transform.position.y, transform.position.z);
    }

 
    public void OnHit(bool isSuccess)
    {
        if (isSuccess)
        {
            velocity = successMoveSpeed; //If Success, it leads to the thing
        }
        else
        {
            velocity = failMoveSpeed; //If Fails, it moves away
        }
    }

}
