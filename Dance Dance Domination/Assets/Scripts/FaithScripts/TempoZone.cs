using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempoZone : MonoBehaviour
{
    public GameObject gray;
    private BoxCollider2D boxCollider;
    private Bounds bounds;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();   
    }

    private void Start()
    {
        BoxCollider2D grayCollider = gray.GetComponent<BoxCollider2D>();
        bounds = grayCollider.bounds;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        float boxCenter = sr.bounds.extents.x;
        float newX = Random.Range(bounds.min.x+boxCenter, bounds.max.x-boxCenter);
        float fixedY = bounds.center.y;

        transform.position = new Vector3(newX, fixedY, transform.position.z);
    }


    public bool IsInside(Vector3 point)
    {
        return boxCollider != null && boxCollider.bounds.Contains(point);
    }
}
