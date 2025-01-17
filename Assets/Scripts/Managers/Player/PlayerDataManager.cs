using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Jerry Mou 
 *
 * Modified By:
 * Ian Fletcher
 */// --------------------------------------------------------

/// <summary>
/// Handle all the player data like health and the like
/// </summary>
public class PlayerDataManager : MonoBehaviour
{
    // Singleton
    public static PlayerDataManager Instance;

    public float health;
    public int mana;
    public int experience;

    // Reference to the current player controller, set in PlayerController.Start
    public PlayerController playerController;
    
    private void Awake()
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
    }
    
	/// <summary>
	/// Handles damage taken
	/// </summary>
	private void OnDamaged(float damage, float multiplier, Elements element, EnemyStatusEffect statusEffect)
	{
        // Subtract health
        health -= damage * multiplier;
		if(health < 0)
        {
            // Death event
            health = 0;
        }
	}
}
