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
 * Handle the status of the player, health, available weapons,
 * money, etc.
 */// --------------------------------------------------------


/// <summary>
/// Handles the status of the player
/// </summary>
public class PlayerStats : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool _doDebugLog;

    public int health;
    public int mana;
    public int experience;
    public List<string> elements = new List<string>();
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
