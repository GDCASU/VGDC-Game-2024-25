using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 *
 * Merging work done by:
 * TJ
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
    [SerializeField] private Material cooldownShaderMat;
    [SerializeField] private MultiAudioEmitter soundEmitter;

    [Header("Ammo Slots")]
    // Neutral element must be first in the list
    [SerializeField] private List<ElementHUDPair> ammoSlots; // All slots, All should be disabled on start

    [Header("Settings")] 
    [SerializeField] private float cooldownDuration;
    [SerializeField] private float maxDisplayScale;
    [SerializeField] private float displayRadius;
    [SerializeField] private float rotationDuration; // In Seconds
    [SerializeField] private float projectileSpawnRadius; // Distance from center of player from which to fire ammo
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local Variables
    private readonly int projectileAudioHash = Animator.StringToHash("Projectile");
    private List<ElementHUDPair> activeAmmoSlots = new List<ElementHUDPair>();
    private ElementHUDPair currentAmmoSlot;
    private Vector3 defaultAmmoPos;
    private Coroutine ammoRotatingRoutine;
    private Coroutine cooldownRoutine;
    private readonly int arc1Property = Shader.PropertyToID("_Arc1");
    
    private void Start()
    {
        // Compute the location of the default position of the element display
        Vector3 direction = ammoDisplayParent.transform.forward * -1;
        defaultAmmoPos = direction * displayRadius;
        
        // Set the arc point 2 of the material shader to 360 as default (clear)
        cooldownShaderMat.SetFloat(arc1Property, 360f);

        // Disable all elements just in case someone forgot to do so in the inspector
        // Also set up references to element storage
        foreach (ElementHUDPair current in ammoSlots)
        {
            current.SetActive(false);
            ElementInvSlot elemSlot = ElementInventoryManager.Instance.GetInvSlotFromElement(current.element);
            current.elementInvSlot = elemSlot;
        }
        
        // Enable elements only if they have ammunition
        // HACK: Neutral element must be first in the inspector
        foreach (ElementHUDPair current in ammoSlots)
        {
            if (current.elementInvSlot.ammoAmount > 0 || current.elementInvSlot.isInfinite)
            {
                // Does have ammo or is infinite, add to active elements
                current.SetActive(true);
                activeAmmoSlots.Add(current);
            }
        }
        // Set the current ammo to neutral and s
        currentAmmoSlot = GetElementHUDPair(Elements.Neutral);
        
        // Arrange the ammo slot spheres
        float angle = 360f / activeAmmoSlots.Count;

        for (int i = 0; i < activeAmmoSlots.Count; i++)
        {
            ElementHUDPair current = activeAmmoSlots[i];
            // Set its position to the default pos
            current.ammoHUDObject.transform.localPosition = defaultAmmoPos;
            // Compute necessary angle shift
            float angleShift = angle * i - current.currentAngle;
            // Rotate around pivot
            current.ammoHUDObject.transform.RotateAround(ammoDisplayParent.transform.position, Vector3.up, -angleShift);
            current.currentAngle = angleShift;
        }
        
        // Scale the ammo spheres depending on ammo quantity
        foreach (ElementHUDPair current in activeAmmoSlots)
        {
            current.ScaleAmmoSpriteSphere(maxDisplayScale);
        }
        
        // DEBUGGING
        if (doDebugLog) PrintActiveSlotsList();
        
        // Subscribe to events
        ElementInventoryManager.AmmoGained += AmmoGainAdjust;
    }

    private void OnDestroy()
    {
        // Unsubscribe to events
        ElementInventoryManager.AmmoGained -= AmmoGainAdjust;
    }

    /// <summary>
    /// Function that makes the HUD switch to next element on player input
    /// </summary>
    public void ChangeElement(bool doGoRight)
    {
        // Don't change if zero or one
        if (activeAmmoSlots.Count <= 1) return;
        
        // Dont perform a change if on animation
        if (ammoRotatingRoutine != null) return;

        // Set the current element depending on direction
        ElementHUDPair previous = currentAmmoSlot;
        if (doDebugLog) Debug.Log("Previous Element = " + previous.element);
        currentAmmoSlot = doGoRight ?  GetNextElementHUDPair() : GetPreviousElementHUDPair();
        if (doDebugLog) Debug.Log("New Current = " + currentAmmoSlot.element);
        
        // If different, shift array so current is at idx 0
        if (previous != currentAmmoSlot)
        {
            if (doGoRight) ShiftLeft(activeAmmoSlots);
            else ShiftRight(activeAmmoSlots);
        }
        
        // Call an update to the HUD slots
        UpdateSlotsPositions(doGoRight);
    }

    /// <summary>
    /// Function to be called anytime the player fires an element
    /// </summary>
    public void FireCurrentElement(Vector3 center, Vector3 direction)
    {
        // Dont fire if currently on animation
        if (ammoRotatingRoutine != null) return;
        
        // Dont fire if currently on cooldown
        if (cooldownRoutine != null) return;
        
        // Spawn a projectile of the current ammo
        Vector3 offsetCenter = center + direction * projectileSpawnRadius;
        GameObject elementProjectile = Instantiate(currentAmmoSlot.elementInvSlot.projectilePrefab, offsetCenter, Quaternion.identity);
        ElementProjectile projectile = elementProjectile.GetComponent<ElementProjectile>();
        projectile.moveDir = direction;
        projectile.ownerTag = this.gameObject.tag;
        
        // Play projectile fire sound
        soundEmitter.PlaySound(projectileAudioHash);
        
        // Trigger Cooldown
        cooldownRoutine = StartCoroutine(CooldownRoutine());
        
        // No need to do anything else if the element is infinite
        if (currentAmmoSlot.elementInvSlot.isInfinite) return;
        
        // Decrease the current element ammo and adjust sphere
        currentAmmoSlot.elementInvSlot.ammoAmount--;
        currentAmmoSlot.ScaleAmmoSpriteSphere(maxDisplayScale);
        
        // Check if we ran out
        if (currentAmmoSlot.elementInvSlot.ammoAmount <= 0)
        {
            // We did, disable it and remove it from the active slots
            currentAmmoSlot.SetActive(false);
            ElementHUDPair toRemove = currentAmmoSlot;
            currentAmmoSlot = GetNextElementHUDPair();
            activeAmmoSlots.Remove(toRemove);
            // Adjust element display to account for the recently removed element
            AdjustSlots();
        }
    }
    
    /// <summary>
    /// Routine that handles the cooldown of element firing
    /// </summary>
    private IEnumerator CooldownRoutine()
    {
        float elapsedTime = 0f;
        float angle = 0f;
        
        // Set the arc value to 0, then increase to 360 as cooldown progresses
        cooldownShaderMat.SetFloat(arc1Property, 0f);
        
        while (elapsedTime < cooldownDuration)
        {
            angle = (elapsedTime / cooldownDuration) * 360f;
            cooldownShaderMat.SetFloat(arc1Property, angle);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // Finished
        cooldownShaderMat.SetFloat(arc1Property, 360f);
        cooldownRoutine = null;
    }

    /// <summary>
    /// Function called to adjust display when an element gained ammo, to adjust display and add element to whell
    /// if necessary
    /// </summary>
    private void AmmoGainAdjust(Elements targetSlot)
    {
        ElementHUDPair targetHUD = GetElementHUDPair(targetSlot);
        // Check if we need to re-enable the element in case it had zero ammo before
        if (!targetHUD.IsActive())
        {
            // We need to re-enable, add it to active ammo and adjust
            targetHUD.SetActive(true);
            activeAmmoSlots.Add(targetHUD);
            AdjustSlots();
        }
        // Scale Ammo Sphere
        targetHUD.ScaleAmmoSpriteSphere(maxDisplayScale);
    }

    
    /// <summary>
    /// Function that updates the position of the Ammo HUD Spheres
    /// </summary>
    private void UpdateSlotsPositions(bool doGoRight)
    {
        // No ammo
        if (activeAmmoSlots.Count <= 0)
        {
            // FIXME: Should Hide UI?
            return;
        }
        
        // Only 1
        if (activeAmmoSlots.Count == 1) return;
        
        // Else, Start animation coroutine
        ammoRotatingRoutine = StartCoroutine(RotateSlotsViaInput(doGoRight));
    }

    /// <summary>
    /// Function that handles the fixing of the Ammo Display in case some element was fully spent
    /// </summary>
    private void AdjustSlots()
    {
        // HACK IAN: Maybe one day do this animated like the switching, its just annoying math I ain't doing rn
        
        // Arrange the ammo slot spheres
        float angle = 360f / activeAmmoSlots.Count;

        for (int i = 0; i < activeAmmoSlots.Count; i++)
        {
            ElementHUDPair current = activeAmmoSlots[i];
            // Zero the current angle
            current.currentAngle = 0;
            // Set its position to the default pos
            current.ammoHUDObject.transform.localPosition = defaultAmmoPos;
            // Compute necessary angle shift
            float angleShift = angle * i - current.currentAngle;
            // Rotate around pivot
            current.ammoHUDObject.transform.RotateAround(ammoDisplayParent.transform.position, Vector3.up, -angleShift);
            current.currentAngle = angleShift;
        }
    }

    /// <summary>
    /// Coroutine that handles the rotation of the Ammo HUD when changing elements, doesnt handle elements spent
    /// </summary>
    private IEnumerator RotateSlotsViaInput(bool doGoRight)
    {
        // Compute angle
        int directionSign = (doGoRight) ? 1 : -1;
        float angle = (360f / activeAmmoSlots.Count) * directionSign;
        
        // Make a list for all the Display Coroutines to check their animation status
        int currID = 1;
        List<int> coroutineIDs = new List<int>();
        
        // Start Coroutine per sphere to fix their pos
        // This section assumes the element array has been updated/shifted prior to this
        // coroutine being called
        for (int i = 0; i < activeAmmoSlots.Count; i++)
        {
            ElementHUDPair current = activeAmmoSlots[i];
            // Store an ID representing the animation coroutine
            coroutineIDs.Add(currID);
            // Start coroutine
            StartCoroutine(SingularSlotRotation(coroutineIDs, current, angle, currID));
            // Increment ID for next
            currID++;
        }
        
        // Wait for all routines to finish
        while (coroutineIDs.Count > 0) yield return null;
        
        // DEBUGGING
        if (doDebugLog) PrintActiveSlotsList();
        
        // Finished rotating, null the coroutine field
        ammoRotatingRoutine = null;
    }

    /// <summary>
    /// Coroutine to hanlde the rotation of a specific slot
    /// </summary>
    private IEnumerator SingularSlotRotation(List<int> coroutineIDList, ElementHUDPair targetDisplay, float rotationAmount, int assignedID)
    {
        float elapsedTime = 0;
        float targetAngle = rotationAmount + targetDisplay.currentAngle;
        
        // Start rotation to target pos
        while (elapsedTime < rotationDuration)
        {
            elapsedTime += Time.deltaTime;
            // Perform rotation
            float degreesToRotate = rotationAmount * Time.deltaTime / rotationDuration;
            targetDisplay.ammoHUDObject.transform.RotateAround(ammoDisplayParent.transform.position, Vector3.up, degreesToRotate);
            targetDisplay.currentAngle += degreesToRotate;
            // Wait a frame
            yield return null;
        }
        
        // Finished, adjust final angle
        float fixAmount = targetAngle - targetDisplay.currentAngle;
        targetDisplay.ammoHUDObject.transform.RotateAround(ammoDisplayParent.transform.position, Vector3.up, fixAmount);
        // Fix current angle in case its over 360 or negative
        targetDisplay.currentAngle = Mathf.Repeat(targetAngle, 360f);
        
        // On finish, remove the routine from the status list to mark it as done on the main routine
        coroutineIDList.Remove(assignedID);
    }

    /// <summary>
    /// Helper function to print to console the current active ammo list
    /// </summary>
    private void PrintActiveSlotsList()
    {
        string msg = "Active ammo list:\n";
        for (int i = 0; i < activeAmmoSlots.Count; i++)
        {
            msg += "[" + i + "] " +  activeAmmoSlots[i].ammoHUDObject.name + "\n";
        }
        Debug.Log(msg);
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
        if (ammoSlotsCount == 1) return currentAmmoSlot;
        
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
    /// Helper function to shift the elements of the list left by one
    /// </summary>
    private void ShiftLeft<T>(List<T> list)
    {
        // Don't do anything if null or length less than 2
        if (list == null || list.Count <= 1) return;

        // Save the first element
        T firstElement = list[0];

        // Shift all elements to the left
        list.RemoveAt(0); // Remove the first element
        list.Add(firstElement); // Add the first element to the end
    }

    /// <summary>
    /// Helper function to shift the elements of the list right by one
    /// </summary>
    private void ShiftRight<T>(List<T> list)
    {
        // Don't do anything if null or length less than 2
        if (list == null || list.Count <= 1) return;

        // Save the last element
        T lastElement = list[list.Count - 1];

        // Shift all elements to the right
        list.RemoveAt(list.Count - 1); // Remove the last element
        list.Insert(0, lastElement); // Insert the last element at the beginning
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
        // Store the current angle difference from the central slot to avoid excessive calculation of angles
        [InspectorReadOnly] public float currentAngle = 0;
        
        /// <summary>
        /// Helper function to enable/disable the HUD GameObject
        /// </summary>
        public void SetActive(bool active)
        {
            ammoHUDObject.SetActive(active);
        }

        /// <summary>
        /// Get the current active state
        /// </summary>
        public bool IsActive()
        {
            return ammoHUDObject.activeSelf;;
        }

        /// <summary>
        /// Helper Function to scale the UI Sphere of the element using max ammo and current ammo
        /// </summary>
        public void ScaleAmmoSpriteSphere(float maxDisplayScale)
        {
            // Local variable defined for readability
            Transform spriteRendererTransform = ammoSpriteRenderer.transform;
            
            // Dont need to scale infinite ammo, set it to max
            if (elementInvSlot.isInfinite)
            {
                spriteRendererTransform.localScale = new Vector3(maxDisplayScale, maxDisplayScale, maxDisplayScale);
                return;
            }
            
            // Block a division by zero or less
            if (elementInvSlot.ammoMaxAmount <= 0)
            {
                string msg = "ERROR! DIVISION BY NON NATURAL NUMBER TRIGGERED! Needs to be 1 or higher\n";
                msg += "Offending Element = " + element + "\n";
                msg += "Max Amount Read = " + elementInvSlot.ammoMaxAmount;
                Debug.LogError(msg);
                return;
            }

            // Otherwise compute scale
            float scale = ((float)elementInvSlot.ammoAmount / (float)elementInvSlot.ammoMaxAmount) * maxDisplayScale;
            
            // Set the transform
            spriteRendererTransform.localScale = new Vector3(scale, scale, scale);
        }
    }
}






















