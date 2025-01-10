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
public class Addbuttonscript : MonoBehaviour
{
    public ItemData data;

    public void buttonPressed()
    {
        OLD_InventorySystem.Instance.Add(data);
    }
}
