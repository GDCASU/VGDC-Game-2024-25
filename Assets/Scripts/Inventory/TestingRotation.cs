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
public class TestingRotation : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    Quaternion rotated = Quaternion.Euler(0, 180, 0);
    private float speed = 0.025f;

    void Update()
    {
        transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, rotated, speed);
    }
}
