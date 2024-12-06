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
public class AmmoSlotsManager : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    public static AmmoSlotsManager Instance {  get; private set; }

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
        if ((isScrolling || InventorySystem.Instance.ammos.Count < 2) && !scrollAnyways) { }
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
        AmmoSlot ammoSlot = transform.GetChild(currentAmmoSlotIndex).GetComponent<AmmoSlot>();
        do
        {
            currentAmmoSlotIndex++;
            if (currentAmmoSlotIndex > 3)
                currentAmmoSlotIndex = 0;
            ammoSlot = transform.GetChild(currentAmmoSlotIndex).GetComponent<AmmoSlot>();
            yield return null;
        } while (ammoSlot.data == null);
        InventorySystem.Instance.MainAmmoSlot = ammoSlot.transform;
        float angle = ammoSlot.angle;
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
        AmmoSlot ammoSlot = transform.GetChild(currentAmmoSlotIndex).GetComponent<AmmoSlot>();
        do
        {
            currentAmmoSlotIndex--;
            if (currentAmmoSlotIndex < 0)
                currentAmmoSlotIndex = 3;
            ammoSlot = transform.GetChild(currentAmmoSlotIndex).GetComponent<AmmoSlot>();
            Debug.Log("AmmoSlot " + ammoSlot.ammoSlotNumber + " : " + ammoSlot.angle);
            yield return null;
        } while (ammoSlot.data == null);
        InventorySystem.Instance.MainAmmoSlot = ammoSlot.transform;
        float angle = ammoSlot.angle;
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