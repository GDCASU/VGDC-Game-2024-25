using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;
using STOP_MODE = FMOD.Studio.STOP_MODE;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Make emitting sounds in the game easy
 */// --------------------------------------------------------


/// <summary>
/// An audio emitter with full customization options for behaviour
/// </summary>
public class SimpleAudioEmitter : MonoBehaviour
{
    [Header("Sound Reference")] 
    [SerializeField] private EventReference eventReference;
    
    [Header("Triggers")]
    [SerializeField] private EmitterGameEvent eventPlayTrigger = EmitterGameEvent.None;
    [SerializeField] private EmitterGameEvent eventStopTrigger = EmitterGameEvent.None;
    
    [Header("Settings")]
    [SerializeField] private PlayConditions playConditions = PlayConditions.PlayAlways;
    [SerializeField] [Range(0f,10f)] private float playCooldown = 0f;
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local Variables
    private StudioEventEmitter emitter;
    private List<EventInstance> eventInstances = new List<EventInstance>();
    private Coroutine cooldownRoutine;

    private void Awake()
    {
        // Create a studio event emitter component and set it up
        emitter = gameObject.AddComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        emitter.EventPlayTrigger = eventPlayTrigger;
        emitter.EventStopTrigger = eventStopTrigger;
    }
    
    private void Start()
    {
        // Don't do anything if the emitter is null
        if (!emitter) return;
        // Subscribe to audio events
        SoundManager.OnGamePaused += OnGamePaused;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        SoundManager.OnGamePaused -= OnGamePaused;
        
        // TODO IAN: Should probably also stop the sounds using the eventStopTrigger setting
        // But there's just so many different possibilities, and I think im better off focusing on other stuff
        
        // TODO: Stop OneShots?
    }

    /// <summary>
    /// Function to call to play the sound, using the conditions set in the inspector
    /// </summary>
    public void PlaySound()
    {
        // Stop sound playback if on cooldown
        if (cooldownRoutine != null) return;
        
        // Check the playback conditions
        switch (playConditions)
        {
            case PlayConditions.PlayAlways:
                PlayAsOneShot();
                break;
            case PlayConditions.InterruptAndPlay:
                StopAllOneShots(); // Stop all OneShot audios if playing
                emitter.Play(); // FMOD by default interrupts the current sound if you call play on it again
                break;
            case PlayConditions.PlayIfNotPlaying:
                if (!emitter.IsPlaying() && eventInstances.Count <= 0) emitter.Play();
                break;
            default:
                Debug.LogError("ERROR! PLAY CONDITION NOT DEFINED IN SIMPLE AUDIO EMITTER!");
                return;
        }
        // Trigger the cooldown if bigger than 0
        if (playCooldown > 0) cooldownRoutine = StartCoroutine(SoundCooldownRoutine());
    }

    /// <summary>
    /// Function to call to stop the sound
    /// </summary>
    public void StopSound()
    {
        // Stop the emitter
        emitter.Stop();
        // Stop all event instances on the list
        StopAllOneShots();
    }

    /// <summary>
    /// <para> Helper function that allows the playback of sounds as oneshots </para>
    /// NOTE: OneShot Audios when stopped cant be resumed
    /// </summary>
    private void PlayAsOneShot()
    {
        // Try to see if the emitter is free
        if (!emitter.IsPlaying())
        {
            // Emitter was free, play here
            emitter.Play();
            return;
        }
        // Else create an event instance and play it as a oneshot
        EventInstance instance = RuntimeManager.CreateInstance(eventReference);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        instance.start();
        instance.release();
        eventInstances.Add(instance);
        StartCoroutine(OneShotLifetimeRoutine(instance));
    }

    /// <summary>
    /// Helper function to stop all OneShot Sounds played previously if any
    /// </summary>
    private void StopAllOneShots()
    {
        foreach (EventInstance instance in eventInstances)
        {
            instance.stop(STOP_MODE.IMMEDIATE);
        }
    }

    /// <summary>
    /// Function that handles what happens to the audio when the game pauses,
    /// triggered by the event on the sound manager
    /// </summary>
    private void OnGamePaused()
    {
        // Don't do anything if the emitter is null
        if (!emitter) return;
    }

    /// <summary>
    /// Routine that acts as a timer to stop new sounds from being played based on a cooldown
    /// </summary>
    /// <returns></returns>
    private IEnumerator SoundCooldownRoutine()
    {
        float elapsedTime = 0f;

        // Run timer
        while (elapsedTime < playCooldown)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Finished cooldown
        cooldownRoutine = null;
    }

    /// <summary>
    /// Routine that will constantly check if its event instance finished to remove it from the list
    /// </summary>
    /// <returns></returns>
    private IEnumerator OneShotLifetimeRoutine(EventInstance instance)
    {
        PLAYBACK_STATE state;
        instance.getPlaybackState(out state);
        while (state != PLAYBACK_STATE.STOPPED)
        {
            instance.getPlaybackState(out state);
            yield return null;
        }
        // It stopped, remove from list
        eventInstances.Remove(instance);
    }

    /// <summary>
    /// Helper enum to set conditions on playback
    /// </summary>
    private enum PlayConditions
    {
        PlayAlways, // Will play the sound no matter what
        PlayIfNotPlaying, // Will only play the sound if not already playing
        InterruptAndPlay, // Will interrupt the sound if already playing and play again
    }

    // TODO: Would making a fadestop be annoying or...?
    /// <summary>
    /// Helper enum to set up how the sound should be stopped
    /// </summary>
    private enum StopStyle
    {
        HardStop, // Stops the sound immediatly
        FadeStop, // Fades Stops the sound
    }
    
}
