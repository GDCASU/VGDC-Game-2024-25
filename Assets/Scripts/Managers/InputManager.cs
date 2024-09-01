using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Manage the input of the player and correlate it to the game
 */// --------------------------------------------------------


/// <summary>
/// Manages all input made by the player and sends events
/// </summary>
public class InputManager : MonoBehaviour
{
    // Singleton Instance
    public static InputManager Instance;

    [Header("Input Settings")]
    [SerializeField] private float tempVal;

    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    // Input-Updated Values
    [HideInInspector] public Vector2 movementInput; // Vector2 for movement

    // Local Variables
    private PlayerControls playerControls;

    #region Player Events

    /// <summary> Player's Move Event </summary>
    public static event System.Action OnMove;

    /// <summary>
    /// Binds all of the Players' controls to their respective events.
    /// </summary>
    private void BindPlayerEvents()
    {
        // Subscribe to input events
    }

    #endregion

    #region UI Events

    /// <summary>
    /// Binds all of the UI Controls to their events
    /// </summary>
    private void BindUIEvents()
    {
        // Subscribe to input events
    }

    #endregion

    #region Unity Events

    private void Awake()
    {
        // Handle Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Control handling
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            BindPlayerEvents();
            BindUIEvents();
        }

        // Enable controls once all setup is done
        playerControls.Enable();
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    #endregion

    #region Input Map Change

    // TBD

    #endregion

    #region Event Handlers

    /// <summary>
    /// update the movement vector everytime the player moves
    /// </summary>
    private void HandleMovementInput(InputAction.CallbackContext context)
    {
        // Read value from input and set the movementInput Vector to it
        movementInput = context.ReadValue<Vector2>();
        if (doDebugLog) Debug.Log("The Movement Input read was = " + movementInput);
    }

    #endregion
}
