using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Xml.Serialization;

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
public class InventorySlot: MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    // Make a ItemData instance
    public ItemData data { get; set; }
    public int stackSize {  get; private set; }
    public Transform UISlot;
    [SerializeField] private MouseSlotFollower mouseFollower;

    public void Awake()
    {
        UISlot = this.transform;
        RefreshUI();
        mouseFollower = transform.parent.parent.GetChild(2).GetComponent<MouseSlotFollower>();
    }

    /// <summary>
    /// Assigns an ItemData object to the inventory slot
    /// </summary>
    /// <param name="itemData"></param>
    public void SetInventorySlot(ItemData itemData)
    {
        // Set ItemData instance
        data = itemData;
        data.displayName = data.id;
        AddToStack(data.value);
        RefreshUI();
    }

    /// <summary>
    /// Imitates this InventorySlot; Used for MouseSlotFollower
    /// </summary>
    /// <param name="itemData"></param>
    public void ImitateSlot(ItemData itemData)
    {
        data = itemData;
        RefreshUI();
    }

    /// <summary>
    /// Adds a certain ammount to the stack
    /// </summary>
    /// <param name="amount"></param>
    public void AddToStack(int amount)
    {
        stackSize += amount;
        RefreshUI();
    }

    /// <summary>
    /// Removes a certain amount from the stack
    /// </summary>
    /// <param name="amount"></param>
    public void RemoveFromStack(int amount)
    {
        stackSize -= amount;
        RefreshUI();
    }
    /// <summary>
    /// Gets stack size
    /// </summary>
    /// <returns></returns>
    public int GetStackSize()
    {
        return stackSize;
    }

    public void SetStackSize(int amount) { stackSize = amount; RefreshUI();}

    /// <summary>
    /// Refreshes the inventory slot's UI
    /// </summary>
    public void RefreshUI()
    {
        try
        {
            if (stackSize > 1)
            {
                UISlot.GetChild(1).GetComponent<TMP_Text>().enabled = true;
                UISlot.GetChild(1).GetComponent<TMP_Text>().text = stackSize + "";
            }
            else
                UISlot.GetChild(1).GetComponent<TMP_Text>().enabled = false;

            UISlot.GetChild(0).GetComponent<Image>().enabled = true;
            UISlot.GetChild(0).GetComponent<Image>().sprite = data.image;
        }
        catch
        {
            UISlot.GetChild(0).GetComponent<Image>().sprite = null;
            UISlot.GetChild(0).GetComponent<Image>().enabled = false;
            UISlot.GetChild(1).GetComponent<TMP_Text>().enabled = false;
        }
    }
    /// <summary>
    /// Resets the inventory slot back to being empty
    /// </summary>
    public void ResetSlot()
    {
        data = null;
        stackSize = 0;
        RefreshUI();
    }

    public void onBeginDrag()
    {
        if (data != null)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetSlot(this);
            mouseFollower.selected = this;
        }
    }

    public void onEndDrag()
    {
        mouseFollower.Toggle(false);
    }

    public void onDrop()
    {
        ItemData temp = data;
        int tempStack = stackSize;
        ImitateSlot(mouseFollower.GetSlot().data);
        SetStackSize(mouseFollower.GetSlot().stackSize);
        mouseFollower.selected.ImitateSlot(temp);
        mouseFollower.selected.SetStackSize(tempStack);
        OLD_InventorySystem.Instance.UpdateInventory(this, mouseFollower.GetSlot());
    }
}
