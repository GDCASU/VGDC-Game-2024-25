using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author: TJ (Yousuf)
 * 
 * 
 * Modified By:
 * Ian Fletcher
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose: Differentiate between game items
 */// --------------------------------------------------------

/// <summary>
/// Enum defining all different types of collectibles
/// </summary>
public enum CollectibleType
{
    InventoryItem,
    Ammo
}

[CreateAssetMenu(menuName = "Inventory Item Data")]
public class ItemData : ScriptableObject
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    [Header("Data")]
    [SerializeField] public string id;
    [SerializeField] public string displayName;
    [SerializeField] public Sprite image;
    [SerializeField] [Range(1,10)] public int value;
    [SerializeField] public GameObject prefab;
    [SerializeField] public CollectibleType itemType;
    [SerializeField] public Elements element;
}
