using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

/* -----------------------------------------------------------
 * Author: TJ (Yousuf)
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose: The Inventory Systen to store items
 * 
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>
public class InventorySystem : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    // Create a Dictionary to store ItemData with InventoryItem(s)
    // Create a List to store the InventoryItem(s) called inventory

    private void Awake()
    {
        // Set the List here
        // Set the Dictionary here
    }

    public void Add(// ItemData)
    {
        // If item already exists in Dictionary, add to stack
        if(//Dictionary.TryGetValue(ItemData, out InventoryItem value)
        {
            // Add to stack
        }
        // Create new InventoryItem instance if item doesn't exist
        else
        {
            // InventoryItem name = new InventoryItem(ItemData)
            // Add item to inventory
            // Add Item with ItemData to dictionary
        }
    }

    public void Remove(// ItemData)
    {
        // If item already exists in Dictionary, remove from stack
        if (//Dictionary.TryGetValue(ItemData, out InventoryItem value)
        {
            // Remove from stack
            
            //If stack == 0, remove item instance
            if(// Item stack == 0)
            {
                // Remove value from inventory
                // Remove Item with ItemData from dictionary
            }
        }
    }
}
