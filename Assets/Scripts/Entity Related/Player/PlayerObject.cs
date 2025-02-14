using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 *
 * Modified By:
 * Sam Cahill
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

    // Local variables
    private Dictionary<Collider, ItemPickups> cachedScripts = new(); // Script Cache

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

    void OnTriggerEnter(Collider other)
    {
        // Check if the collider is already cached
        if (cachedScripts.ContainsKey(other)) return; // It does, dont need to do anything

        // Check if the item is not of type collectible
        if (!other.CompareTag(TagDefinitions.CollectibleTag)) return; // Not a collectible

        // Else, add it to the dictionary
        ItemPickups scr = other.GetComponent<ItemPickups>();
        if (scr == null) return; // dont add if null
        cachedScripts.Add(other, scr);
    }

    // Runs per collider found within the sphere
    private void OnTriggerStay(Collider other)
    {
        // Check if its on cache
        if (!cachedScripts.TryGetValue(other, out ItemPickups itemPickup)) return; // Not on cache

        // Check which type of collectible it is
        switch (itemPickup.itemData.itemType)
        {
            case CollectibleType.InventoryItem:
                HandleInventoryItem(itemPickup);
                break;
            default:
                // TODO: HANDLE OTHER TYPES
                Debug.LogError("ERROR! ITEM TYPE CASE NOT DEFINED! We dont know how to handle = " + itemPickup.itemData.itemType);
                return;
        }
    }

    // Remove the collider from the cache when it exits the trigger
    private void OnTriggerExit(Collider other)
    {
        // Remove from cache
        cachedScripts.Remove(other);
    }

    private void HandleInventoryItem(ItemPickups itemPickup)
    {
        if ( itemPickup.itemData.id == "HealthPickup")
        {
            if (currentHealth < maxHealth)
            {
                currentHealth += itemPickup.itemData.value;
                Destroy(itemPickup.gameObject);
                return;
            }
        }
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