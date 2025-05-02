using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Kill shit
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>
public class KillBox : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;


    void OnCollisionEnter(Collision collision)
    {
        Destroy(collision.gameObject);
    }
}
