using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem;
using TMPro;

/* -----------------------------------------------------------
 * Author:
 * Cami Lee
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Links to interaction key and allows player to perform
 * different actions. Use when interaction key is being used.
 */// --------------------------------------------------------


/// <summary>
/// Class that will protect all objects that are meant to be present on all scenes
/// </summary>
/// 
public class Interactions : MonoBehaviour
{
    string typeName; // name of current function
    System.Action currentEvent; // current function
    bool canInteract; // whether the player is near the interactable

    /// <summary> Initializes action sequence </summary>
    public void StartInteraction()
    {
        if (currentEvent == null) { Debug.LogError("No function assigned to currentEvent. Use ChangeInteraction() to assign a new interaction event."); }

        typeName = currentEvent.Method.Name;
        this.GetComponentInChildren<TMP_Text>().text = $"<b>{name}</b> \n {typeName}";
        this.GetComponentsInChildren<TMP_Text>()[1].text = $"{GetInputKey()}";

        InputManager.OnInteract += currentEvent;
        canInteract = true;
    }

    /// <summary> Prevents action from being called when not highlighted </summary>
    public void EndInteraction()
    {
        InputManager.OnInteract -= currentEvent;
        canInteract = false;
    }

    /// <summary> Call whenever a new action needs to be assigned to the object </summary>
    public void ChangeInteraction(System.Action newEvent)
    {
        // Resets action performed on interact key
        if (canInteract)
        {
            InputManager.OnInteract -= currentEvent;
            currentEvent = newEvent;
            InputManager.OnInteract += currentEvent;
        }
        else { currentEvent = newEvent; }
    }

    /// <summary> Class that checks for the last used input and updates keybinds accordingly </summary>
    public string GetInputKey() // update this if input changes
    {
        Gamepad gamepad = Gamepad.current;
        Keyboard keyboard = Keyboard.current;
        string input = "";

        if (gamepad != null)
        {
            if (gamepad is XInputController)
            {
                // Player is using an XBOX controller
                input = "A";
            }
        }
        else if (keyboard != null)
        {
            // Player is using a keyboard
            input = "T";
        }
        return input;
    }

}
