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
    [Header("References")]
    public Rigidbody rb;
    
    [Header("Projectile Settings")]
    public HazardTile tile;
    public int radius;
    
    private void OnCollisionEnter(Collision collision)
    {
        // Create hazard at location
        GameGridManager.Instance.PlaceHazardTiles(radius, transform.position, tile);
        Destroy(gameObject);
    }
    
}
