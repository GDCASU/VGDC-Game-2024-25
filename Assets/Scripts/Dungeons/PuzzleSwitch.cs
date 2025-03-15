using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Handle the switch puzzle piece
 */// --------------------------------------------------------


/// <summary>
/// Class that Handles a Puzzel Switch
/// </summary>
public class PuzzleSwitch : Interactable
{
    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private Outline _outlineScript;
    
    [Header("Settings")] 
    [SerializeField] private bool _startOn;
    
    [Header("Readouts")]
    [InspectorReadOnly] public bool isOn = false;
    [InspectorReadOnly] public bool hasTriggeredOnce = false;
    [InspectorReadOnly] public bool isTransitioning = false;
    
    [Header("Events")]
    [SerializeField] private UnityEvent _switchOnEvent;
    [SerializeField] private UnityEvent _switchOffEvent;
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local Variables
    private int animatorIsOnParam = Animator.StringToHash("IsOn");

    private void Awake()
    {
        _outlineScript.enabled = false;
        isOn = _startOn;
        _animator.SetBool(animatorIsOnParam, isOn);
    }

    private void LateUpdate()
    {
        // If we are in transition, do nothing
        if (isTransitioning) return;
        
        // Dont run code here if already triggered the event
        if (hasTriggeredOnce) return;

        // Ensure the animator state matches our boolean
        bool animatorState = _animator.GetBool(animatorIsOnParam);

        if (animatorState)
        {
            // Was on, trigger event
            _switchOnEvent.Invoke();
            hasTriggeredOnce = true;
            if (doDebugLog) Debug.Log("Switch turned ON");
        }
        else
        {
            _switchOffEvent.Invoke();
            hasTriggeredOnce = true;
            if (doDebugLog) Debug.Log("Switch turned OFF");
        }
    }

    public override void OnInteractionExecuted()
    {
        // Don't allow interaction during animation
        if (isTransitioning) return;

        // Toggle the switch state
        isOn = !isOn;
        _animator.SetBool(animatorIsOnParam, isOn);

        // Start transition
        isTransitioning = true;
        hasTriggeredOnce = false;

        // Wait for animation completion
        StartCoroutine(WaitForAnimation());
    }
    
    /// <summary>
    /// Coroutine that waits for the animation to finish before allowing input again
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(GetAnimationLength()); // Wait for animation to finish
        isTransitioning = false;
    }

    private float GetAnimationLength()
    {
        // Retrieve animation state info
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length + 0.5f;
    }

    public override void OnFocusEnter()
    {
        _outlineScript.enabled = true;
    }

    public override void OnFocusExit()
    {
        _outlineScript.enabled = false;
    }
}
