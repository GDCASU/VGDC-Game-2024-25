using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/* -----------------------------------------------------------
 * Author:
 * Matthew Glos
 *
 * Modified By:
 * Ian Fletcher
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Pressure plate system that provides Unity event hooks for when a valid object 
 * enters, stays, or leaves the trigger zone.
 * 
 * The pressure plate will only activate when an object with a valid tag enters the trigger zone.
 * It includes protection for multiple objects entering the zone and visual feedback for the plate's state.
 */// --------------------------------------------------------

public class PressurePlateController : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] UnityEvent objectEnterEvents = new UnityEvent(); // Event invoked when a valid object enters the trigger zone.
    [SerializeField] UnityEvent objectStayEvents = new UnityEvent();  // Event invoked while a valid object stays in the trigger zone.
    [SerializeField] UnityEvent objectLeaveEvents = new UnityEvent(); // Event invoked when a valid object leaves the trigger zone.
    
    [Header("References")]
    [SerializeField] private GameObject displayCube; // Reference to the visual representation of the pressure plate.
    
    [Header("Status Lights")]
    [SerializeField] private List<MeshRenderer> statusLights = new();
    
    [Header("Settings")]
    [SerializeField] private List<string> validObjects = new List<string>(); // List of tags that are considered valid for activating the pressure plate.
    [SerializeField] private float squishTime; // The speed at which the display cube scales between its neutral and down states.
    [SerializeField] private float activationDelay; // The time after which the pressure plate will actually activate
    [SerializeField] private Vector3 downScale; // The scale of the display cube when the plate is in the "down" position.
    
    [Header("Readouts")]
    [SerializeField] [InspectorReadOnly] private float brightness = 3f;
    [SerializeField] [InspectorReadOnly] private modeTypes currentMode = modeTypes.UP; // Current state of the pressure plate.
    
    // Enum to represent the two states of the pressure plate: up (not pressed) and down (pressed).
    private enum modeTypes
    {
        UP,
        DOWN
    } 
    
    // Local Variables
    private float activationTimer; // The timer that tracks the switch's delayed activation
    private Vector3 neutralScale; // The original scale of the display cube when the plate is in the "up" position.
    private static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");
    
    private void Start()
    {
        // Initialize the neutral scale of the display cube at the start of the game.
        neutralScale = displayCube.transform.localScale;
        activationTimer = activationDelay;
    }

    /// <summary>
    /// Called while a Collider stays within the trigger zone.
    /// If the object has a valid tag, it triggers the "ObjectStayEvents".
    /// </summary>
    public void OnTriggerStay(Collider other)
    {
        
        activationTimer -= Time.deltaTime;
        if (activationTimer < 0) {
            if (currentMode == modeTypes.UP && validObjects.Contains(other.tag))
            {
                objectEnterEvents.Invoke();
                Debug.Log("Set to green");
                ChangeMaterialOnLights(Color.green);
                currentMode = modeTypes.DOWN;
            }
        }

        if (validObjects.Contains(other.tag))
            objectStayEvents.Invoke();
    }

    /// <summary>
    /// <para> Called when a Collider exits the trigger zone. </para>
    /// If the object has a valid tag and the plate is in the "down" state,
    /// it triggers the "ObjectLeaveEvents" and changes the plate's state to "up".
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other)
    {
        if (validObjects.Contains(other.tag)) activationTimer = activationDelay;

        if (currentMode == modeTypes.DOWN && validObjects.Contains(other.tag))
        {
            objectLeaveEvents.Invoke();
            Debug.Log("Set to red");
            ChangeMaterialOnLights(Color.red);
            currentMode = modeTypes.UP;
        }
    }

    /// <summary>
    /// Handles the visual scaling of the display cube in
    /// FixedUpdate to ensure smooth transitions between states.
    /// </summary>
    public void FixedUpdate()
    {
        if (currentMode == modeTypes.DOWN)
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

    /// <summary>
    /// Helper func to change all materials on the light list
    /// </summary>
    private void ChangeMaterialOnLights(Color target)
    {
        foreach (MeshRenderer meshRenderer in statusLights)
        {
            Color finalColor = target * brightness;
            meshRenderer.material.color = finalColor;
            meshRenderer.material.SetColor(emissionColor, finalColor);
        }
    }
}