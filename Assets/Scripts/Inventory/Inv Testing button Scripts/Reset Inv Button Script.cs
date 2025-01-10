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
public class ResetInvButtonScript : MonoBehaviour
{

    public void buttonPressed()
    {
        OLD_InventorySystem.Instance.ResetInventory();
    }
}
