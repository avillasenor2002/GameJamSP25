using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempoBarManager : MonoBehaviour
{
    //Where the changing area (red block) is
    public TempoZone tempoZone;

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

    public GameObject gray;
    private float lastNPCKillTime = 0f;

    Bounds bounds;

    private void Start()
    {
        BoxCollider2D grayCollider = gray.GetComponent<BoxCollider2D>();
        bounds = grayCollider.bounds;
 
        lastInputTime = Time.time;

        minX = bounds.min.x;
        maxX = bounds.max.x;

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

        if (Time.time - lastInputTime >= missTime)
        {
            lastInputTime = Time.time;
            OnHit(false);


        }
        if (isOutforLong() && Time.time - lastNPCKillTime >= secondsPlayerWasOut)
        {
            HumanNPC.RemoveLastNPC();
            lastNPCKillTime = Time.time; 
        }
    }


    void HandleMovement()
    {
        transform.position += new Vector3(velocity * Time.deltaTime, 0, 0);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float boxCenter = sr.bounds.extents.x;
        float clampedX = Mathf.Clamp(transform.position.x, minX + boxCenter, maxX - boxCenter);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);

        velocity = Mathf.Lerp(velocity, 0, Time.deltaTime * 1.5f);
    }

    void MoveAwayFromTempoZone()
    {
        float distance = transform.position.x - tempoZone.transform.position.x;
        float direction = Mathf.Sign(distance); 

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
        return tempoZone.IsInside(transform.position);
    }

    //I placed this in the Updates so that it goes along with the real-time.
    //isOutforLong will be true if the player is out of the redzone of the bar for certain amount of seconds.
    //once player is back into the zone, then the time will become = 0 (reset), function will return 0.

    // usage: if(isOutforLong()) { //Add penalty here }
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
            if (timeAwayFromZone > 0f)
            {
                timeAwayFromZone = 0;

            }
        }
        return false;
    }
    bool IsFarFromTempoZone()
    {
        float buffer = 0.1f;
        Bounds bounds = tempoZone.GetComponent<BoxCollider2D>().bounds;

        float left = bounds.min.x - buffer;
        float right = bounds.max.x + buffer;

        return transform.position.x < left || transform.position.x > right;
    }

    public void OnHit(bool isSuccess)
    {
        float direction = Mathf.Sign(tempoZone.transform.position.x - transform.position.x); 

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
