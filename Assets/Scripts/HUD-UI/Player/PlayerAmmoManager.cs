using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using DG.Tweening;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Handle the element display above the player, done dynamically
 */// --------------------------------------------------------


/// <summary>
/// Manager of the Ammo Display above the player
/// </summary>
public class PlayerAmmoManager : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private GameObject ammoDisplayParent;
    
    [Header("Ammo Slots")]
    [SerializeField] private List<ElementHUDPair> ammoSlots; // All slots, All should be disabled on start

    [Header("Settings")] 
    [SerializeField] private float ammoDisplayRadius;
    [SerializeField] private float rotationDuration; // In Seconds
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local Variables
    private List<ElementHUDPair> activeAmmoSlots; // Treated like a queue
    private ElementHUDPair currentAmmoSlot;
    
    // State Bools
    private Coroutine ammoRotatingRoutine;

    private void Start()
    {
        // Disable all elements just in case someone forgot to do so in the inspector
        // Also set up references to element storage
        foreach (ElementHUDPair current in ammoSlots)
        {
            current.SetActive(false);
            ElementInvSlot elemSlot = ElementInventoryManager.Instance.GetInvSlotFromElement(current.element);
            current.elementInvSlot = elemSlot;
        }
        
        // Enable elements only if they have ammunition
        foreach (ElementHUDPair current in ammoSlots)
        {
            if (current.elementInvSlot.ammoAmount > 0 || current.elementInvSlot.isInfinite)
            {
                // Does have ammo or is infinite, add to active elements
                current.SetActive(true);
                activeAmmoSlots.Add(current);
            }
        }
        // Set the current ammo to neutral
        currentAmmoSlot = GetElementHUDPair(Elements.Neutral);
        
        // Arrange the ammo slot spheres
        UpdateSlotsPositions();
    }

    /// <summary>
    /// Function that makes the HUD switch to next element on player input
    /// </summary>
    public void ChangeElement(bool doGoRight)
    {
        // Dont change if zero or one
        if (activeAmmoSlots.Count <= 1) return;
        
        // FIXME: Block during switch animation?

        // Set the current element depending on direction
        currentAmmoSlot = doGoRight ? GetNextElementHUDPair() : GetPreviousElementHUDPair();
        
        // Call an update to the HUD slots
        UpdateSlotsPositions();
    }

    /// <summary>
    /// Function to be called anytime the player fires an element
    /// </summary>
    public void ElementConsumed()
    {
        
    }

    
    /// <summary>
    /// Function that updates the position of the Ammo HUD Spheres
    /// </summary>
    private void UpdateSlotsPositions()
    {
        // No ammo
        if (activeAmmoSlots.Count <= 0)
        {
            // FIXME: Should Hide UI?
            return;
        }
        
        // Only 1
        if (activeAmmoSlots.Count == 1) return;
        
        // Disable any slots that ran out of ammo if any
        for (int i = activeAmmoSlots.Count - 1; i >= 0; i--)
        {
            ElementHUDPair current = activeAmmoSlots[i];
            if (current.elementInvSlot.ammoAmount <= 0 && !current.elementInvSlot.isInfinite)
            {
                // Ran out of ammo and is not infinite
                current.SetActive(false);
                activeAmmoSlots.Remove(current);
            }
        }

        // Start coroutine
        ammoRotatingRoutine = StartCoroutine(RotateSlots());
    }

    /// <summary>
    /// Coroutine that handles the rotation of the Ammo HUD
    /// </summary>
    private IEnumerator RotateSlots()
    {
        float angle = 360f / activeAmmoSlots.Count;
        
        // Make a list for all the Display Coroutines to check their status
        List<Coroutine> coroutines = new List<Coroutine>();
        
        // Start Coroutines
        Coroutine currentRoutine;
        currentRoutine = StartCoroutine(SingularSlotRotation(coroutines, currentAmmoSlot, 0f, ref currentRoutine));
        
        // Wait for all routines to finish
        while (coroutines.Count > 0) yield return null;
        
        
        
        
        // Start rotating towards target pos
        float elapsedTime = 0;
        
        /*
        while (elapsedTime < rotationDuration)
        {
            // Calculate Step
            elapsedTime += Time.deltaTime;
            // Rotate
            foreach (ElementHUDPair current in activeAmmoSlots)
            {
                current.ammoHUDObject.transform.RotateAround(ammoDisplayParent.transform.position, Vector3.up, angle);
            }
            // Wait a frame
            yield return null;
        }
        */
        // Finished rotating, null the coroutine field
        ammoRotatingRoutine = null;
    }

    /// <summary>
    /// Coroutine to hanlde the rotation of a specific slot
    /// </summary>
    /// <returns></returns>
    private IEnumerator SingularSlotRotation(List<Coroutine> statusList, ElementHUDPair targetDisplay, float targetAngle, ref Coroutine thisRoutine)
    {
        // TODO: FINISH THIS CODE
        
        // On finish, remove the routine from the status list to mark it as done on the main routine
        statusList.Remove(thisRoutine);
    }
    
    /// <summary>
    /// Function to call when ammo has been spent and spheres need resizing
    /// </summary>
    private void UpdateSlotsScales()
    {
        foreach (ElementHUDPair current in activeAmmoSlots)
        {
            current.ScaleAmmoSpriteSphere();
        }
    }

    /// <summary>
    /// Helper function to retrieve a element hud pair based on element
    /// </summary>
    private ElementHUDPair GetElementHUDPair(Elements targetElement)
    {
        foreach (ElementHUDPair pair in ammoSlots)
        {
            // Check for match
            if (pair.element == targetElement) return pair;
        }
        // Did not find it
        Debug.LogError("ERROR! ELEMENT HUD PAIR NOT FOUND! MUST BE ADDED IN THE INSPECTOR");
        return null;
    }

    /// <summary>
    /// Function to get the next active slot on the HUD
    /// </summary>
    private ElementHUDPair GetNextElementHUDPair()
    {
        int ammoSlotsCount = activeAmmoSlots.Count;
        
        // If there's only one element or less, return the current element
        if (ammoSlotsCount <= 1) return currentAmmoSlot;
        
        // Get indexes
        int nextIdx = activeAmmoSlots.IndexOf(currentAmmoSlot) + 1;
        int lastIdx = ammoSlotsCount - 1;
        
        // Check for array out of bounds
        if (nextIdx > lastIdx)
        {
            // Overflowed, next is the first element
            return activeAmmoSlots[0];
        }
        // Else, return next
        return activeAmmoSlots[nextIdx];
    }
    
    /// <summary>
    /// Function to get the previous active slot on the HUD
    /// </summary>
    private ElementHUDPair GetPreviousElementHUDPair()
    {
        int ammoSlotsCount = activeAmmoSlots.Count;

        // If there's only one element or less, return the current element
        if (ammoSlotsCount <= 1) return currentAmmoSlot;

        // Get indexes
        int prevIdx = activeAmmoSlots.IndexOf(currentAmmoSlot) - 1;

        // Check for array out of bounds
        if (prevIdx < 0)
        {
            // Underflowed, previous is the last element
            return activeAmmoSlots[ammoSlotsCount - 1];
        }
        // Else, return previous
        return activeAmmoSlots[prevIdx];
    }
    
    /// <summary>
    /// Helper class to match the hud slots to their elements on the inspector
    /// </summary>
    [Serializable]
    private class ElementHUDPair
    {
        public GameObject ammoHUDObject;
        public SpriteRenderer ammoSpriteRenderer;
        public Elements element;
        
        // Store a reference to the Inventory Slot
        [HideInInspector] public ElementInvSlot elementInvSlot;
        
        /// <summary>
        /// Helper function to enable/disable the HUD GameObject
        /// </summary>
        public void SetActive(bool active)
        {
            ammoHUDObject.SetActive(active);
        }

        /// <summary>
        /// Helper Function to scale the UI Sphere of the element using max ammo and current ammo
        /// </summary>
        public void ScaleAmmoSpriteSphere()
        {
            // Block a division by zero or less
            if (elementInvSlot.ammoMaxAmount <= 0)
            {
                string msg = "ERROR! DIVISION BY NON NATURAL NUMBER TRIGGERED! Needs to be 1 or higher\n";
                msg += "Offending Element = " + element + "\n";
                msg += "Max Amount Read = " + elementInvSlot.ammoMaxAmount;
                Debug.LogError(msg);
                return;
            }
            
            // Local variable defined for readability
            Transform spriteRendererTransform = ammoSpriteRenderer.transform;
            
            // Compute scale
            float scale = (float)elementInvSlot.ammoMaxAmount / (float)elementInvSlot.ammoMaxAmount;
            
            // Set the transform
            spriteRendererTransform.localScale = new Vector3(scale, scale, scale);
        }
    }
}






















