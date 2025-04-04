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
 * Allow for triggering of cheats GTA style through typing, case insensitive
 */// --------------------------------------------------------


/// <summary>
/// Class that constantly checks the keyboard input for cheat codes if enabled
/// </summary>
public class CheatManager : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Settings")]
    [SerializeField] private bool enableCheats = true;
    
    [FormerlySerializedAs("_doDebugLog")]
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local variables
    private Dictionary<string, System.Action> _cheatCodes; // Stores cheat codes and their functions
    private const int MaxCheatLength = 11; // has to be 1+ the longest cheat code
    private string _currentInput = "";
    
    void Start()
    {
        // Define cheat codes and their associated functions
        _cheatCodes = new Dictionary<string, System.Action>
        {
            { "FULLHEALTH", FullHealth },
            { "MAXAMMO", MaxAmmo },
        };
    }
    
    void Update()
    {
        if (enableCheats)
        {
            DetectCheatInput();
        }
    }
    
    void DetectCheatInput()
    {
        foreach (char c in Input.inputString)
        {
            if (char.IsLetter(c)) // Accept only letters
            {
                _currentInput += c; // Keep input as typed (case-insensitive dictionary will handle matching)
                CheckForCheat(); // Check after each key press
            }
            else if (c == '\b' && _currentInput.Length > 0) // Handle backspace
            {
                _currentInput = _currentInput.Substring(0, _currentInput.Length - 1);
            }

            // Prevent excessive length by trimming the start (FIFO behavior)
            if (_currentInput.Length > MaxCheatLength)
            {
                _currentInput = _currentInput.Substring(_currentInput.Length - MaxCheatLength);
            }
        }
    }

    void CheckForCheat()
    {
        foreach (var cheat in _cheatCodes.Keys)
        {
            if (_currentInput.Contains(cheat, System.StringComparison.OrdinalIgnoreCase)) // Case-insensitive match
            {
                Debug.Log($"Cheat activated: {cheat}");
                _cheatCodes[cheat].Invoke(); // Execute the associated function
                _currentInput = ""; // Reset input buffer after activation
                break;
            }
        }
    }

    #region Cheat Functions
    
    /// <summary>
    /// Cheat that will set the player's health to its maximum
    /// </summary>
    private void FullHealth()
    {
        if (doDebugLog) Debug.Log("Triggered the Full Health cheat!");
        // Check if the player is in the current scene, else return
        if (PlayerObject.Instance == null) return;
        // Max the health of the player
        PlayerObject.Instance.AddHealth(int.MaxValue);
    }
    
    /// <summary>
    /// Cheat that will give the player full ammo on all element types
    /// </summary>
    private void MaxAmmo()
    {
        if (doDebugLog) Debug.Log("Triggered the Max Ammo cheat!");
        // Check if the player is in the current scene, else return
        if (PlayerObject.Instance == null) return;
        // Max out element storage
        ElementInventoryManager.Instance.AddAmmoToElement(Elements.Fire, int.MaxValue);
        ElementInventoryManager.Instance.AddAmmoToElement(Elements.Fungal, int.MaxValue);
        ElementInventoryManager.Instance.AddAmmoToElement(Elements.Sparks, int.MaxValue);
        ElementInventoryManager.Instance.AddAmmoToElement(Elements.Water, int.MaxValue);
    }

    #endregion
}
