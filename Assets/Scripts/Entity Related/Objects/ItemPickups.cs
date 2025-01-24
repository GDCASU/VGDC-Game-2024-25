using System;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Jacob Kaufman-Warner
 *
 * Modified By:
 * Ian Fletcher
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Holds the itemData for the type of item that will be added to the inventory upon collection
 */// --------------------------------------------------------

public class ItemPickups : MonoBehaviour
{
    // Note: MagnetAttraction.cs handles the collection of the item
    [Header("Data")]
    public ItemData itemData;
    
    [Header("Settings")]
    public bool isMagnetAttractable = true; // Will make the item float to player
    
    // Local variables
    [HideInInspector] public Rigidbody rb; // Used by the magnet attraction script

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
}
