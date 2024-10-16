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
    public static bool isScrolling, isScrollingDown; 
    private static float rotatedCounterClockAngle = 360f-65f;
    private static float rotatedClockAngle = 25f;
    private bool checkFor3rdSlot, scrollAnyways;

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
        if(scrollAnyways) { ScrollUpThroughAmmo(); }
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0 || isScrolling && InventorySystem.Instance.ammos.Count > 1)
        {
            if (!isScrolling && InventorySystem.Instance.ammos.Count > 1)
            {
                for (int i = 0; i < transform.parent.childCount; i++)
                {
                    if (transform.parent.GetChild(i).GetComponent<AmmoSlot>().ammoSlotNumber == 4)
                    {
                        rotatedCounterClockAngle -= transform.parent.GetChild(i).GetComponent<AmmoSlot>().IsSlotEmpty();
                        if (transform.parent.GetChild(i).GetComponent<AmmoSlot>().IsSlotEmpty() == 90f)
                        {
                            checkFor3rdSlot = true;
                            if (checkFor3rdSlot)
                            {
                                for (int k = 0; k < transform.parent.childCount; k++)
                                {
                                    if (transform.parent.GetChild(k).GetComponent<AmmoSlot>().ammoSlotNumber == 3)
                                    {
                                        rotatedCounterClockAngle -= transform.parent.GetChild(k).GetComponent<AmmoSlot>().IsSlotEmpty();
                                        if (transform.parent.GetChild(k).GetComponent<AmmoSlot>().IsSlotEmpty() == 90f)
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
            }
            ScrollUpThroughAmmo();
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0 || isScrollingDown && InventorySystem.Instance.ammos.Count > 1)
        {
            if (!isScrollingDown && InventorySystem.Instance.ammos.Count > 1)
            {
                for (int i = 0; i < transform.parent.childCount; i++)
                {
                    if (transform.parent.GetChild(i).GetComponent<AmmoSlot>().ammoSlotNumber == 2)
                    {
                        rotatedClockAngle += transform.parent.GetChild(i).GetComponent<AmmoSlot>().IsSlotEmpty();
                        if (transform.parent.GetChild(i).GetComponent<AmmoSlot>().IsSlotEmpty() == 90f)
                        {
                            checkFor3rdSlot = true;
                            if (checkFor3rdSlot)
                            {
                                for (int k = 0; k < transform.parent.childCount; k++)
                                {
                                    if (transform.parent.GetChild(k).GetComponent<AmmoSlot>().ammoSlotNumber == 3)
                                    {
                                        rotatedClockAngle += transform.parent.GetChild(k).GetComponent<AmmoSlot>().IsSlotEmpty();
                                        if (transform.parent.GetChild(k).GetComponent<AmmoSlot>().IsSlotEmpty() == 90f)
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
            }
            ScrollDownThroughAmmo();
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
        if (stackSize <= 0)
        {
            scrollAnyways = true;
        }
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

    public void ScrollUpThroughAmmo()
    {
        
        if (InventorySystem.Instance.ammos.Count > 1 || scrollAnyways)
        {
            isScrolling = true;
            Quaternion rotated = Quaternion.Euler(0, rotatedCounterClockAngle, 0);
            switch (ammoSlotNumber)
            {
                case 1:
                    transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, rotated, speed);
                    float y1 = Mathf.Round(Mathf.Abs(transform.parent.rotation.y) * 10000) / 10000;
                    float y2 = Mathf.Round(Mathf.Abs(rotated.y) * 10000) / 10000;
                    if (Mathf.Approximately(y1, y2))
                    {
                        Debug.Log("Done");
                        isScrolling = false;
                        scrollAnyways = false;
                        rotatedCounterClockAngle -= 90f;
                        if(rotatedCounterClockAngle <= 0f)
                        {
                            rotatedCounterClockAngle = 360f-65f;
                        }
                        for (int i = 0; i < transform.parent.childCount; i++)
                        {
                            transform.parent.GetChild(i).GetComponent<AmmoSlot>().SwitchAmmoSlots();
                        }
                    }
                    break;
            }
        }
    }

    public void ScrollDownThroughAmmo()
    {

        if (InventorySystem.Instance.ammos.Count > 1)
        {
            isScrollingDown = true;
            Quaternion rotated = Quaternion.Euler(0, rotatedClockAngle, 0);
            switch (ammoSlotNumber)
            {
                case 1:
                    transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, rotated, speed);
                    float y1 = Mathf.Round(Mathf.Abs(transform.parent.rotation.y) * 10000) / 10000;
                    float y2 = Mathf.Round(Mathf.Abs(rotated.y) * 10000) / 10000;
                    if (Mathf.Approximately(y1, y2))
                    {
                        Debug.Log("Done");
                        isScrollingDown = false;
                        rotatedClockAngle += 90f;
                        if (rotatedClockAngle >= 360f)
                        {
                            rotatedClockAngle = 25f;
                        }
                        for (int i = 0; i < transform.parent.childCount; i++)
                        {
                            transform.parent.GetChild(i).GetComponent<AmmoSlot>().SwitchAmmoSlotsDown();
                        }
                    }
                    break;
            }
        }
    }

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

    public float IsSlotEmpty()
    {
        if (data == null)
            return 90;
        else
            return 0;
    }
}
