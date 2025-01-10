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
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

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
    
    void Start()
    {
        // Set the current element to neutral
        
    }

    private void OnDestroy()
    {
        // null Singleton field
        Instance = null;
    }

    public ElementInvSlot GetInvSlotFromElement(Elements targetElement)
    {
        switch (targetElement)
        {
            case Elements.Neutral:
                return neutralElement;
                break;
            case Elements.Fire:
                return fireElement;
                break;
            case Elements.Fungal:
                return fungalElement;
                break;
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
