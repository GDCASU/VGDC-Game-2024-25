using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 *
 * Merging work from:
 * Davyd Yehudin
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Handle the status effects on the entity, since status effects can be applied to anything, not just enemies
 */// --------------------------------------------------------


/// <summary>
/// Handles the status effects on the entity
/// </summary>
public class ElementStatusHandler : MonoBehaviour
{
    
    
    //[Header("Current")]
    public StatusEffect currentStatusEffect;
    public Coroutine statusEffectCo;
    
    // Interface must be set by the entity script 
    // so status effects can deal damage over time
    public IDamageable damageable; 
    
    // Use this bool to gate all your Debug.Log Statements please
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
}
