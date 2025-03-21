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
 * Handles a hazard pellet, it should get destroyed upon contact with anything
 * Honestly needs more work so it can work standalone
 */// --------------------------------------------------------


/// <summary>
/// Hanldes a hazard pellet
/// </summary>
public class HazardProjectile : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collision was of type ground
        Destroy(gameObject);
    }
    
}
