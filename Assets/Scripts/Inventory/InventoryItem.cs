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
public class InventoryItem
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Make a ItemData instance
    public int stackSize {  get; private set; }

    // Constructor
    public InventoryItem(//ItemData)
    {
        // Set ItemData instance
        AddToStack();
    }

    // Adds 1 to item stack
    public void AddToStack()
    {
        stackSize++;
    }

    // Removes 1 from item stack
    public void RemoveFromStack()
    {
        stackSize--;
    }
}
