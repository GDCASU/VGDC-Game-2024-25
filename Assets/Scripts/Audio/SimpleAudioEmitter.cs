using System;
using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using PlayConditions = AudioEmitterSettings.PlayConditions;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Make emitting sounds in the game easy and conditionable
 */// --------------------------------------------------------


/// <summary>
/// An audio emitter with full customization options for behaviour
/// </summary>
public class SimpleAudioEmitter : MonoBehaviour
{
    // Settings
    public AudioEmitterSettings settings;
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local Variables
    private StudioEventEmitter emitter;
    private List<EventInstance> eventInstances = new List<EventInstance>();
    private Coroutine cooldownRoutine;
    
    private void Start()
    {
        // Create a studio event emitter component and set it up
        emitter = gameObject.AddComponent<StudioEventEmitter>();
        emitter.EventReference = settings.eventReference;
        emitter.EventPlayTrigger = settings.eventPlayTrigger;
        emitter.EventStopTrigger = settings.eventStopTrigger;
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
        
        // Boolean to not trigger cooldown if a sound didnt play after exiting the switch statement
        bool didPlay = false;
        // Check the playback conditions
        switch (settings.playConditions)
        {
            case PlayConditions.PlayAlways:
                PlayAsOneShot();
                didPlay = true;
                break;
            case PlayConditions.InterruptAndPlay:
                StopAllOneShots(); // Stop all OneShot audios if playing
                emitter.Play(); // FMOD by default interrupts the current sound if you call play on it again
                didPlay = true;
                break;
            case PlayConditions.PlayIfNotPlaying:
                if (!emitter.IsPlaying() && eventInstances.Count <= 0)
                {
                    emitter.Play();
                    didPlay = true;
                }
                break;
            default:
                Debug.LogError("ERROR! PLAY CONDITION NOT DEFINED IN SIMPLE AUDIO EMITTER!");
                return;
        }
        // Trigger the cooldown if we did play a sound and the cooldown time is set to a number bigger than 0
        if (didPlay && settings.playCooldown > 0)
        {
            cooldownRoutine = StartCoroutine(SoundCooldownRoutine());
        }
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
        EventInstance instance = RuntimeManager.CreateInstance(settings.eventReference);
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
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    // TODO: Finish this function if needed later, probably needs some more condition checks
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
        while (elapsedTime < settings.playCooldown)
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

}
