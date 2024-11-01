using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

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
    private float speed = 0.05f;
    public int stackSize { get; private set; }
    public Transform UISlot;
    [SerializeField] public int ammoSlotNumber;
    private bool isScrolling, isScrollingDown;
    public float angle;
    private bool checkFor3rdSlot, scrollAnyways, scrollUpAnyways;

    public void Awake()
    {
        UISlot = this.transform;
        RefreshUI();
        if (ammoSlotNumber <= 0 || ammoSlotNumber > 4)
        {
            Debug.LogError("An ammo slot is not a assigned a number from 1 - 4, please check for the AmmoSlot script on the ammo slots in gameObject \"Player\" and assign a number (order should be counter-clockwise)");
        }
        switch(ammoSlotNumber)
        {
            case 1:
                angle = 25f;
                break;
            case 2:
                angle = 115f;
                break;
            case 3:
                angle = 205f;
                break;
            case 4:
                angle = 295f;
                break;
        }
    }

    public void Update()
    {
        if (ammoSlotNumber == 1)
        {
            stackSize = 99;
        }
    }

    /// <summary>
    /// Sets the AmmoSlot's data
    /// </summary>
    /// <param name="itemData"></param>
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
    /// Removes a certain amount from the stack and auto rotates to the next ammo (Counter-Clockwise)
    /// </summary>
    /// <param name="amount"></param>
    public void RemoveFromStack(int amount)
    {
        stackSize -= amount;
        RefreshUI();
        if (stackSize <= 0)
        {
            AmmoSlotsManager.Instance.AmmoFinished();
        }
    }

    /// <summary>
    /// Gets the StackSize of the AmmoSlot
    /// </summary>
    /// <returns></returns>
    public int GetStackSize()
    {
        return stackSize;
    }

    /// <summary>
    /// Resets AmmoSlot
    /// </summary>
    public void ResetSlot()
    {
        data = null;
        stackSize = 0;
        RefreshUI();
    }

    /// <summary>
    /// Refreshes the AmmoSlot UI
    /// </summary>
    public void RefreshUI()
    {
        try
        {
            UISlot.GetComponent<SpriteRenderer>().enabled = true;
            UISlot.GetComponent<SpriteRenderer>().sprite = data.image;
            switch (stackSize)
            {
                case 1:
                    UISlot.GetComponent<SpriteRenderer>().size = new Vector3(0.5f, 0.5f, 1f);
                    break;
                case 2:
                    UISlot.GetComponent<SpriteRenderer>().size = new Vector3(0.75f, 0.75f, 1f);
                    break;
                case 3:
                    UISlot.GetComponent<SpriteRenderer>().size = new Vector3(1f, 1f, 1f);
                    break;
                default:
                    if(stackSize > 3)
                    {
                        UISlot.GetComponent<SpriteRenderer>().size = new Vector3(1f, 1f, 1f);
                    }
                    break;
            }
        }
        catch
        {
            UISlot.GetComponent<SpriteRenderer>().sprite = null;
            UISlot.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

}
