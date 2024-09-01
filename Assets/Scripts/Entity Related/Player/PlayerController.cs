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
 * Pupose:
 * Handle the movement and input of the player
 */// --------------------------------------------------------


/// <summary>
/// Controls the player
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float _speed;
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool _doDebugLog;

    // Ian HACK: We probably shouldnt use the character controller for
    // this, so script the movement system ourselves later for more granular control
    [SerializeField] private CharacterController _characterController;

    // Update is called once per frame
    void Update()
    {
        // Get Vector2 Input from Input Manager
        Vector3 input = InputManager.Instance.movementInput;

        // Since the player could be slightly rotated from the
        // world axis, we compute the rotation
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Compute the actual world space direction of movement for the player
        Vector3 moveDirWorldSpace = (forward * input.y + right * input.x).normalized;

        // Move Player
        _characterController.Move(Time.deltaTime * _speed * moveDirWorldSpace);
    }
}
