using System.Collections;
using UnityEngine;

public class TaxiRandomBump : MonoBehaviour
{
    [Header("Bump Settings")]
    public float bumpAmount = 0.1f;        // How far each bump moves
    public float bumpSpeed = 0.05f;        // How fast the bump happens
    public float delayBetweenBumps = 0.5f; // Max delay between bumps

    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.localPosition;
        StartCoroutine(BumpLoop());
    }

    IEnumerator BumpLoop()
    {
        while (true)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-bumpAmount, bumpAmount),
                Random.Range(-bumpAmount, bumpAmount),
                0f // Add Z if needed
            );

            Vector3 targetPosition = originalPosition + randomOffset;

            // Move to target
            float elapsed = 0f;
            while (elapsed < bumpSpeed)
            {
                transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, elapsed / bumpSpeed);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = targetPosition;

            // Return to original
            elapsed = 0f;
            while (elapsed < bumpSpeed)
            {
                transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, elapsed / bumpSpeed);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.localPosition = originalPosition;

            // Wait a random time before the next bump
            float delay = Random.Range(0f, delayBetweenBumps);
            yield return new WaitForSeconds(delay);
        }
    }
}
