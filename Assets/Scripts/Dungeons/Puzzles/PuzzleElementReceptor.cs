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
 * Handles the element receptor puzzle object
 */// --------------------------------------------------------


/// <summary>
/// Class that handles a puzzle receptor object
/// </summary>
public class PuzzleElementReceptor : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private List<MeshRenderer> statusMeshRenderers;
    
    [Header("Settings")]
    [SerializeField] private Elements receptorElement;
    [SerializeField, Range(0f,255f)] private float inactiveTransparency;

    [Header("Events")] 
    [SerializeField] private UnityEvent OnRequirementCompleted;
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local variables
    private static readonly int emissionColor = Shader.PropertyToID("_EmissionColor");
    
    void Start()
    {
        // Make sure that the sprite is semi transparent
        Color color = spriteRenderer.color;
        color.a = inactiveTransparency;
        spriteRenderer.color = color;
    }
    
    // We have been hit by an element
    public ReactionType TakeDamage(int damage, Elements element, Vector3 direction)
    {
        // Check if its the matching element
        if (element != receptorElement) return ReactionType.Undefined;
        
        // Else it did match, light up sprite and 
        Color color = spriteRenderer.color;
        color.a = 255f;
        spriteRenderer.color = color;
        
        // Change status pillars to green
        foreach (MeshRenderer statusMeshRenderer in statusMeshRenderers)
        {
            Color finalColor = Color.green * 5f;
            statusMeshRenderer.material.color = finalColor;
            statusMeshRenderer.material.SetColor(emissionColor, finalColor);
        }
        
        // Raise event
        OnRequirementCompleted?.Invoke();
        
        return ReactionType.Undefined;
    }
}
