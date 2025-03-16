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
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local Variables
    public System.Action OnStatusUpdated;
    private static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");

    /// <summary>
    /// Should be called when the requirement for the node to be true is present
    /// </summary>
    public void ActivateNode()
    {
        isNodeActivated = true;
        meshRenderer.material.color = Color.green;
        meshRenderer.material.SetColor(emissionColor, Color.green);
        OnStatusUpdated?.Invoke();
    }

    public void DeactivateNode()
    {
        isNodeActivated = false;
        meshRenderer.material.color = Color.red;
        meshRenderer.material.SetColor(emissionColor, Color.red);
        OnStatusUpdated?.Invoke();
    }
    
    
}
