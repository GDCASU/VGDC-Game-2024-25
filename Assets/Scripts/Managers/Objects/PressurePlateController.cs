using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * Written by Matthew Glos
 * 
 * Pressure plate system that provides Unity event hooks for when a valid object 
 * enters, stays, or leaves the trigger zone.
 * 
 * The pressure plate will only activate when an object with a valid tag enters the trigger zone.
 * It includes protection for multiple objects entering the zone and visual feedback for the plate's state.
 * 
 */
public class PressurePlateController : MonoBehaviour
{
    [SerializeField] UnityEvent ObjectEnterEvents = new UnityEvent(); // Event invoked when a valid object enters the trigger zone.
    [SerializeField] UnityEvent ObjectStayEvents = new UnityEvent();  // Event invoked while a valid object stays in the trigger zone.
    [SerializeField] UnityEvent ObjectLeaveEvents = new UnityEvent(); // Event invoked when a valid object leaves the trigger zone.

    [SerializeField] List<string> validObjects = new List<string>(); // List of tags that are considered valid for activating the pressure plate.

    private enum modeTypes { up, down } // Enum to represent the two states of the pressure plate: up (not pressed) and down (pressed).
    private modeTypes mode = modeTypes.up; // Current state of the pressure plate.

    private Vector3 neutralScale; // The original scale of the display cube when the plate is in the "up" position.
    [SerializeField] GameObject displayCube; // Reference to the visual representation of the pressure plate.
    [SerializeField] Vector3 downScale; // The scale of the display cube when the plate is in the "down" position.
    [SerializeField] float squishTime; // The speed at which the display cube scales between its neutral and down states.
    [SerializeField] float activationDelay; // The time after which the pressure plate will actually activate
    private float activationTimer; // The timer that tracks the switch's delayed activation

    /**
     * Initializes the neutral scale of the display cube at the start of the game.
     */
    private void Start()
    {
        neutralScale = displayCube.transform.localScale;
        activationTimer = activationDelay;
    }

    /**
     * Called when a Collider enters the trigger zone.
     * If the object has a valid tag and the plate is in the "up" state, it triggers the "ObjectEnterEvents" and changes the plate's state to "down".
     */
    public void OnTriggerEnter(Collider other)
    {
        
    }

    /**
     * Called while a Collider stays within the trigger zone.
     * If the object has a valid tag, it triggers the "ObjectStayEvents".
     */ 
    public void OnTriggerStay(Collider other)
    {
        
        activationTimer -= Time.deltaTime;
        if (activationTimer < 0) {
            if (mode == modeTypes.up && validObjects.Contains(other.tag))
            {
                ObjectEnterEvents.Invoke();
                mode = modeTypes.down;
            }
        }

        if (validObjects.Contains(other.tag))
            ObjectStayEvents.Invoke();
    }

    /**
     * Called when a Collider exits the trigger zone.
     * If the object has a valid tag and the plate is in the "down" state, it triggers the "ObjectLeaveEvents" and changes the plate's state to "up".
     */
    public void OnTriggerExit(Collider other)
    {
        if (validObjects.Contains(other.tag))
        activationTimer = activationDelay;

        if (mode == modeTypes.down && validObjects.Contains(other.tag))
        {

            ObjectLeaveEvents.Invoke();
            mode = modeTypes.up;
        }
    }

    /**
     * Handles the visual scaling of the display cube in FixedUpdate to ensure smooth transitions between states.
     */
    public void FixedUpdate()
    {
        if (mode == modeTypes.down)
        {
            // Smoothly scale the display cube to the "down" position.
            displayCube.transform.localScale = Vector3.Lerp(displayCube.transform.localScale, downScale, Time.deltaTime * squishTime);
        }
        else
        {
            // Smoothly scale the display cube back to the "neutral" position.
            displayCube.transform.localScale = Vector3.Lerp(displayCube.transform.localScale, neutralScale, Time.deltaTime * squishTime);
        }
    }
}