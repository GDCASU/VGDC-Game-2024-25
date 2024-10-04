using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public void Awake()
    {
        UISlot = this.transform;
        RefreshUI();
    }

    // Constructor
    public void SetInventorySlot(ItemData itemData)
    {
        // Set ItemData instance
        data = itemData;
        data.displayName = data.id;
        AddToStack(data.value);
        RefreshUI();
    }

    public void AddToStack(int amount)
    {
        stackSize += amount;
        RefreshUI();
    }

    // Removes 1 from item stack
    public void RemoveFromStack(int amount)
    {
        stackSize -= amount;
        RefreshUI();
    }

    public int GetStackSize()
    {
        return stackSize;
    }

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

    public void ResetSlot()
    {
        data = null;
        stackSize = 0;
        RefreshUI();
    }
}
