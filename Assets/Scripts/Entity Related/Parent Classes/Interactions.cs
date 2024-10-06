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
    string[][] actionType;
    string typeName;
    System.Action currentEvent;

    public void StartInteraction()
    {
        if (currentEvent == null) { Debug.LogError("No function assigned to currentEvent. Use ChangeInteraction() to assign a new interaction event."); }

        typeName = currentEvent.Method.Name;
        this.GetComponentInChildren<TMP_Text>().text = $"<b>{name}</b> \n {typeName}";
        this.GetComponentsInChildren<TMP_Text>()[1].text = $"{GetInputKey()}";
    }

    /// <summary> Call whenever a new action needs to be assigned to the object </summary>
    public void ChangeInteraction(System.Action newEvent)
    {
        // Resets action performed on interact key
        InputManager.OnInteract -= currentEvent;
        currentEvent = newEvent;
        InputManager.OnInteract += currentEvent;
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
            input = "E";
        }

        return input;
    }

}
