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
    public bool isScrolling, isScrollingDown;
    private float angle;
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
        if (data != null)
        {
            switch (data.itemType)
            {
                case CollectibleType.Ammo:
                    switch (data.element)
                    {
                        case AmmoType.None:
                            stackSize = 99;
                            break;
                    }
                    break;
            }
        }
        if (scrollAnyways) { ScrollUpThroughAmmo(); }
        if ( ammoSlotNumber == 1 && stackSize <= 0)
        {
            for (int i = 0; i < transform.parent.childCount; i++)
            {
                transform.parent.GetChild(i).GetComponent<AmmoSlot>().scrollUpAnyways = true;
                transform.parent.GetChild(i).GetComponent<AmmoSlot>().isScrolling = false;
                transform.parent.GetChild(i).GetComponent<AmmoSlot>().isScrollingDown = false;
            }
        }
        if ((Input.GetAxisRaw("Mouse ScrollWheel") > 0 && !isScrollingDown && InventorySystem.Instance.ammos.Count > 1) || isScrolling && InventorySystem.Instance.ammos.Count > 1 || scrollUpAnyways)
        {
            if (!isScrolling && InventorySystem.Instance.ammos.Count > 1)
            {
                for (int i = 0; i < transform.parent.childCount; i++)
                {
                    if (transform.parent.GetChild(i).GetComponent<AmmoSlot>().ammoSlotNumber == 4)
                    {
                        if (transform.parent.GetChild(i).GetComponent<AmmoSlot>().IsSlotEmpty())
                        {
                            checkFor3rdSlot = true;
                            if (checkFor3rdSlot)
                            {
                                for (int k = 0; k < transform.parent.childCount; k++)
                                {
                                    if (transform.parent.GetChild(k).GetComponent<AmmoSlot>().ammoSlotNumber == 3)
                                    {
                                        if (transform.parent.GetChild(k).GetComponent<AmmoSlot>().IsSlotEmpty())
                                        {
                                            for (int l = 0; l < transform.parent.childCount; l++)
                                            {
                                                transform.parent.GetChild(l).GetComponent<AmmoSlot>().SwitchAmmoSlots();
                                            }
                                        }
                                    }
                                    checkFor3rdSlot = false;
                                }
                            }
                            for (int j = 0; j < transform.parent.childCount; j++)
                            {
                                transform.parent.GetChild(j).GetComponent<AmmoSlot>().SwitchAmmoSlots();
                            }
                        }
                    }
                }
                for (int i = 0; i < transform.parent.childCount; i++)
                {
                    transform.parent.GetChild(i).GetComponent<AmmoSlot>().SwitchAmmoSlots();
                    transform.parent.GetChild(i).GetComponent<AmmoSlot>().isScrolling = true;
                }
            }
            ScrollUpThroughAmmo();
        }
        else if ((Input.GetAxisRaw("Mouse ScrollWheel") < 0 && InventorySystem.Instance.ammos.Count > 1 && !isScrolling) || isScrollingDown && InventorySystem.Instance.ammos.Count > 1)
        {
            if (!isScrollingDown && InventorySystem.Instance.ammos.Count > 1)
            {
                for (int i = 0; i < transform.parent.childCount; i++)
                {
                    if (transform.parent.GetChild(i).GetComponent<AmmoSlot>().ammoSlotNumber == 2)
                    {
                        if (transform.parent.GetChild(i).GetComponent<AmmoSlot>().IsSlotEmpty())
                        {
                            checkFor3rdSlot = true;
                            if (checkFor3rdSlot)
                            {
                                for (int k = 0; k < transform.parent.childCount; k++)
                                {
                                    if (transform.parent.GetChild(k).GetComponent<AmmoSlot>().ammoSlotNumber == 3)
                                    {
                                        if (transform.parent.GetChild(k).GetComponent<AmmoSlot>().IsSlotEmpty())
                                        {
                                            for (int l = 0; l < transform.parent.childCount; l++)
                                            {
                                                transform.parent.GetChild(l).GetComponent<AmmoSlot>().SwitchAmmoSlotsDown();
                                            }
                                        }
                                    }
                                    checkFor3rdSlot = false;
                                }
                            }
                            for (int j = 0; j < transform.parent.childCount; j++)
                            {
                                transform.parent.GetChild(j).GetComponent<AmmoSlot>().SwitchAmmoSlotsDown();
                            }
                        }
                    }
                }
                for (int i = 0; i < transform.parent.childCount; i++)
                {
                    transform.parent.GetChild(i).GetComponent<AmmoSlot>().SwitchAmmoSlotsDown();
                    transform.parent.GetChild(i).GetComponent<AmmoSlot>().isScrollingDown = true;
                }
            }
            ScrollDownThroughAmmo();
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
            if (InventorySystem.Instance.ammos.Count <= 1)
            {
                for (int j = 0; j < transform.parent.childCount; j++)
                {
                    transform.parent.GetChild(j).GetComponent<AmmoSlot>().ammoSlotNumber = j + 1;
                    if (transform.parent.GetChild(j).GetComponent<AmmoSlot>().ammoSlotNumber == 1)
                        InventorySystem.Instance.MainAmmoSlot = transform.parent.GetChild(j);
                }
                for (int i = 0; i < transform.parent.childCount; i++)
                {
                    transform.parent.GetChild(i).GetComponent<AmmoSlot>().isScrolling = false;
                    transform.parent.GetChild(i).GetComponent<AmmoSlot>().isScrollingDown = false;
                    transform.parent.GetChild(i).GetComponent<AmmoSlot>().scrollAnyways = true;
                }
            }
            else
            {
                for (int i = 0; i < transform.parent.childCount; i++)
                {
                    transform.parent.GetChild(i).GetComponent<AmmoSlot>().scrollUpAnyways = true;
                }
            }
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

    /// <summary>
    /// Used to rotate the UI Counter-Clockwise to the new MainAmmoSlot Location
    /// </summary>
    public void ScrollUpThroughAmmo()
    {
        
        if (InventorySystem.Instance.ammos.Count > 1 || scrollAnyways && !isScrollingDown)
        {
            isScrolling = true;
            switch (ammoSlotNumber)
            {
                case 1:
                    Quaternion rotated = Quaternion.Euler(0, angle, 0);
                    transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, rotated, speed);
                    float y1 = Mathf.Round(Mathf.Abs(transform.parent.rotation.y) * 10000) / 10000;
                    float y2 = Mathf.Round(Mathf.Abs(rotated.y) * 10000) / 10000;
                    if (Mathf.Approximately(y1, y2) && isScrolling)
                    {
                        for (int i = 0; i < transform.parent.childCount; i++)
                        {
                            transform.parent.GetChild(i).GetComponent<AmmoSlot>().isScrolling = false;
                            transform.parent.GetChild(i).GetComponent<AmmoSlot>().scrollUpAnyways = false;
                        }
                        for (int i = 0; i < transform.parent.childCount; i++)
                        {
                            transform.parent.GetChild(i).GetComponent<AmmoSlot>().scrollAnyways = false;
                        }
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Used to rotate the UI Clockwise to the new MainAmmoSlot Location
    /// </summary>
    public void ScrollDownThroughAmmo()
    {

        if (InventorySystem.Instance.ammos.Count > 1 && !isScrolling)
        {
            isScrollingDown = true;
            switch (ammoSlotNumber)
            {
                case 1:
                    Quaternion rotated = Quaternion.Euler(0, angle, 0);
                    transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, rotated, speed);
                    float y1 = Mathf.Round(Mathf.Abs(transform.parent.rotation.y) * 10000) / 10000;
                    float y2 = Mathf.Round(Mathf.Abs(rotated.y) * 10000) / 10000;
                    if (Mathf.Approximately(y1, y2) && isScrollingDown)
                    {
                        for (int i = 0; i < transform.parent.childCount; i++)
                        {
                            transform.parent.GetChild(i).GetComponent<AmmoSlot>().isScrollingDown = false;
                        }
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Switches AmmoSlots Upwars/Counter-Clockwise
    /// </summary>
    public void SwitchAmmoSlots()
    {
        switch (ammoSlotNumber)
        {
            case 1:
                ammoSlotNumber = 2;
                break;
            case 2:
                ammoSlotNumber = 3;
                break;
            case 3:
                ammoSlotNumber = 4;
                break;
            case 4:
                ammoSlotNumber = 1;
                InventorySystem.Instance.MainAmmoSlot = this.transform;
                break;
        }
    }

    /// <summary>
    /// Switches AmmoSlots Downwards/Clockwise
    /// </summary>
    public void SwitchAmmoSlotsDown()
    {
        switch (ammoSlotNumber)
        {
            case 1:
                ammoSlotNumber = 4;
                break;
            case 2:
                ammoSlotNumber = 1;
                InventorySystem.Instance.MainAmmoSlot = this.transform;
                break;
            case 3:
                ammoSlotNumber = 2;
                break;
            case 4:
                ammoSlotNumber = 3;
                break;
        }
    }

    /// <summary>
    /// Checks if the AmmoSlot isn't assigned a data/ is empty
    /// </summary>
    /// <returns></returns>
    public bool IsSlotEmpty()
    {
        if (data == null)
            return true;
        else
            return false;
    }
}
