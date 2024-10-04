using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // maybe use Events to figure out type?

    string[][] actionType;
    public string typeName = "Run";
    PlayerControls playerControls;

    private void Update()
    {
        if (playerControls != null)
        {
            string keybind = playerControls.UI.Interaction.bindings.ToString();

            print(keybind);
        }
    }

    public void StartInteraction() { }


    //-- Enemy Actions --//


    //-- NPC Actions --//


    //-- Object Actions --//
}
