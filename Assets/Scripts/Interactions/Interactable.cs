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
 * Purpose:
 * Create an interface all scripts can use for linking with the
 * interaction system (Lore Notes, Switches, Etc.)
 * The item must have a non-trigger collider with a rigidbody
 * and have the interactable script be on  the base object.
 * I suggest making the rigidbody kinematic if the object doesnt move
 */// --------------------------------------------------------


/// <summary>
/// Abstract Class that defines functions for interactions
/// </summary>
public abstract class Interactable : MonoBehaviour
{

    #region Interaction Functions

    /// <summary>
    /// <para>Executes Once</para>
    /// Will trigger only once when the player enters the range of the interactable
    /// </summary>
    public System.Action OnInteractionEnter;

    /// <summary>
    /// <para>Executes every frame if on range</para>
    /// Will run if the player interaction radius is in range
    /// </summary>
    public System.Action OnInteractionStay;

    /// <summary>
    /// <para>Executes Once</para>
    /// Will execute if the object is the closest to the player
    /// </summary>
    public System.Action OnFocusEnter;

    /// <summary>
    /// <para>Executes every frame if on range</para>
    /// Will run if the object is the closest to the player on the frame
    /// </summary>
    public System.Action OnFocusStay;

    /// <summary>
    /// <para>Executes Once</para>
    /// Will execute when the object no longer is the closest to the player
    /// </summary>
    public System.Action OnFocusExit;

    /// <summary>
    /// <para>Executes Per Input</para>
    /// Once the player hits the interaction key, this will execute
    /// </summary>
    public System.Action OnInteractionExecuted;

    /// <summary>
    /// <para>Executes Once</para>
    /// Will execute when the player interaction radious no longer reaches the object
    /// </summary>
    public System.Action OnInteractionExit;

    #endregion



}
