using System;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author: TJ (Yousuf)
 * 
 * 
 * Modified By:
 * Ian Fletcher
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose: The Inventory Systen to store items
 * 
 */// --------------------------------------------------------


/// <summary>
/// The class that manages the whole inventory system
/// </summary>
[Obsolete]
public class OLD_InventorySystem : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    // Create a private Dictionary to store ItemData with InventoryItem(s)
    private Dictionary<string, InventorySlot> itemDictionary;
    private Dictionary<string, OLD_AmmoSlot> ammoDictionary;
    public List<InventorySlot> inventory { get; private set; }
    public List<OLD_AmmoSlot> ammos { get; private set; }
    public static OLD_InventorySystem Instance { get; private set; }

    // Access the UI for slots
    [SerializeField] private GameObject slotsHolder;
    [SerializeField] private GameObject ammoSlotHolder;
    [SerializeField] private ItemData neutralAmmo;
    [SerializeField] public Transform MainAmmoSlot;
    private GameObject[] slots;
    private GameObject[] ammoSlots;
    
    // Events
    public static System.Action OnElementInventoryChanged; // Should be changed on any change done to the inventory

    private void Awake()
    {
        UnityEngine.Debug.Log("Inventory Awake");
        inventory = new List<InventorySlot> ();
        ammos = new List<OLD_AmmoSlot>();
        // Set the Dictionary here
        itemDictionary = new Dictionary<string, InventorySlot> ();
        ammoDictionary = new Dictionary<string, OLD_AmmoSlot>();

        // Set the Singleton
        if (Instance != null && Instance != this)
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
        ammoSlots = new GameObject[ammoSlotHolder.transform.childCount];
        for (int i = 0; i < ammoSlotHolder.transform.childCount; i++)
        {
            ammoSlots[i] = ammoSlotHolder.transform.GetChild(i).gameObject;
        }
        Add(neutralAmmo);
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
        else if (ammoDictionary.TryGetValue(itemData.id, out OLD_AmmoSlot avalue) && avalue.stackSize < 3) //Dictionary.TryGetValue(ItemData, out InventorySlot value
        {
            int amount = itemData.value;
            // Add to stack
            avalue.AddToStack(amount);
            UnityEngine.Debug.Log("item added = " + itemData.displayName);
            UnityEngine.Debug.Log("Stack = " + avalue.stackSize);
        }
        // Create new InventoryItem instance if item doesn't exist
        else
        {
            switch (itemData.itemType)
            {
                case CollectibleType.InventoryItem :
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
                    break;
                case CollectibleType.Ammo:
                    if (!ammoDictionary.TryGetValue(itemData.id, out OLD_AmmoSlot existingvalue))
                    {
                        for (int i = 0; i < ammoSlots.Length; i++)
                        {
                            OLD_AmmoSlot component = ammoSlots[i].GetComponent<OLD_AmmoSlot>();
                            UnityEngine.Debug.Log(component.data);
                            if (component.data == null)
                            {
                                component.SetAmmoSlot(itemData);
                                // Add item to inventory
                                ammos.Add(component);
                                // Add Item with ItemData to dictionary
                                ammoDictionary.Add(itemData.id, component);
                                UnityEngine.Debug.Log("New ammo detected = " + itemData.displayName);
                                UnityEngine.Debug.Log("Stack = " + component.stackSize);
                                break;
                            }
                        }
                    }
                    break;
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
            if (value.stackSize - amount <= 0) // value stack == 0
            {
                // Remove value from inventory
                inventory.Remove(value);
            }
            value.RemoveFromStack(amount);
            if (value.stackSize <= 0) // value stack == 0
            {
                itemDictionary.Remove(itemData.id);
                value.ResetSlot();
            }
            UnityEngine.Debug.Log("item removed = " + itemData.displayName);
            UnityEngine.Debug.Log("Stack = " + value.stackSize);

            //If stack == 0, remove item instance
        }
        else if (ammoDictionary.TryGetValue(itemData.id, out OLD_AmmoSlot avalue)) //Dictionary.TryGetValue(ItemData, out InventorySlot value
        {
            int amount = itemData.value;
            // Remove from stack
            if (avalue.stackSize - amount <= 0) // value stack == 0
            {
                ammos.Remove(avalue);
            }
            avalue.RemoveFromStack(amount);
            UnityEngine.Debug.Log("item removed = " + itemData.displayName);
            UnityEngine.Debug.Log("Stack = " + avalue.stackSize);

            //If stack == 0, remove item instance
            if (avalue.stackSize <= 0) // value stack == 0
            {
                // Remove value from inventory
                ammos.Remove(avalue);
                // Remove Item with ItemData from dictionary
                ammoDictionary.Remove(itemData.id);
                avalue.ResetSlot();
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

    public bool CheckForAmmo()
    {
        if(ammos.Count <= 0) return false;
        else return true;
    }

    public Elements GetSelectedAmmo()
    {
        try
        {
            ItemData selectedAmmo = MainAmmoSlot.GetComponent<OLD_AmmoSlot>().data;
            return selectedAmmo.element;
        } catch { return Elements.None; }
    }

    /// <summary>
    /// Checks if the ammo type is not full (3)
    /// </summary>
    /// <param name="ammo"></param>
    /// <returns></returns>
    public bool CheckIfAmmoNotFull(ItemData ammo)
    {
        switch (ammo.itemType)
        {
            case CollectibleType.Ammo:
                if (ammoDictionary.TryGetValue(ammo.id, out OLD_AmmoSlot avalue))
                {
                    return avalue.stackSize < 3;
                }
                else
                    return true;
            default:
                UnityEngine.Debug.Log("Not an Ammo type item");
                break;
        }
        return false;
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

    public void UpdateInventory(InventorySlot placedOn, InventorySlot placedBefore)
    {
        Dictionary<string, InventorySlot> temp = itemDictionary;
        itemDictionary = new Dictionary<string, InventorySlot>();
        bool matches = false;
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].data != null)
            {
                foreach (string key in temp.Keys)
                {
                    if (inventory[i].data.id.Equals(key))
                    {
                        itemDictionary.Add(inventory[i].data.id, inventory[i]);
                        matches = true;
                    }
                }
                if (!matches)
                {
                    inventory.RemoveAt(i);
                    i--;
                }
                else
                    matches = false;
            }
            else
            {
                inventory.RemoveAt(i);
                i--;
            }
        }

        inventory.Add(placedOn);
        itemDictionary.Add(placedOn.data.id, placedOn);
        try
        {
            itemDictionary.Add(placedBefore.data.id, placedBefore);
            inventory.Add(placedBefore);
        }
        catch { }
    }
}
