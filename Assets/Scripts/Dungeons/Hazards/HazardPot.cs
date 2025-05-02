using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Script that will create a hazard, honestly needs more work
 */// --------------------------------------------------------


/// <summary>
/// Script used to make hazards upon hit with a projectile
/// </summary>
public class HazardPot : MonoBehaviour, IDamageable
{
    [Header("References")] 
    [SerializeField] private GameObject modelToDestroy;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private HazardProjectile hazardProjectile;
    [SerializeField] private HazardTile hazardTile;
    
    [Header("Settings")]
    [SerializeField] private int hazardRadius = 3;
    [SerializeField] private float force;
    
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    /// <summary>
    /// Function so projectiles can break the pot
    /// </summary>
    public ReactionType TakeDamage(int damage, Elements element, Vector3 direction)
    {
        
        // Spawn hazard pellets
        HazardProjectile hazardSpawner = Instantiate(hazardProjectile, spawnPoint.position, Quaternion.identity);
        hazardSpawner.radius = hazardRadius;
        hazardSpawner.tile = hazardTile;
        hazardSpawner.rb.AddForce(Vector3.up * force, ForceMode.Impulse);
        
        Destroy(gameObject);
        return ReactionType.Undefined;
    }

    ReactionType IDamageable.TakeDamage(int damage, Elements element)
    {
        throw new NotImplementedException();
    }
}
