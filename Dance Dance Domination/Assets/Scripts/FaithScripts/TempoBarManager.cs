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

    public float maxSpeed;

    private float velocity = 0f;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        if (!Input.GetKey(KeyCode.Space))
        {
            MoveAwayFromTempoZone();
        }

    }


    void HandleMovement()
    {

        transform.position += new Vector3(velocity * Time.deltaTime, 0, 0);
        transform.position = new Vector3(Mathf.Clamp(transform.position.x, minX, maxX), transform.position.y, transform.position.z);

        velocity = Mathf.Lerp(velocity, 0, Time.deltaTime * 1.5f);

    }

    void MoveAwayFromTempoZone()
    {
        float direction = Mathf.Sign(transform.position.x - TempoZone.position.x); 
        velocity += moveSpeed * direction * Time.deltaTime;
        velocity = Mathf.Clamp(velocity, -maxSpeed, maxSpeed); 
    }


    public void OnHit(bool isSuccess)
    {
        float direction = Mathf.Sign(TempoZone.position.x - transform.position.x); 

        if (isSuccess)
        {
            velocity += successMoveSpeed * direction; //If Success, it leads to the thing
        }
        else
        {
            velocity += failMoveSpeed * -direction; //If Fails, it moves away
        }
        velocity = Mathf.Clamp(velocity, -maxSpeed, maxSpeed);
    }

}
