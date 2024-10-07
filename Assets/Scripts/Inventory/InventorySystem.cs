using FMOD;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.SceneManagement;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.UI;

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
/// The class that manages the whole inventory system
/// </summary>
public class InventorySystem : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    // Create a private Dictionary to store ItemData with InventoryItem(s)
    private Dictionary<string, InventorySlot> itemDictionary;
    public List<InventorySlot> inventory { get; set; }

    public static InventorySystem Instance { get; private set; }

    // Access the UI for slots
    [SerializeField] private GameObject slotsHolder;
    private GameObject[] slots;

    private void Awake()
    {
        UnityEngine.Debug.Log("Inventory Awake");
        inventory = new List<InventorySlot> ();
        // Set the Dictionary here
        itemDictionary = new Dictionary<string, InventorySlot> ();

        // Set the Singleton
        if(Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else { Instance = this; }

        // Sets UI slots
        slots = new GameObject[slotsHolder.transform.childCount];
        for (int i = 0; i < slotsHolder.transform.childCount; i++)
        {
            slots[i] = slotsHolder.transform.GetChild(i).gameObject;
        }
    }

    /// <summary>
    /// Adds an item to the inventory
    /// </summary>
    /// <param name="itemData"></param>
    public void Add(ItemData itemData) // argument: ItemData
    {
        // If item already exists in Dictionary, add to stack
        if (itemDictionary.TryGetValue(itemData.id, out InventorySlot value)) //Dictionary.TryGetValue(ItemData, out InventorySlot value
        {
            int amount = itemData.value;
            // Add to stack
            value.AddToStack(amount);
            UnityEngine.Debug.Log("item added = " + itemData.displayName);
            UnityEngine.Debug.Log("Stack = " + value.stackSize);
        }
        // Create new InventoryItem instance if item doesn't exist
        else
        {
            for (int i = 0; i < slots.Length; i++)
            {
                InventorySlot component = slots[i].GetComponent<InventorySlot>();
                UnityEngine.Debug.Log(component.data);
                if (component.data == null)
                {
                    component.SetInventorySlot(itemData);
                    // Add item to inventory
                    inventory.Add(component);
                    // Add Item with ItemData to dictionary
                    itemDictionary.Add(itemData.id, component);
                    UnityEngine.Debug.Log("New object detected = " + itemData.displayName);
                    UnityEngine.Debug.Log("Stack = " + component.stackSize);
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Removes an item from the inventory
    /// </summary>
    /// <param name="itemData"></param>
    public void Remove(ItemData itemData) // argument: ItemData
    {
        // If item already exists in Dictionary, remove from stack
        if (itemDictionary.TryGetValue(itemData.id, out InventorySlot value)) //Dictionary.TryGetValue(ItemData, out InventorySlot value
        {
            int amount = itemData.value;
            // Remove from stack
            value.RemoveFromStack(amount);
            UnityEngine.Debug.Log("item removed = " + itemData.displayName);
            UnityEngine.Debug.Log("Stack = " + value.stackSize);

            //If stack == 0, remove item instance
            if (value.stackSize <= 0) // value stack == 0
            {
                // Remove value from inventory
                inventory.Remove(value);
                // Remove Item with ItemData from dictionary
                itemDictionary.Remove(itemData.id);
                value.ResetSlot();
            }
        }
    }

    /// <summary>
    /// Gets a the first inventory slot containing ItemData
    /// </summary>
    /// <param name="itemData"></param>
    /// <returns></returns>
    public InventorySlot getSlot(ItemData itemData)
    {
        if (itemDictionary.TryGetValue(itemData.id, out InventorySlot value))
            return value;
        else 
            return null;
    }

    /// <summary>
    /// Can be used if there are any errors; Removes all items from the Inventory
    /// </summary>
    public void ResetInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i].GetComponent<InventorySlot>();
            try
            {
                inventory.Remove(slot);
                itemDictionary.Remove(slot.data.id);
                slot.ResetSlot();
            }
            catch { }
        }
    }

    public void UpdateInventory()
    {
        Dictionary<string, InventorySlot> temp = itemDictionary;
        itemDictionary = new Dictionary<string, InventorySlot>();
        foreach (string ids in temp.Keys)
        {
            for(int i = 0; i < inventory.Count; i++)
            {
                if(inventory[i].data.id.Equals(ids))
                    itemDictionary.Add(ids, inventory[i]);
            }
        }
    }
}
