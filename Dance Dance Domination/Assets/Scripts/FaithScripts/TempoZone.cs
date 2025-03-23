using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempoZone : MonoBehaviour
{
    public GameObject gray;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();   
    }

    private void Start()
    {
        BoxCollider2D grayCollider = gray.GetComponent<BoxCollider2D>();
        
        Bounds bounds = grayCollider.bounds;
        float newX = Random.Range(bounds.min.x, bounds.max.x);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }


    public bool IsInside(Vector3 point)
    {
        return boxCollider != null && boxCollider.bounds.Contains(point);
    }
}
