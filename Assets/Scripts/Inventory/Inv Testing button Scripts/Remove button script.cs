using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * 
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>
public class Removebuttonscript : MonoBehaviour
{
    public ItemData data;

    public void buttonPressed()
    {
        InventorySystem.Instance.Remove(data);
    }
}
