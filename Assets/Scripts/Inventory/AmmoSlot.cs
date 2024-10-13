using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* -----------------------------------------------------------
 * Author: TJ & Jacob
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose: For Ammo Slots
 *
 * 
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>
public class AmmoSlot : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    public ItemData data { get; set; }
    public int stackSize { get; private set; }
    public Transform UISlot;

    public void Awake()
    {
        UISlot = this.transform;
        RefreshUI();
    }

    public void SetAmmoSlot(ItemData itemData)
    {
        // Set ItemData instance
        data = itemData;
        AddToStack(itemData.value);
        data.displayName = data.id;
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

    public int GetStackSize()
    {
        return stackSize;
    }

    public void ResetSlot()
    {
        data = null;
        stackSize = 0;
        RefreshUI();
    }

    public void RefreshUI()
    {
        try
        {
            UISlot.GetComponent<SpriteRenderer>().enabled = true;
            UISlot.GetComponent<SpriteRenderer>().sprite = data.image;
            switch (stackSize)
            {
                case 1: 
                    UISlot.localScale = new Vector3(0.01f, 0.01f, 1f);
                    break;
                case 2:
                    UISlot.localScale = new Vector3(0.015f, 0.015f, 1f);
                    break;
                case 3:
                    UISlot.localScale = new Vector3(0.02f, 0.02f, 1f);
                    break;
            }
        }
        catch
        {
            UISlot.GetComponent<SpriteRenderer>().sprite = null;
            UISlot.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

}
