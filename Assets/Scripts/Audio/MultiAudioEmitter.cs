using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Allows playing from a list of sounds without having to create
 * a SimpleAudioEmitter for each one of them
 */// --------------------------------------------------------


/// <summary>
/// An audio emitter capable of playing sounds from a library using only one script
/// </summary>
public class MultiAudioEmitter : MonoBehaviour
{
    [Header("Sound Definitions")] 
    // WARNING: The multi audio emitter is not designed to be edited at runtime. Any changes done will probably not update
    [SerializeField] private SerializedDictionary<string, AudioEmitterSettings> soundDictionary;
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local Fields
    private Dictionary<int, SimpleAudioEmitter> soundEmitterDictionary = new();
    private GameObject emitterContainer;

    void Awake()
    {
        // Create the hashed dictionary from the string dictionary
        SerializedDictionary<int, AudioEmitterSettings> hashedSettingsDictionary = new();
        foreach (string soundString in soundDictionary.Keys)
        {
            soundDictionary.TryGetValue(soundString, out AudioEmitterSettings settings);
            int hashedKey = AudioEmitterTools.StringToInteger(soundString);
            hashedSettingsDictionary.Add(hashedKey, settings);
        }
        // Now create an emitter container
        emitterContainer = new GameObject("Audio Emitter Container");
        emitterContainer.transform.SetParent(this.transform, false);
        // Create a simple emitter for each of the sounds to handle their playback and add them to container
        foreach (int soundID in hashedSettingsDictionary.Keys)
        {
            hashedSettingsDictionary.TryGetValue(soundID, out AudioEmitterSettings settings);
            SimpleAudioEmitter emitter = emitterContainer.AddComponent<SimpleAudioEmitter>();
            emitter.settings = settings;
            soundEmitterDictionary.Add(soundID, emitter);
        }
    }

    /// <summary>
    /// <para>Play the specified sound that is paired with the passed string.</para>
    /// <para>DON'T USE THIS FUNCTION UNLESS DEBUGGING! String modification is very inefficient.</para>
    /// <para>Hash the audio string with AudioEmitterTools.StringToInteger(string) and then
    /// use the integer in the argument. Store the equivalent integer in a field and use that from then on.</para>
    /// </summary>
    /// <param name="soundString">The name of the sound to play set in the inspector. Case Sensitive</param>
    public void PlaySound(string soundString)
    {
        string warningMessage = "WARNING! Specifying the sound to play using a string is bad!\n";
        warningMessage += "String comparisons are expensive to make, so please ";
        warningMessage += "hash your sound and use the integer instead!\n";
        warningMessage += "Use AudioEmitterTools.StringToInteger(\"sound name here\") to hash it.";
        Debug.LogWarning(warningMessage);
        
        // Parameter Checks
        if (soundString == null)
        {
            Debug.LogError("ERROR! You passed a null string to the argument!");
            return;
        }
        // Trim whitespaces off
        soundString = soundString.Trim();
        if (soundString == "")
        {
            Debug.LogError("ERROR! You passed an empty string to the argument!");
            return;
        }
        // All good, Hash the string then call PlaySound with it
        int soundID = AudioEmitterTools.StringToInteger(soundString);
        PlaySound(soundID);
    }
    
    /// <summary>
    /// Play the specified sound that is paired with the passed integer.
    /// </summary>
    /// <param name="soundID">The integer ID of the target sound</param>
    public void PlaySound(int soundID)
    {
        bool doesValueExist = soundEmitterDictionary.TryGetValue(soundID, out SimpleAudioEmitter emitter);
        if (!doesValueExist)
        {
            // Did not find value in dictionary
            string msg = "ERROR! DID NOT FIND THE SPECIFIED SOUND ID!\n";
            msg += "Are you sure you hashed the right string? Remember that this is Case Sensitive";
            Debug.LogError(msg);
            return;
        }
        // we did find it, play the sound
        emitter.PlaySound();
    }
}