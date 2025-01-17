using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Using Merged work from
 * Davyd Yehudin, William Peng
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Handle everything that can happen to the enemy on a single script
 */// --------------------------------------------------------


/// <summary>
/// Handles everything about the enemy, health, status effects, death, etc.
/// </summary>
public class EntityScript : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private ElementStatusHandler elementStatusHandler;
    
    [Header("Entity Stats")]
    [SerializeField] private int currentHealth;

    [Header("Entity Settings")]
    [SerializeField] private int maxHealth = 10;

    [Header("Multipliers")] 
    [SerializeField] private DamageMultiplier damageMults;
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    // Local Variables


    private void Start()
    {
        // Set stats
        currentHealth = maxHealth;
        elementStatusHandler.damageable = this;
    }

    /// <summary>
    /// Function derived from interface to deal damage to this entity
    /// </summary>
    public ReactionType TakeDamage(int damage, Elements element)
    {
        // Compute damage through multiplier
        int newDamage = damageMults.ComputeDamage(damage, element);
        // Damage health
        currentHealth -= newDamage;
        if (currentHealth <= 0)
        {
            // Enemy died
            OnDeath();
            // FIXME:
            return ReactionType.Undefined;
        }
    }

    /// <summary>
    /// Called when the entity's health reaches zero
    /// </summary>
    private void OnDeath()
    {
        
    }
}

/// <summary>
/// Helper class to define damage multipliers on the inspector
/// </summary>
[Serializable]
public class DamageMultiplier
{
    // Settings
    [Range(0,10)] public float neutralMultiplier = 1f;
    [Range(0,10)] public float fungalMultiplier = 1f;
    [Range(0,10)] public float sparkMultiplier = 1f;
    [Range(0,10)] public float fireMultiplier = 1f;

    /// <summary>
    /// Function that returns a new damage value based on the multipliers
    /// </summary>
    public int ComputeDamage(int damage, Elements element)
    {
        float newDamage = damage;

        switch(element)
        {
            case Elements.Fire:
                newDamage *= fireMultiplier;
                break;
            case Elements.Fungal:
                newDamage *= fungalMultiplier;
                break;
            case Elements.Neutral:
                newDamage *= neutralMultiplier;
                break;
            case Elements.Sparks:
                newDamage *= sparkMultiplier;
                break;
            default:
                Debug.LogError("ERROR! ELEMENT NOT DEFINED ON SWITCH AT COMPUTE DAMAGE!");
                return damage;
        }
        // Round the new damage float and return it
        return Mathf.RoundToInt(newDamage);
    }
}










