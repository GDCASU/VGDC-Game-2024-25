using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetAttraction : MonoBehaviour
//Summary: Attracts objects that has the tag collectible to collect them (applies to all collectable objects/enemy drops)
//UPDATE: Fixed error where it tries to collect items previously collected
//Made by Eliza Chook
{
    public float attractionSpeed = 1f;  // Speed at which objects are pulled towards the player
     public float attractionDelay = 0.2f; // Delay before starting the attraction
    public float collectionThreshold = 0.4f; // Distance threshold for collecting the object

    private List<Transform> collectiblesInRange = new List<Transform>();

    //Detects if object enters attraction field/capsule collider and adds them to list
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            collectiblesInRange.Add(other.transform);
        }
    }

    //Detects if object leaves attraction field/capsule collider and removes them from list
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            collectiblesInRange.Remove(other.transform);
        }
    }

    private void Update()
    {

        // Loop through collectibles within the attraction range
        for (int i = collectiblesInRange.Count - 1; i >= 0; i--)
        {
            Transform collectible = collectiblesInRange[i];

            // If the collectible is destroyed continue with other objects within loop
            if (collectible == null)
            {
                collectiblesInRange.RemoveAt(i);
                continue;
            }

            // Delay attracting collectable so that it doesn't attract object too fast 
            StartCoroutine(AttractCollectible(collectible));
        }
    }

    // Coroutine to gradually pull the object towards the player
    private IEnumerator AttractCollectible(Transform collectible)
    {
        // Delay the attraction process
        yield return new WaitForSeconds(attractionDelay);

        // Move the collectible towards the player
        while (collectible != null && Vector3.Distance(collectible.position, transform.position) > collectionThreshold)
        {
            collectible.position = Vector3.MoveTowards(collectible.position, transform.position, attractionSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame to continue moving
        }

        // Check if collectible is still valid before collecting
        if (collectible != null)
        {
            CollectItem(collectible);
        }
    }

    // Collect/destroy the item
    private void CollectItem(Transform collectible)
    {
        // Destroy the item and remove it from the list of items in attraction field
        if (collectible != null)
        {
            Destroy(collectible.gameObject);
            collectiblesInRange.Remove(collectible);
        }
    }
}