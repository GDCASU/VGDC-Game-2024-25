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
 * Handles the behaviour of the charge receptor
 */// --------------------------------------------------------


/// <summary>
/// Class that handles the behaviour of the charge receptor
/// </summary>
public class PuzzleChargeReceptor : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onActivate;
    [SerializeField] private UnityEvent onDeactivate;
    
    [Header("Readouts")]
    [InspectorReadOnly, SerializeField] private bool isActivated = false; 
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local fields
    private static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");

    public ReactionType TakeDamage(int damage, Elements element)
    {
        // Check if its the spark element
        if (element == Elements.Sparks && !isActivated)
        {
            // It was and it wasnt activated, activate
            isActivated = true;
            ChangeReceptorColor(Color.yellow);
            onActivate?.Invoke();
            return ReactionType.Undefined;
        }
        
        // Check if its water
        if (element == Elements.Water && isActivated)
        {
            isActivated = false;
            ChangeReceptorColor(Color.cyan);
            onDeactivate?.Invoke();
            return ReactionType.Undefined;
        }
        
        // Else do nothing
        return ReactionType.Undefined;
    }

    private void ChangeReceptorColor(Color color)
    {
        Color finalColor = color * 5f;
        meshRenderer.material.color = finalColor;
        meshRenderer.material.SetColor(emissionColor, finalColor);
    }
}
