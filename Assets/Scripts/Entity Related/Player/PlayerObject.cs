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
 * Pupose:
 * Interface to the player object in the scene
 */// --------------------------------------------------------


/// <summary>
/// Handles the interaction between the player object and its inventory, data, etc.
/// </summary>
public class PlayerObject : MonoBehaviour
{
    // Singleton
    public static PlayerObject Instance;

    [Header("References")]
    public CapsuleCollider capsuleCollider;
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool _doDebugLog;

    private void Awake()
    {
        // Set the singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
}
