using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amalgam : MonoBehaviour
{
    [Header("References")]
    public AIDestinationSetter destinationSetter;

    [Header("Leap Settings")]
    public LayerMask ignoreLayer;
    public float leapBackMaxDistance = 3.0f;
    public float leapSpeed = 5.0f; // distance per second
    public float idleTime = 0.5f;

    private PlayerController player;
    private Coroutine leapBackCoro = null;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    private void Start()
    {
        destinationSetter.target = player.transform;
    }

    private IEnumerator LeapBackCoroutine(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;
        float distance = Vector3.Distance(startPosition, targetPosition);
        float duration = distance / leapSpeed;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.position = targetPosition;

        yield return new WaitForSeconds(idleTime);

        leapBackCoro = null;
        destinationSetter.target = player.transform;
    }

    public void OnPlayerHit()
    {
        destinationSetter.target = null;

        if (leapBackCoro != null)
            StopCoroutine(leapBackCoro);

        Vector3 direction = transform.position - player.transform.position;
        direction.Normalize();
        direction.y = 0;

        Vector3 target = transform.position + direction * leapBackMaxDistance;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, leapBackMaxDistance, ~ignoreLayer))
            target = hit.point;

        if (leapBackCoro != null)
            StopCoroutine(leapBackCoro);

        leapBackCoro = StartCoroutine(LeapBackCoroutine(target));   
    }
}
