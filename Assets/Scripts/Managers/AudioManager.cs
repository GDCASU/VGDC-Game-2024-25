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
    private EventInstance ambienceEventInstance;
    private EventInstance musicEventInstance;
    private List<EventInstance> eventInstances; //used to stop all events when switching scenes
    private List<StudioEventEmitter> studioEventEmitters; //used to stop all emitters when switching scenes

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
        studioEventEmitters = new List<StudioEventEmitter>();
    }

    private void Start()
    {
        initializeAmbience(FMODEvents.instance.levelAmbience);
        initializeMusic(FMODEvents.instance.levelBGMEvent);
    }

    // Initialize the level ambience, which is assumed to be a single looping event
    private void initializeAmbience(EventReference ambience)
    {
        ambienceEventInstance = CreateEventInstance(ambience);
        ambienceEventInstance.start();
    }

    // Initialize the level music, which is assumed to be a single looping event for now
    // If we want more adaptive music (eg: different music for different areas of the level or when in combat) we will create a seperate manager for music
    private void initializeMusic(EventReference music)
    {
        musicEventInstance = CreateEventInstance(music);
        musicEventInstance.start();
    }

    // Set a parameter for the ambience
    public void setAmbienceParameter(string parameter, float value)
    {
        EventDescription eventDescription;
        FMOD.RESULT result = ambienceEventInstance.getDescription(out eventDescription);
        if (result == FMOD.RESULT.OK)
        {
            if (eventDescription.getParameterDescriptionByName(parameter, out PARAMETER_DESCRIPTION parameterDescription) == FMOD.RESULT.OK)
            {
                ambienceEventInstance.setParameterByName(parameter, value);
            }
            else
            {
                Debug.LogWarning($"Ambience parameter '{parameter}' not found. Error: {result}");
            }
        }
        else
        {
            Debug.LogWarning($"Failed to get event description. Error: {result}");
        }
    }

    public void setMusicTrack(string trackName)
    {
        // Guard against null references
        if (FMODEvents.instance == null || FMODEvents.instance.levelBGMTracks == null)
        {
            Debug.LogError("FMODEvents instance or levelBGMTracks is null");
            return;
        }

        // Guard against null or empty musicTracks list
        if (FMODEvents.instance.levelBGMTracks.musicTracks == null || FMODEvents.instance.levelBGMTracks.musicTracks.Count == 0)
        {
            Debug.LogError("Music tracks list is null or empty");
            return;
        }

        // Check if the trackName exists in the list
        if (!FMODEvents.instance.levelBGMTracks.musicTracks.Exists(track => track.trackName == trackName))
        {
            Debug.LogWarning($"Music track '{trackName}' not found in levelBGMTracks list");
            return;
        }

        int trackNumber = FMODEvents.instance.levelBGMTracks.musicTracks.FindIndex(track => track.trackName == trackName);
        musicEventInstance.setParameterByName("music_track", trackNumber);
    }

    // Play a one shot sound at a given position, one shot sounds are short lived and dont loop
    public void PlayOneShot(EventReference sound, Vector3 position)
    {
        RuntimeManager.PlayOneShot(sound, position);
    }

    // Create an event instance, used for looping sounds
    public EventInstance CreateEventInstance(EventReference sound)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(sound);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    // Play an event instance if it's not already playing
    public void PlayEventNoDuplicate(EventInstance eventInstance)
    {
        if (eventInstance.isValid())
        {
            PLAYBACK_STATE playbackState;
            eventInstance.getPlaybackState(out playbackState);
            if (playbackState == PLAYBACK_STATE.STOPPED)
            {
                eventInstance.start();
            }
        }
        else
        {
            Debug.LogWarning("Attempted to play an invalid event instance: " + eventInstance.getDescription(out EventDescription description));
        }
    }

    // Create an event emitter, used for spatial audio
    public StudioEventEmitter CreateEventEmitter(EventReference sound, GameObject emitterObject)
    {
        StudioEventEmitter emitter = emitterObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = sound;
        studioEventEmitters.Add(emitter);
        return emitter;
    }

    private void cleanUp()
    {
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        foreach (StudioEventEmitter emitter in studioEventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        cleanUp();
    }

}
