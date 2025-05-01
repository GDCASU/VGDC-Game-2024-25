using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 *
 * Merging work from:
 * Davyd Yehudin
 * 
 * Modified By:
 * Chandler Van
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
    [Header("References")] 
    [SerializeField] private GameObject fireStatusDisplay;
    [SerializeField] private GameObject waterStatusDisplay;
    [SerializeField] private GameObject sparkStatusDisplay;
    [SerializeField] private GameObject fungalStatusDisplay;
    
    [Header("Settings")]
    [SerializeField] private StatusSettings statusSettings;
    
    [Header("Readouts")]
    [InspectorReadOnly] [SerializeField] private StatusEffect currentStatusEffect = StatusEffect.Undefined;
    [InspectorReadOnly] [SerializeField] private float timeLeft = 0f;
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local Variables
    [HideInInspector] public EntityScript entityScript = null; // Set on the Entity Script 
    private Coroutine statusEffectCo;
    
    /// <summary>
    /// <para>Called to check if we need to apply a status effect to this enemy on impact </para>
    /// Also checks if the new element causes a reaction with the current status and returns the result
    /// </summary>
    /// <param name="element"> The element for which to check for a status effect</param>
    public ReactionType HandleElementStatus(Elements element)
    {
        // Get the status effect associated with the element
        StatusEffect statusEffect = StatusEffect.Undefined;
        System.Func<IEnumerator> targetRoutine = null;
        switch (element)
        {
            case Elements.Fire:
                statusEffect = StatusEffect.Burning;
                targetRoutine = BurningCoroutine;
                break;
            case Elements.Fungal:
                statusEffect = StatusEffect.Spored;
                targetRoutine = SporedCoroutine;
                break;
            case Elements.Water:
                break;
            case Elements.Sparks:
                statusEffect = StatusEffect.Charged;
                targetRoutine = ChargedCoroutine;
                break;
            case Elements.Neutral:
                return ReactionType.Undefined; // Neutral doesnt cause a reaction with anything
            default:
                Debug.LogWarning("ElementStatusHandler: HandleElementStatus: Unknown element");
                break;
        }
        
        // If no status effect was inflicted from this element, return early
        if (targetRoutine == null) return ReactionType.Undefined;
        
        // The element does carry an effect. Check if there's any status effect already active first
        if (currentStatusEffect == StatusEffect.Undefined)
        {
            // No status effect present, inflict it
            currentStatusEffect = statusEffect;
            //ShowStatusEffectDisplay(element);
            statusEffectCo = StartCoroutine(targetRoutine());
            
            // Since there wasnt any other status present, no reaction happens
            return ReactionType.Undefined;
        }
        
        // Otherwise, there was already another status present. if they are the same, refresh
        if (currentStatusEffect == statusEffect)
        {
            // Refresh routine
            if(statusEffectCo != null)
                StopCoroutine(statusEffectCo);
            //DisableAllStatusEffectDisplay();
            //ShowStatusEffectDisplay(element);
            statusEffectCo = StartCoroutine(targetRoutine());

            if (doDebugLog) Debug.Log("Status Effect Refreshed: " + currentStatusEffect.ToString(), gameObject);

            // Returns undefined since we only refreshed the timer
            return ReactionType.Undefined;
        }
        
        // Otherwise the elements/statuses are different, check for possible reaction
        ReactionType result = ReactionDefinitions.TryGetReaction(statusEffect, currentStatusEffect);
        // Check if it returned undefined
        if (result == ReactionType.Undefined) return ReactionType.Undefined;
        // Else it caused a reaction, stop the current status and return the reaction
        if (doDebugLog) Debug.Log("REACTION CAUSED! Reaction type: " + result.ToString());
        //DisableAllStatusEffectDisplay();
        if(statusEffectCo != null)
            StopCoroutine(statusEffectCo);
        
        //entityScript.healthBar.ResetHealthBarColor();
        statusEffectCo = null;
        currentStatusEffect = StatusEffect.Undefined;
        return result;
    }

    /// <summary>
    /// Function to enable the display of the specific status effect
    /// </summary>
    private void ShowStatusEffectDisplay(Elements element)
    {
        switch (element)
        {
            case Elements.Fire:
                fireStatusDisplay.SetActive(true);
                break;
            case Elements.Fungal:
                fungalStatusDisplay.SetActive(true);
                break; 
            case Elements.Water:
                waterStatusDisplay.SetActive(true);
                break;
            case Elements.Sparks:
                sparkStatusDisplay.SetActive(true);
                break;
            default:
                Debug.Log("ERROR! DISPLAY NOT FOUND");
                break;
        }
    }
    
    /// <summary>
    /// Disables all displays of element status
    /// </summary>
    private void DisableAllStatusEffectDisplay()
    {
        fireStatusDisplay.SetActive(false);
        waterStatusDisplay.SetActive(false);
        sparkStatusDisplay.SetActive(false);
        fungalStatusDisplay.SetActive(false);
    }

    /// <summary>
    /// Coroutine that handles the charged effect
    /// </summary>
    private IEnumerator ChargedCoroutine()
    {
        timeLeft = statusSettings.chargedDuration;

        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(statusSettings.stunInterval);

            // Apply stun
            entityScript.stunned = true;
            float _oldSpeed = entityScript.speedMult;
            entityScript.speedMult = 0;
            if (doDebugLog) Debug.Log("Stun Applied");

            timeLeft -= statusSettings.stunInterval;

            yield return new WaitForSeconds(statusSettings.stunDuration);

            // Remove stun
            entityScript.stunned = false;
            entityScript.speedMult = _oldSpeed;
            if (doDebugLog) Debug.Log("Stun Removed");

            timeLeft -= statusSettings.stunDuration;
        }

        timeLeft = 0f;
        currentStatusEffect = StatusEffect.Undefined;
        //DisableAllStatusEffectDisplay();
        statusEffectCo = null;
    }


    /// <summary>
    /// Coroutine that handles the burning effect
    /// </summary>
    private IEnumerator BurningCoroutine()
    {
        timeLeft = statusSettings.burnDuration;
        entityScript.healthBar.SetHealthBarColor(Color.red);
        float damageIntervalTime = statusSettings.burnIntervalTime; 
        // Run timer
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            damageIntervalTime -= Time.deltaTime;
            // Check if we passed the damage interval
            if (damageIntervalTime < 0f)
            {
                // Apply burning damage
                entityScript.StatusDamage(statusSettings.burnDmgPerInterval);
                // Reset interval tracker
                damageIntervalTime = statusSettings.burnIntervalTime;
            }
            // Wait a frame
            yield return null;
        }
        // Burning status ended
        timeLeft = 0f;
        currentStatusEffect = StatusEffect.Undefined;
        entityScript.healthBar.ResetHealthBarColor();
        //DisableAllStatusEffectDisplay();
        statusEffectCo = null;
    }
    
    /// <summary>
    /// Coroutine that handles the spored status.
    /// </summary>
    /// <returns></returns>
    private IEnumerator SporedCoroutine()
    {
        timeLeft = statusSettings.sporedDuration;
        entityScript.healthBar.SetHealthBarColor(Color.green);
        // Apply slowdown
        entityScript.speedMult = 1 - statusSettings.slowSpeedBy;
        // Run timer
        while (timeLeft > 0f)
        {
            timeLeft -= Time.deltaTime;
            yield return null;
        }
        // Spored effect finished
        timeLeft = 0f;
        entityScript.speedMult = 1f;
        entityScript.healthBar.ResetHealthBarColor();
        currentStatusEffect = StatusEffect.Undefined;
        //DisableAllStatusEffectDisplay();
        statusEffectCo = null;
    }
}

/// <summary>
/// Helper class to define element settings on the entity itself
/// </summary>
[Serializable]
public class StatusSettings
{
    [Header("Burning/Fire ****************")]
    public float burnDuration = 5f;
    public float burnIntervalTime = 1f;
    public int burnDmgPerInterval = 1;
    [Header("Spored/Fungal ***************")]
    public float sporedDuration = 5f;
    [Range(0f,1f)] public float slowSpeedBy = 0.3f; // EX: If set to 0.3, it will slow the entity by 30%
    [Header("Charged/Spark ***************")]
    public float chargedDuration = 10f;
    public float stunInterval = 1.0f;
    public float stunDuration = 0.5f;
}











