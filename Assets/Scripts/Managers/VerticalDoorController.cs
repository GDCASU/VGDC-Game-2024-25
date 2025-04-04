using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Written by Matthew Glos
 * 
 * Controls a vertical door that will go up when activated, and fall down when deactivated.
 * 
 * This script manages the movement of a door GameObject between two positions: a down position and an up position.
 * The door can be activated to move up or deactivated to move down at specified speeds.
 * 
 */
public class VerticalDoorController : MonoBehaviour
{
    [SerializeField] GameObject doorObject; // Reference to the door GameObject that will be moved.

    // Enum to define the possible states of the door.
    private enum DoorModes { down, up, movingDown, movingUp }

    private DoorModes doorMode = DoorModes.down; // Current state of the door.

    private Vector3 doorDownPosition; // The initial position of the door when it is down.
    [SerializeField] Vector3 doorUpOffset; // The offset from the down position to define the up position.
    [SerializeField] float doorUpSpeed; // Speed at which the door moves up.
    [SerializeField] float doorDownSpeed; // Speed at which the door moves down.

    /**
     * Initializes the door's down position at the start of the game.
     */
    public void Start()
    {
        doorDownPosition = doorObject.transform.position;
    }

    /**
     * Activates the door, causing it to move up if it is currently down or moving down.
     */
    public void Activate()
    {
        if (doorMode == DoorModes.down || doorMode == DoorModes.movingDown)
        {
            doorMode = DoorModes.movingUp;
        }
    }

    /**
     * Deactivates the door, causing it to move down if it is currently up or moving up.
     */
    public void Deactivate()
    {
        if (doorMode == DoorModes.movingUp || doorMode == DoorModes.up)
        {
            doorMode = DoorModes.movingDown;
        }
    }

    /**
     * Handles the door's movement
     */
    private void FixedUpdate()
    {
        switch (doorMode)
        {
            case DoorModes.down:
                // Door is already down, no action needed.
                break;

            case DoorModes.up:
                // Door is already up, no action needed.
                break;

            case DoorModes.movingDown:
                // Move the door towards the down position.
                doorObject.transform.position = Vector3.MoveTowards(doorObject.transform.position, doorDownPosition, Time.deltaTime * doorDownSpeed);
                break;

            case DoorModes.movingUp:
                // Move the door towards the up position.
                doorObject.transform.position = Vector3.MoveTowards(doorObject.transform.position, doorDownPosition + doorUpOffset, Time.deltaTime * doorUpSpeed);
                break;
        }
    }
}