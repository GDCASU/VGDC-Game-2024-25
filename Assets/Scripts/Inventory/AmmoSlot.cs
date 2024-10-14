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
    private float speed = 0.02f;
    public int stackSize { get; private set; }
    public Transform UISlot;
    [SerializeField] private int ammoSlotNumber;
    public static bool isScrolling;
    private float rotatedCounterClockAngle = -90f;

    public void Awake()
    {
        UISlot = this.transform;
        RefreshUI();
        if (ammoSlotNumber <= 0 || ammoSlotNumber > 4)
        {
            Debug.LogError("An ammo slot is not a assigned a number from 1 - 4, please check for the AmmoSlot script on the ammo slots in gameObject \"Player\" and assign a number (order should be counter-clockwise)");
        }
    }

    public void Update()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0 || isScrolling)
        {
            ScrollUpThroughAmmo();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            Debug.Log("Scrolling down");
        }
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
            UISlot.GetComponent<SpriteRenderer>().enabled = true;
        }
    }

    public void ScrollUpThroughAmmo()
    {
        if (InventorySystem.Instance.ammos.Count > 1)
        {
            isScrolling = true;
            switch (ammoSlotNumber)
            {
                case 1:
                    Quaternion rotated = Quaternion.Euler(0f, rotatedCounterClockAngle, 0f);
                    transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, rotated, speed);
                    if (transform.parent.rotation == rotated)
                    {
                        isScrolling = false;
                        Debug.Log("Done");
                        rotatedCounterClockAngle -= 90f;
                    }
                    break;
                case 2:

                    break;
                case 3:

                    break;
                case 4:

                    break;
            }
        }
    }
}
