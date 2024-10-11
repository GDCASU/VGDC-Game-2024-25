using Gaskellgames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author: TJ (Yousuf)
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose: Differenciate between game items
 * 
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>

public enum CollectibleType
{
    InventoryItem,
    Ammo
}
[CreateAssetMenu(menuName = "Inventory Item Data")]
public class ItemData : GGScriptableObject
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    [SerializeField] public string id;
    [SerializeField] public string displayName;
    [SerializeField] public Sprite image;
    [SerializeField] public int value;
    [SerializeField] public GameObject prefab;
    [SerializeField] public CollectibleType itemType;
}
