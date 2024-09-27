using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author: TJ (Yousuf)
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose: Act as a slot in the inventory that can hold ItemData
 * 
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>
[System.Serializable]
public class InventorySlot
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Make a ItemData instance
    public ItemData data { get; private set; }
    public int stackSize {  get; private set; }

    // Constructor
    public InventorySlot(ItemData itemData)
    {
        // Set ItemData instance
        data = itemData;
        data.displayName = data.id;
        AddToStack(data.value);
    }

    public void AddToStack(int amount)
    {
        stackSize += amount;
    }

    // Removes 1 from item stack
    public void RemoveFromStack(int amount)
    {
        stackSize -= amount;
    }

    public int GetStackSize()
    {
        return stackSize;
    }
}
