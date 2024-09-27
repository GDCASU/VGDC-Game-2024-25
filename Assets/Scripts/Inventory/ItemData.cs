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

[CreateAssetMenu(menuName = "Inventory Item Data")]
public class ItemData : GGScriptableObject
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    [SerializeField] private string id;
    [SerializeField] public string name;
    [SerializeField] public Sprite image;
    [SerializeField] public int value;
    [SerializeField] public GameObject prefab;

    public string GetID() { return id; }
}
