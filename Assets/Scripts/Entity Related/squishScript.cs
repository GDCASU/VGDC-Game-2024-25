using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * Written by Matthew Glos
 * 
 * squishes the cube
 */
public class squishScript : MonoBehaviour
{
    public Vector3 squishScale = new Vector3(1.5f, 0.5f, 1.5f); // Squished size
    public float squishDuration = 0.2f; // Duration of the squish
    public float returnDuration = 0.2f; // Duration to return to normal

    private Vector3 originalScale;
    private bool isSquishing = false;

    public void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnTriggerEnter(UnityEngine.Collider other)
    {
        Squish();   
    }
    public void Squish()
    {
        if (!isSquishing)
        {
            StartCoroutine(SquishRoutine());
        }
    }

    private IEnumerator SquishRoutine()
    {
        isSquishing = true;

        // Squish
        yield return ExponentialScaleOverTime(transform, squishScale, squishDuration);

        // Return to normal
        yield return ExponentialScaleOverTime(transform, originalScale, returnDuration);

        isSquishing = false;
    }

    private IEnumerator ExponentialScaleOverTime(Transform target, Vector3 targetScale, float duration)
    {
        float time = 0;
        Vector3 startScale = target.localScale;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = 1f - Mathf.Pow(2f, -10f * (time / duration)); // Exponential ease-out
            target.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        target.localScale = targetScale;
    }

}
