using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquishVisualEffect : MonoBehaviour
{
    public float squishSpeed = 2f;         // How fast it squishes
    public float squishAmount = 0.1f;      // How much it squishes
    private Vector3 originalScale;

    private static float globalSquishTimer = 0f;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        SyncGlobalSquishTime();

        float scaleY = 1f + Mathf.Sin(globalSquishTimer * squishSpeed) * squishAmount;
        float scaleX = 1f - (scaleY - 1f); // Inverse to maintain volume feel
        transform.localScale = new Vector3(originalScale.x * scaleX, originalScale.y * scaleY, originalScale.z);
    }

    void SyncGlobalSquishTime()
    {
        // All squish effects will reference this shared timer
        globalSquishTimer += Time.deltaTime;

        // Optional reset after 1000 seconds to prevent overflow
        if (globalSquishTimer > 1000f)
        {
            globalSquishTimer = 0f;
        }
    }
}
