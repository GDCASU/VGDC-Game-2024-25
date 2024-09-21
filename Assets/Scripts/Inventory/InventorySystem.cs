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
/// InventorySystem has been tested and turned out to be fully functional with expected results (9/14 2:20 PM)
/// </summary>
public class InventorySystem : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    // Create a private Dictionary to store ItemData with InventoryItem(s)
    private Dictionary<ItemData_Placeholder, InventorySlot> itemDictionary;
    public List<InventorySlot> inventory { get; private set; }

    public InventorySystem inventorySystem;

    private void Awake()
    {
        inventory = new List<InventorySlot> ();
        // Set the Dictionary here
        itemDictionary = new Dictionary<ItemData_Placeholder, InventorySlot> ();

        // Set the Singleton
        inventorySystem = this;
    }

    public void Add(ItemData_Placeholder itemData) // argument: ItemData
    {
        // If item already exists in Dictionary, add to stack
        if(itemDictionary.TryGetValue(itemData, out InventorySlot value)) //Dictionary.TryGetValue(ItemData, out InventorySlot value
        {
            // Add to stack
            value.AddToStack();
            Debug.Log("item added = " + itemData.displayName);
            Debug.Log("Stack = " + value.stackSize);
        }
        // Create new InventoryItem instance if item doesn't exist
        else
        {
            // InventoryItem name = new InventorySlot(ItemData)
            InventorySlot newItem = new InventorySlot(itemData);
            // Add item to inventory
            inventory.Add(newItem);
            // Add Item with ItemData to dictionary
            itemDictionary.Add(itemData, newItem);
            Debug.Log("New object detected = " + itemData.displayName);
        }
    }

    public void Remove(ItemData_Placeholder itemData) // argument: ItemData
    {
        // If item already exists in Dictionary, remove from stack
        if (itemDictionary.TryGetValue(itemData, out InventorySlot value)) //Dictionary.TryGetValue(ItemData, out InventorySlot value
        {
            // Remove from stack
            value.RemoveFromStack();
            Debug.Log("item removed = " + itemData.displayName);
            Debug.Log("Stack = " + value.stackSize);

            //If stack == 0, remove item instance
            if (value.stackSize == 0) // value stack == 0
            {
                // Remove value from inventory
                inventory.Remove(value);
                // Remove Item with ItemData from dictionary
                itemDictionary.Remove(itemData);
            }
        }
    }

    public void Add(ItemData_Placeholder itemData, int amount) // argument: ItemData
    {
        // If item already exists in Dictionary, add to stack
        if (itemDictionary.TryGetValue(itemData, out InventorySlot value)) //Dictionary.TryGetValue(ItemData, out InventorySlot value
        {
            // Add to stack
            value.AddToStack(amount);
            Debug.Log("item added = " + itemData.displayName);
            Debug.Log("Stack = " + value.stackSize);
        }
        // Create new InventoryItem instance if item doesn't exist
        else
        {
            // InventoryItem name = new InventorySlot(ItemData)
            InventorySlot newItem = new InventorySlot(itemData);
            // Add item to inventory
            inventory.Add(newItem);
            // Add Item with ItemData to dictionary
            itemDictionary.Add(itemData, newItem);
            Debug.Log("New object detected = " + itemData.displayName);
        }
    }

    public void Remove(ItemData_Placeholder itemData, int amount) // argument: ItemData
    {
        // If item already exists in Dictionary, remove from stack
        if (itemDictionary.TryGetValue(itemData, out InventorySlot value)) //Dictionary.TryGetValue(ItemData, out InventorySlot value
        {
            // Remove from stack
            value.RemoveFromStack(amount);
            Debug.Log("item removed = " + itemData.displayName);
            Debug.Log("Stack = " + value.stackSize);

            //If stack == 0, remove item instance
            if (value.stackSize == 0) // value stack == 0
            {
                // Remove value from inventory
                inventory.Remove(value);
                // Remove Item with ItemData from dictionary
                itemDictionary.Remove(itemData);
            }
        }
    }

    // InventorySlot getter
    public InventorySlot getSlot(ItemData_Placeholder itemData)
    {
        if (itemDictionary.TryGetValue(itemData, out InventorySlot value))
            return value;
        else 
            return null;
    }
}
