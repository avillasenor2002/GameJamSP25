using System.Collections;
using UnityEngine;

public class TempoBarShake : MonoBehaviour
{
    private Vector3 originalPosition;

    [SerializeField] private float shakeIntensity = 0.5f;
    [SerializeField] private float shakeDuration = 0.5f;
    void Start()
    {
        originalPosition = transform.localPosition;    
    }

    public void ShakeCamera()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-shakeIntensity, shakeIntensity);
            float y = Random.Range(-shakeIntensity, shakeIntensity);

            // Apply shake to the camera position
            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset camera position after shaking
        transform.localPosition = originalPosition;
    }
}
