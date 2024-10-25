using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

/* -----------------------------------------------------------
 * Author:
 * Sameer Reza
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Audio manager to work as a singleton intermediary between the game and FMOD for audio, sound control (eg:volume) handled by SoundManager
 */// --------------------------------------------------------
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set;}

    private List<EventInstance> eventInstances = new List<EventInstance>(); //used to stop all events when switching scenes

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("AudioManager already exists, destroying duplicate");
            Destroy(gameObject);
        }
        eventInstances = new List<EventInstance>();
    }

    // Play a one shot sound at a given position, one shot sounds are short lived and dont loop
    public void PlayOneShot(EventReference sound, Vector3 position)
    {
        RuntimeManager.PlayOneShot(sound, position);
    }

    public EventInstance CreateEventInstance(EventReference sound)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(sound);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    private void cleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDestroy()
    {
        cleanUp();
    }
}
