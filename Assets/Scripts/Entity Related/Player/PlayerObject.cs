using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 *
 * Modified By:
 *
 */ // --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Interface to the player object in the scene
 */ // --------------------------------------------------------


/// <summary>
/// Manager of all the components that make up the player
/// </summary>
public class PlayerObject : MonoBehaviour, IDamageable
{
    // Singleton
    public static PlayerObject Instance;
    
    [Header("Settings")]
    [SerializeField] private int maxHealth = 100;
    
    [Header("References")]
    [SerializeField] private PlayerAmmoManager playerAmmoManager;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private MagnetAttraction magnetAttraction;
    [SerializeField] private MultiAudioEmitter audioEmitter;
    
    [Header("Readouts")]
    [InspectorReadOnly] [SerializeField] private int currentHealth = 0;

    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")] 
    [SerializeField] private bool _doDebugLog;
    
    // Local Variables

    void Awake()
    {
        // Set the Singleton
        if (Instance != null && Instance != this)
        {
            // Already set, destroy this object
            Destroy(gameObject);
            return;
        }
        // Not set yet
        Instance = this;
        
        // Set up stats
        currentHealth = maxHealth;
    }

    void OnDestroy()
    {
        // Null singleton
        Instance = null;
    }

    /// <summary>
    /// Function called when the player is hit by a projectile.
    /// </summary>
    public ReactionType TakeDamage(int damage, Elements element)
    {
        // TODO: The player doesnt trigger reactions right?
        // Damage health
        currentHealth -= damage;
        // Check for death
        if (currentHealth <= 0)
        {
            // Player died
            OnDeath();
            return ReactionType.Undefined;
        }
        
        // Return undefined
        return ReactionType.Undefined;
    }

    /// <summary>
    /// Function called when the player's health drops to or below 0
    /// </summary>
    public void OnDeath()
    {
        // TODO: Unfinished
    }
}