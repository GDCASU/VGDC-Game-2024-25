using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
 * Its a requirement node for unlocking a door that uses puzzle pieces
 */// --------------------------------------------------------


/// <summary>
/// Class that handles a puzzle node
/// </summary>
public class PuzzleNode : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;
    
    [Header("Readouts")] 
    [InspectorReadOnly] public bool isNodeActivated = false;
    [SerializeField] [InspectorReadOnly] private float brightness = 5f;
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local Variables
    public System.Action OnStatusUpdated;
    private static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");

    private void Start()
    {
        // Make sure it has emission enabled 
        meshRenderer.material.EnableKeyword("_EMISSION");
    }

    /// <summary>
    /// Should be called when the requirement for the node to be true is present
    /// </summary>
    public void ActivateNode()
    {
        isNodeActivated = true;
        Color finalColor = Color.green * brightness;
        meshRenderer.material.color = finalColor;
        meshRenderer.material.SetColor(emissionColor, finalColor);
        OnStatusUpdated?.Invoke();
    }

    public void DeactivateNode()
    {
        isNodeActivated = false;
        Color finalColor = Color.red * brightness;
        meshRenderer.material.color = finalColor;
        meshRenderer.material.SetColor(emissionColor, finalColor);
        OnStatusUpdated?.Invoke();
    }
    
    
}
