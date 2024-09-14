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

    // Create a private Dictionary to store ItemData with InventoryItem(s)
    private Dictionary<ItemData_Placeholder, InventoryItem> itemDictionary;
    public List<InventoryItem> inventory { get; private set; }

    private void Awake()
    {
        inventory = new List<InventoryItem> ();
        // Set the Dictionary here
        itemDictionary = new Dictionary<ItemData_Placeholder, InventoryItem> ();
    }

    public void Add(ItemData_Placeholder itemData) // argument: ItemData
    {
        // If item already exists in Dictionary, add to stack
        if(itemDictionary.TryGetValue(itemData, out InventoryItem value)) //Dictionary.TryGetValue(ItemData, out InventoryItem value
        {
            // Add to stack
            value.AddToStack();
        }
        // Create new InventoryItem instance if item doesn't exist
        else
        {
            // InventoryItem name = new InventoryItem(ItemData)
            InventoryItem newItem = new InventoryItem(itemData);
            // Add item to inventory
            inventory.Add(newItem);
            // Add Item with ItemData to dictionary
            itemDictionary.Add(itemData, newItem);
        }
    }

    public void Remove(ItemData_Placeholder itemData) // argument: ItemData
    {
        // If item already exists in Dictionary, remove from stack
        if (itemDictionary.TryGetValue(itemData, out InventoryItem value)) //Dictionary.TryGetValue(ItemData, out InventoryItem value
        {
            // Remove from stack
            value.RemoveFromStack();
            
            //If stack == 0, remove item instance
            if(value.stackSize == 0) // value stack == 0
            {
                // Remove value from inventory
                inventory.Remove(value);
                // Remove Item with ItemData from dictionary
                itemDictionary.Remove(itemData);
            }
        }
    }
}
