using System;
using UnityEngine;
using FMODUnity;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Store the settings of audio emitters on its own so they dont
 * require to inherit from monobehaviour, since the makes multi
 * audio emitter easier to implement.
 */// --------------------------------------------------------


/// <summary>
/// Class that stores the settings to play an audio
/// </summary>
[Serializable]
public class AudioEmitterSettings
{
    [Header("Sound Reference")] 
    public EventReference eventReference;
    
    [Header("Triggers")]
    public EmitterGameEvent eventPlayTrigger = EmitterGameEvent.None;
    public EmitterGameEvent eventStopTrigger = EmitterGameEvent.None;
    
    [Header("Settings")]
    public PlayConditions playConditions = PlayConditions.PlayAlways;
    [Range(0f,10f)] public float playCooldown = 0f;
    /// <summary>
    /// Helper enum to set conditions on playback
    /// </summary>
    public enum PlayConditions
    {
        PlayAlways, // Will play the sound no matter what
        PlayIfNotPlaying, // Will only play the sound if not already playing
        InterruptAndPlay, // Will interrupt the sound if already playing and play again
    }
    
    // TODO: Would making a fadestop be annoying or...?
    /// <summary>
    /// Helper enum to set up how the sound should be stopped
    /// </summary>
    public enum StopStyle
    {
        HardStop, // Stops the sound immediatly
        FadeStop, // Fades Stops the sound
    }
}

/// <summary>
/// Class meant to hold tools for the audio emitters
/// </summary>
public static class AudioEmitterTools
{
    /// <summary>
    /// Function that returns the integer hash equivalent of the string passed to it. Case Sensitive
    /// </summary>
    /// <returns> an integer representation of the string. Returns 0 if an error is found</returns>
    public static int StringToInteger(string soundName)
    {
        // Parameter Checks
        if (soundName == null)
        {
            Debug.LogError("ERROR! You passed a null string to the argument!");
            return 0;
        }
        // Trim whitespaces off
        soundName = soundName.Trim();
        if (soundName == "")
        {
            Debug.LogError("ERROR! You passed an empty string to the argument!");
            return 0;
        }
        // All good, Hash the string to int
        // IAN NOTE: I'm going to make use of the fact that the animator already
        // has a tool like this, since creating my own hashing system would take a while
        return Animator.StringToHash(soundName);
    }
}