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
    
    [Header("Events")]
    [SerializeField] private UnityEvent onActivate;
    [SerializeField] private UnityEvent onDeactivate;
    
    [Header("Readouts")]
    [InspectorReadOnly, SerializeField] private bool isActivated = false; 
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public ReactionType TakeDamage(int damage, Elements element)
    {
        // Check if its the spark element
        return ReactionType.Undefined;
    }
}
