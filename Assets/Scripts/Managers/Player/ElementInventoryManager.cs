using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Serialization;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Handle the player's Element Inventory
 */// --------------------------------------------------------


/// <summary>
/// Singleton Manager to handle the players Element inventory
/// </summary>
public class ElementInventoryManager : MonoBehaviour
{
    // Singleton
    public static ElementInventoryManager Instance { get; private set; }
    
    
    [Header("Elements Storage")]
    [SerializeField] private ElementInvSlot neutralElement;
    [SerializeField] private ElementInvSlot fireElement;
    [SerializeField] private ElementInvSlot fungalElement;
    [SerializeField] private ElementInvSlot sparkElement;
    [SerializeField] private ElementInvSlot waterElement;
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Event to raise whenever the player gains element ammo
    public static event System.Action<Elements> AmmoGained;

    private void Awake()
    {
        // Set the Singleton
        if (Instance != null && Instance != this)
        {
            // Already set, destroy this object
            Destroy(gameObject);
            return;
        }
        // Not set yet
        Instance = this;
    }

    private void OnDestroy()
    {
        // null Singleton field
        Instance = null;
    }

    /// <summary>
    /// Function to add a charge to an element on the inventory
    /// </summary>
    /// <param name="targetElement"></param>
    public void AddAmmoToElement(Elements targetElement, int amount = 1)
    {
        ElementInvSlot slot = GetInvSlotFromElement(targetElement);
        // Dont do anything if null
        if (slot == null) return;
        // Check if full
        if (slot.ammoAmount >= slot.ammoMaxAmount) return;
        // Else, Add charge and raise event
        slot.ammoAmount += amount;
        // Check for overflow and underflow (negative check is to handle the max ammo cheat)
        if (slot.ammoAmount > slot.ammoMaxAmount || slot.ammoAmount < -100) slot.ammoAmount = slot.ammoMaxAmount;
        // Raise Event
        AmmoGained?.Invoke(targetElement);
    }

    /// <summary>
    /// Helper function. Gets the element inventory slot from this object via a specified enumerator
    /// </summary>
    public ElementInvSlot GetInvSlotFromElement(Elements targetElement)
    {
        switch (targetElement)
        {
            case Elements.Neutral:
                return neutralElement;
            case Elements.Fire:
                return fireElement;
            case Elements.Fungal:
                return fungalElement;
            case Elements.Sparks:
                return sparkElement;
            case Elements.Water:
                return waterElement;
        }
        // Did not find element in inventory
        Debug.LogError("ERROR! DID NOT FIND TARGET ELEMENT CASE ON SWITCH!");
        return null;
    }

}

// Helper Class to handle element storage
[Serializable]
public class ElementInvSlot
{
    public Elements element;
    public GameObject projectilePrefab;
    public bool isInfinite;
    public int ammoAmount;
    public int ammoMaxAmount;
}
