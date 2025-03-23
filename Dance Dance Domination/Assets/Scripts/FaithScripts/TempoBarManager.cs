using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempoBarManager : MonoBehaviour
{
    //Where the changing area (red block) is
    public Transform TempoZone;

    public float moveSpeed; //Default (negative value, gets away from the zone by default)
    public float successMoveSpeed; //When got it right
    public float failMoveSpeed; //When wrong (negative value)
    public float minX;
    public float maxX;

    public float maxSpeed;

    private float velocity = 0f;

    public float missTime = 1.0f;
    private float lastInputTime;

    private float timeAwayFromZone;
    //how many seconds the player has been in the zone
    public float secondsPlayerWasOut = 4.0f;

    private void Start()
    {
        lastInputTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        if (!Input.GetKey(KeyCode.Space))
        {
            MoveAwayFromTempoZone();
        }
        else
        {
            lastInputTime = Time.time;
        }

        if(Time.time - lastInputTime >= missTime)
        {
            lastInputTime = Time.time;
            OnHit(false);
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

        if (IsInsideTempoZone())
        {
            velocity += moveSpeed * direction * 3f * Time.deltaTime;
        }
        else
        {
            velocity += moveSpeed * direction * 2f * Time.deltaTime; 
        }

        velocity = Mathf.Clamp(velocity, -maxSpeed, maxSpeed);
    }

    bool IsInsideTempoZone()
    {
        float zoneLeft = TempoZone.position.x - (TempoZone.localScale.x / 2);
        float zoneRight = TempoZone.position.x + (TempoZone.localScale.x / 2);

        return transform.position.x >= zoneLeft && transform.position.x <= zoneRight;
    }

    //I placed this in the Updates so that it goes along with the real-time.
    //isOutforLong will be true if the player is out of the redzone of the bar for certain amount of seconds.
    //once player is back into the zone, then the time will become = 0 (reset), function will return 0.
    
    // usage: if(TempoBarManager.isOutforLong()) { //Add penalty here }
    public bool isOutforLong()
    {
        if (!IsInsideTempoZone())
        {
            timeAwayFromZone += Time.deltaTime;

            if (timeAwayFromZone >= secondsPlayerWasOut)
            {
                return true;
            }
        }
        else
        {
            timeAwayFromZone = 0;
        }
        return false;
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
            velocity += failMoveSpeed * 2;
        }
        velocity = Mathf.Clamp(velocity, -maxSpeed, maxSpeed);
    }

}
