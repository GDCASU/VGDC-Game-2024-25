using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Handle a set of nodes for puzzles. When all nodes are activated,
 * This node will trigger an event
 */// --------------------------------------------------------


/// <summary>
/// Class that handles the master node of the puzzles
/// </summary>
public class PuzzleMasterNode : MonoBehaviour
{
    [Header("Node List")]
    [SerializeField] private List<PuzzleNode> nodes = new List<PuzzleNode>();
    
    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;
    
    [Header("Events")]
    [SerializeField] private UnityEvent OnActivate;
    [SerializeField] private UnityEvent OnDeactivate; // Only called if the master node goes from active to inactive
    
    [Header("Readouts")]
    [SerializeField] [InspectorReadOnly] private bool isActivated = false;

    [SerializeField] [InspectorReadOnly] private float brightness = 5f;
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local Variables
    private static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");
    
    private void Start()
    {
        // Make sure the material has emission enabled
        meshRenderer.material.EnableKeyword("_EMISSION");
        // NOTE: Adding nodes at runtime is not supported, all nodes must be set
        // in the inspector before hitting play
        foreach (PuzzleNode node in nodes)
        {
            // Subscribe to the update event of each node
            node.OnStatusUpdated += OnNodeUpdatedRaised;
        }
    }
    
    
    /// <summary>
    /// Function that will be triggered each time a node changes state
    /// </summary>
    private void OnNodeUpdatedRaised()
    {
        foreach (PuzzleNode node in nodes)
        {
            // Check if all nodes are active/true
            if (!node.isNodeActivated)
            {
                // Check if it was activated before
                if (isActivated)
                {
                    // Node went from activated to deactivated, change color and raise event
                    Color finalColor = Color.yellow * brightness;
                    meshRenderer.material.color = finalColor;
                    meshRenderer.material.SetColor(emissionColor, finalColor);
                    OnDeactivate.Invoke();
                    isActivated = false;
                }
                return;
            }
        }
        
        // Else all nodes active, change properties and raise event
        if (!isActivated)
        {
            Color finalColor = Color.green * brightness;
            meshRenderer.material.color = finalColor;
            meshRenderer.material.SetColor(emissionColor, finalColor);
            OnActivate.Invoke();
            isActivated = true;
        }
    }
}
