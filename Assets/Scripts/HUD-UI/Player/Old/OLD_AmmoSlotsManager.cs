using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author: TJ
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:Scrolls throught the ammo
 * 
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>
[Obsolete]
public class OLD_AmmoSlotsManager : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    public static OLD_AmmoSlotsManager Instance {  get; private set; }

    private int currentAmmoSlotIndex = 0;
    private bool isScrolling, scrollAnyways;
    private float speed = 0.05f;
    float y1, y2;

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        } else { Instance = this; }

    }
    void Update()
    {
        if ((isScrolling || OLD_InventorySystem.Instance.ammos.Count < 2) && !scrollAnyways) { }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") > 0 || Input.GetKeyDown(KeyCode.E) || scrollAnyways)
        {
            isScrolling = true;
            scrollAnyways = false;
            StartCoroutine(ScrollUpThroughAmmo());
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0 || Input.GetKeyDown(KeyCode.Q))
        {
            isScrolling = true;
            StartCoroutine(ScrollDownThroughAmmo());
        }
    }

    IEnumerator ScrollUpThroughAmmo()
    {
        OLD_AmmoSlot oldAmmoSlot = transform.GetChild(currentAmmoSlotIndex).GetComponent<OLD_AmmoSlot>();
        do
        {
            currentAmmoSlotIndex++;
            if (currentAmmoSlotIndex > 3)
                currentAmmoSlotIndex = 0;
            oldAmmoSlot = transform.GetChild(currentAmmoSlotIndex).GetComponent<OLD_AmmoSlot>();
            yield return null;
        } while (oldAmmoSlot.data == null);
        OLD_InventorySystem.Instance.MainAmmoSlot = oldAmmoSlot.transform;
        float angle = oldAmmoSlot.angle;
        Quaternion rotated = Quaternion.Euler(0, angle, 0);
        y1 = Mathf.Round(Mathf.Abs(transform.rotation.y) * 10000) / 10000;
        y2 = Mathf.Round(Mathf.Abs(rotated.y) * 10000) / 10000;
        while (!WaitUntilRotated())
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotated, speed);
            y1 = Mathf.Round(Mathf.Abs(transform.rotation.y) * 10000) / 10000;
            yield return null;
        }
        isScrolling = false;
    }

    IEnumerator ScrollDownThroughAmmo()
    {
        OLD_AmmoSlot oldAmmoSlot = transform.GetChild(currentAmmoSlotIndex).GetComponent<OLD_AmmoSlot>();
        do
        {
            currentAmmoSlotIndex--;
            if (currentAmmoSlotIndex < 0)
                currentAmmoSlotIndex = 3;
            oldAmmoSlot = transform.GetChild(currentAmmoSlotIndex).GetComponent<OLD_AmmoSlot>();
            Debug.Log("AmmoSlot " + oldAmmoSlot.ammoSlotNumber + " : " + oldAmmoSlot.angle);
            yield return null;
        } while (oldAmmoSlot.data == null);
        OLD_InventorySystem.Instance.MainAmmoSlot = oldAmmoSlot.transform;
        float angle = oldAmmoSlot.angle;
        Quaternion rotated = Quaternion.Euler(0, angle, 0);
        y1 = Mathf.Round(Mathf.Abs(transform.rotation.y) * 10000) / 10000;
        y2 = Mathf.Round(Mathf.Abs(rotated.y) * 10000) / 10000;
        while (!WaitUntilRotated())
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, rotated, speed);
            y1 = Mathf.Round(Mathf.Abs(transform.rotation.y) * 10000) / 10000;
            yield return null;
        }
        isScrolling = false;
    }

    bool WaitUntilRotated()
    {
        return Mathf.Approximately(y1, y2);
    }

    public void AmmoFinished()
    {
        StopAllCoroutines();
        isScrolling = false;
        scrollAnyways = true;
    }

}