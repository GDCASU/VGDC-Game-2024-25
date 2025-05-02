using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * Cami Lee
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
    [SerializeField] private float _tempVal;

    [Header("Debugging")]
    [SerializeField] private bool _doDebugLog;

    // Input-Updated Values
    [HideInInspector] public Vector2 movementInput; // Vector2 for movement
    [HideInInspector] public bool isAttacking = false;

    // Local Variables
    private PlayerControls _playerControls;

    #region Player Events

    /// <summary> Player's Move Event </summary>
    public static event System.Action OnMove;
    public static event System.Action OnAttack;

    public static event System.Action<bool> OnChangeElement;

    /// <summary> Player's UI Event </summary>
    public static event System.Action OnInteract;
    public static event System.Action OnPause;
    public static event System.Action OnChangeDialogue;

    /// <summary>
    /// Binds all of the Players' controls to their respective events.
    /// </summary>
    private void BindPlayerEvents()
    {
        // Subscribe to input events
        _playerControls.OnFoot.Move.performed += i => HandleMovementInput(i);
        _playerControls.PlayerActions.Attack.started += i => HandleAttackInput(i);
        _playerControls.PlayerActions.Attack.canceled += i => HandleAttackInput(i);
        _playerControls.PlayerActions.ChangeNextElement.performed += i => HandleChangeElementInput(i, true);
        _playerControls.PlayerActions.ChangePreviousElement.performed += i => HandleChangeElementInput(i, false);
    }

    #endregion

    #region UI Events

    /// <summary>
    /// Binds all of the UI Controls to their events
    /// </summary>
    private void BindUIEvents()
    {
        // Subscribe to input events
        _playerControls.UI.Interaction.performed += i => HandleInteractionInput(i);
        _playerControls.UI.Pause.performed += i => HandlePauseInput(i);
        _playerControls.UI.ContinueDialogue.performed += i => HandleDialogueInput(i);
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
        if (_playerControls == null)
        {
            _playerControls = new PlayerControls();
            BindPlayerEvents();
            BindUIEvents();
        }
        
        // Start Coroutines
        StartCoroutine(AttackRoutine());

        // Enable controls once all setup is done
        _playerControls.Enable();
    }

    private void OnDestroy()
    {
        if (_playerControls != null) { _playerControls.Disable(); }
        StopAllCoroutines();
    }

    #endregion

    #region Input Map Change

    /// <summary> TBD </summary>
    #endregion

    #region Event Handlers

    /// <summary>
    /// update the movement vector everytime the player moves
    /// </summary>
    private void HandleMovementInput(InputAction.CallbackContext context)
    {
        // Read value from input and set the movementInput Vector to it
        movementInput = context.ReadValue<Vector2>();
        if (_doDebugLog) Debug.Log("The Movement Input read was = " + movementInput);
    }

    private void HandleAttackInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isAttacking = true;
        }
        else
        {
            isAttacking = false;
        }
    }
    
    /// <summary>
    /// Helper Routine to be able to hold down attack
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            if (isAttacking)
            {
                OnAttack?.Invoke();
            }
            yield return null;
        }
    }

    private void HandleChangeElementInput(InputAction.CallbackContext context, bool doGoRight)
    {
        // Check if change to next or previous was called
        if (doGoRight)
        {
            // Is change to next element
            OnChangeElement?.Invoke(true);
            if(_doDebugLog) Debug.Log("Input: Change to next element");
        }
        else
        {
            // Is change to previous element
            OnChangeElement?.Invoke(false);
            if(_doDebugLog) Debug.Log("Input: Change to previous element");
        }
    }

    private void HandleInteractionInput(InputAction.CallbackContext context)
    {
        if (_doDebugLog) Debug.Log("Interacted");
        OnInteract?.Invoke();
    }

    private void HandlePauseInput(InputAction.CallbackContext context)
    {
        if (_doDebugLog) Debug.Log("Paused");
        OnPause?.Invoke();
    }

    private void HandleDialogueInput(InputAction.CallbackContext context)
    {
        if (_doDebugLog) Debug.Log("Dialogue was Changed");
        OnChangeDialogue?.Invoke();
    }
    #endregion
}
