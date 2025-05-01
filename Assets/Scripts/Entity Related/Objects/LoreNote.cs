using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/* -----------------------------------------------------------
 * Created by: Eliza Chook
 * 
 * Modified by: Cami Lee
 
 * Pupose: An interact script attached to objects to allow players to view lore notes 
 * by pressing 'E' on the keyboard or 'A' on console to stop all movement (pause game) and open a UI canvas showing the notes. 
 * Players press 'E'/'A' again to exit lore note and resume movement. (Works similar to TestInteraction)
 */// --------------------------------------------------------



public class LoreNote : Interactable
{
    [SerializeField] bool rangeOfObj; // placeholder bool to change functions
    private float fixedDeltaTime;//game's flow of time
    public GameObject targetObject; // Reference to the object with the Canvas
    [SerializeField] public Canvas canvas;//reference canvas UI (lore notes)
    [SerializeField] private Outline _outlineScript;

    // Start is called before the first frame update
    void Start()
    {
        //record current rate of frame/second in game
        this.fixedDeltaTime = Time.fixedDeltaTime;

        OnFocusEnter += EnableOutline;
        OnFocusExit += DisableOutline;
        OnInteractionExecuted += TriggerLoreNote;
    }

    void OnDestroy()
    {
        OnFocusEnter -= EnableOutline;
        OnFocusExit -= DisableOutline;
        OnInteractionExecuted -= TriggerLoreNote;
    }

    public void TriggerLoreNote()
    {
        if (Time.timeScale != 0) // pause game with inputKey ('T' for keyboard, 'A' for console)
        {
            Time.timeScale = 0;
            canvas.gameObject.SetActive(true); // enable lore note UI
        }
        else
        {
            Time.timeScale = 1; // resume game with inputKey ('T' for keyboard, 'A' for console)
            canvas.gameObject.SetActive(false); // disable lore note UI
        }
    }

    public void EnableOutline()
    {
        _outlineScript.enabled = true;
    }

    public void DisableOutline()
    {
        _outlineScript.enabled = false;
    }
}
