using System;
using System.Collections;
using System.Collections.Generic;
using AssetUsageDetectorNamespace;
using UnityEngine;
using AYellowpaper.SerializedCollections;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Handle the interactions between the player and interactables
 */// --------------------------------------------------------


/// <summary>
/// Class that manages the interactions between the player and items
/// </summary>
public class InterMan : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    [InspectorReadOnly] [SerializeField] private int currListSize = 0;
    
    // Local Variables
    private SerializedDictionary<Collider, Interactable> cachedScripts = new();
    private List<Interactable> interactablesList = new();
    private Interactable focusedInteractable;
    private bool hasFocusedEventExecuted = false;
    
    void Start()
    {
        // Start Cleaning routine
        StartCoroutine(CleanDictionaryRoutine());
        // Subscribe to events
        InputManager.OnInteract += OnInteractionExecuted;
    }
    
    void Update()
    {
        FindClosestInteraction();
        currListSize = cachedScripts.Count;
    }

    private void OnDestroy()
    {
        // Unsubscribe to events
        InputManager.OnInteract -= OnInteractionExecuted;
    }

    /// <summary>
    /// Function that will be called whenever the player hits the interaction button
    /// </summary>
    private void OnInteractionExecuted()
    {
        focusedInteractable?.OnInteractionExecuted();
    }

    /// <summary>
    /// Function that will find which object is the closest to the player and focus it
    /// </summary>
    private void FindClosestInteraction()
    {
        // Check if empty, if so, return early
        if (interactablesList.IsEmpty())
        {
            if (focusedInteractable)
            {
                focusedInteractable.OnFocusExit();
                focusedInteractable = null;
            }
            hasFocusedEventExecuted = false;
            return;
        }
        
        // Else we do have interactables, find closest
        Vector3 playerPos = PlayerObject.Instance.transform.position;
        Interactable closest = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (var interactable in interactablesList)
        {
            if (interactable == null) continue; // Skip null entries

            float distanceSqr = (interactable.transform.position - playerPos).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closest = interactable;
            }
        }
        
        // Check if the closest is different from the previous
        if (focusedInteractable != null && focusedInteractable != closest)
        {
            // It was different, call OnFocusExit
            focusedInteractable.OnFocusExit();
            hasFocusedEventExecuted = false;
            focusedInteractable = closest;
        }
        
        // Check if it was focused before
        if (!hasFocusedEventExecuted)
        {
            // First time this object has been focused
            focusedInteractable = closest;
            hasFocusedEventExecuted = true;
            focusedInteractable.OnFocusEnter();
        }
        else
        {
            // It was, execute OnFocusStay
            focusedInteractable.OnFocusStay();
        }
        
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check and cache objects that are interactable
        other.TryGetComponent<Interactable>(out Interactable interactable);
        if (interactable)
        {
            // Object was of type interactable, add to dictionary and trigger the 
            cachedScripts.Add(other, interactable);
            interactablesList.Add(interactable);
            interactable.OnInteractionEnter();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Execute the stay function if it is an interactable
        cachedScripts.TryGetValue(other, out Interactable interactable);
        // If valid, execute
        if (interactable) interactable.OnInteractionStay();
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove from cache
        cachedScripts.TryGetValue(other, out Interactable interactable);
        // If valid, remove
        if (interactable)
        {
            interactablesList.Remove(interactable);
            cachedScripts.Remove(other);
            interactable.OnInteractionExit();
        }
    }

    /// <summary>
    /// Routine that will run every 2 second to check the dictionary for nulls and clean them
    /// </summary>
    private IEnumerator CleanDictionaryRoutine()
    {
        List<Collider> toRemove = new List<Collider>();
        WaitForSeconds waitTime = new WaitForSeconds(2f);
        while (true)
        {
            // Wait X seconds
            yield return waitTime;
            
            // Clean dictionary
            toRemove.Clear();

            // Identify null colliders
            foreach (var entry in cachedScripts)
            {
                if (!entry.Key)
                {
                    toRemove.Add(entry.Key);
                }
            }

            // Remove them
            foreach (var collider in toRemove)
            {
                cachedScripts.Remove(collider);
            }
        }
    }
}
