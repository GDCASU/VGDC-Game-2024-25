using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * Sameer Reza
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Manage everything related to sound in the game
 */// --------------------------------------------------------

// VCA Enums, necessary if we want outsider scripts modifying the VCA volumes
public enum SoundControllers
{
    Master,
    Music,
    SFX,
}

/// <summary>
/// Enum used to stop specific groups of sounds, determined on the FMOD Mixer tab
/// </summary>
public enum SoundGroups
{
    Master,
    Music,
    SFX,
    Combat
}

/// <summary>
/// Manages the sound of the game
/// </summary>
public class SoundManager : MonoBehaviour
{
    // Singleton
    public static SoundManager Instance;

    #region VCA

    // VCAs:
    // The string must contain the name assigned to the bus on the FMOD Mixer.
    // This is useful if someone decides to change how they are named without re-scripting everything
    [Header("VCA Names")]
    [SerializeField] private string _masterVCAString;
    [SerializeField] private string _musicVCAString;
    [SerializeField] private string _sfxVCAString;
    // FMOD VCAs
    private FMOD.Studio.VCA _masterVCA;
    private FMOD.Studio.VCA _musicVCA;
    private FMOD.Studio.VCA _sfxVCA;
    // VCA Dictionary
    private Dictionary<SoundControllers, FMOD.Studio.VCA> _VCADictionary = new();

    #endregion

    #region Sound Groups

    // Groups allows us to do apply changes to a group of sound effects if playing
    [Header("Group Bus Paths")]
    [SerializeField] private string _sfxGroupPath;
    [SerializeField] private string _musicGroupPath;
    [SerializeField] private string _combatGroupPath;

    // Group variables
    private FMOD.Studio.Bus _sfxGroup;
    private FMOD.Studio.Bus _musicGroup;
    private FMOD.Studio.Bus _masterGroup;
    private FMOD.Studio.Bus _combatGroup;
    // Group Dictionary
    private Dictionary<SoundGroups, FMOD.Studio.Bus> _soundGroupDictionary = new();

    #endregion

    // Volume Inspector Slider
    [Header("Volume Sliders")]
    [Range(0f, 1f)][SerializeField] private float _masterSlider;
    [Range(0f, 1f)][SerializeField] private float _musicSlider;
    [Range(0f, 1f)][SerializeField] private float _sfxSlider;

    // Settings
    [Header("Settings")]
    [SerializeField] private bool _disableSliders;

    // Debugging
    [Header("Debugging")]
    [SerializeField] private bool _doDebugLog;

    // Audio Events
    public static event System.Action OnGamePaused;
    public static event System.Action OnGameResumed;
    
    // Local variables
    private float _currentMasterVolumeVal;
    private float _currentSFXVolumeValue;
    private float _currentMusicVolumeValue;
    private float _previousMasterVolume;
    private float _previousMusicVolume;
    private float _previousSFXVolume;

    private void Awake()
    {
        // Handle Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Set up variables
        SetupVCA();
        SetupGroups();

        // Setup the previous volume values for checking
        _masterVCA.getVolume(out float masterVolumeVal);
        _musicVCA.getVolume(out float musicVolumeVal);
        _sfxVCA.getVolume(out float sfxVolumeVal);
        _previousMasterVolume = masterVolumeVal;
        _previousMusicVolume = musicVolumeVal;
        _previousSFXVolume = sfxVolumeVal;
    }

    /// <summary>
    /// Sets up all variables related to VCAs
    /// </summary>
    private void SetupVCA()
    {
        // Fecth the matching VCAs
        _masterVCA = FMODUnity.RuntimeManager.GetVCA("vca:/" + _masterVCAString);
        _musicVCA = FMODUnity.RuntimeManager.GetVCA("vca:/" + _musicVCAString);
        _sfxVCA = FMODUnity.RuntimeManager.GetVCA("vca:/" + _sfxVCAString);
        // Populate VCA Dictionary
        _VCADictionary.Add(SoundControllers.Master, _masterVCA);
        _VCADictionary.Add(SoundControllers.Music, _musicVCA);
        _VCADictionary.Add(SoundControllers.SFX, _sfxVCA);
    }

    /// <summary>
    /// Sets up all variables related to Sound Groups
    /// </summary>
    private void SetupGroups()
    {
        // Fetch group buses
        _sfxGroup = FMODUnity.RuntimeManager.GetBus("bus:/" + _sfxGroupPath);
        _musicGroup = FMODUnity.RuntimeManager.GetBus("bus:/" + _musicGroupPath);
        _masterGroup = FMODUnity.RuntimeManager.GetBus("bus:/");
        _combatGroup = FMODUnity.RuntimeManager.GetBus("bus:/" + _combatGroupPath);
        // Populate Group Dictionary
        _soundGroupDictionary.Add(SoundGroups.SFX, _sfxGroup);
        _soundGroupDictionary.Add(SoundGroups.Music, _musicGroup);
        _soundGroupDictionary.Add(SoundGroups.Master, _masterGroup);
        _soundGroupDictionary.Add(SoundGroups.Combat, _combatGroup);
    }

    // Debugging
    private void Update()
    {
        
    }

    // Loop for inspector sliders, can be removed once UI can manage this
    private void LateUpdate()
    {
        // Get the current volumes of their VCA's
        _masterVCA.getVolume(out float currMasterVolume);
        _musicVCA.getVolume(out float currMusicVolume);
        _sfxVCA.getVolume(out float currSFXVolume);
        // if disabled, lock the values of the sliders and update them
        if (_disableSliders)
        {
            _masterSlider = currMasterVolume;
            _musicSlider = currMusicVolume;
            _sfxSlider = currSFXVolume;
            _previousMasterVolume = currMasterVolume;
            _previousMusicVolume = currMusicVolume;
            _previousSFXVolume = currSFXVolume;
            return;
        }

        // TWO VITAL CHECKS

        // Check if the volume value has been changed from outside the script
        // Update the sliders to the new value
        if (_previousMasterVolume != currMasterVolume)
        {
            _masterSlider = currMasterVolume;
        }
        if (_previousMusicVolume != currMusicVolume)
        {
            _musicSlider = currMusicVolume;
        }
        if (_previousSFXVolume != currSFXVolume)
        {
            _sfxSlider = currSFXVolume;
        }

        // Check if the volume sliders in this script have been changed in the inspector
        // If so, update the values of the volumes
        if (_masterSlider != currMasterVolume)
        {
            SetVolume(SoundControllers.Master, _masterSlider, 1);
        }
        if (_musicSlider != currMusicVolume)
        {
            SetVolume(SoundControllers.Music, _musicSlider, 1);
        }
        if (_sfxSlider != currSFXVolume)
        {
            SetVolume(SoundControllers.SFX, _sfxSlider, 1);
        }

        // Set the previous values to the current ones
        _previousMasterVolume = currMasterVolume;
        _previousMusicVolume = currMusicVolume;
        _previousSFXVolume = currSFXVolume;
    }

    #region Audio Management Functions

    /// <summary>
    /// <para> Sets the volume of the controller. Where maxSliderVal is the maximum value of your slider. </para> 
    /// <para> if the range is [0,1]. Then [1 = Full volume] [0.5 = Half Volume] [0 = Silent] </para> 
    /// </summary>
    public void SetVolume(SoundControllers targetVCA, float volume, float maxSliderVal)
    {
        // Fetch the matching VCA 
        bool valueFound = _VCADictionary.TryGetValue(targetVCA, out FMOD.Studio.VCA obtainedVCA);

        // Check if the key did get a value
        if (!valueFound) 
        {
            string msg = "<color=red>ERROR! Target VCA Specified does not exist within dictionary!</color>\n";
            msg += "Error thrown by calling SoundManager.instance.SetVolume";
            Debug.LogError(msg);
            return;
        }

        // Normalize value if the range is bigger than [0,1]
        float scaledVolume = volume / maxSliderVal;
        
        // WARNING: Range must be within [0,1], throw a warning message in case the range is outside
        if (scaledVolume < 0 || scaledVolume > 1)
        {
            string msg1 = "<color=red>WARNING! A SCRIPT TRIED TO SET THE VOLUME TO A VALUE OUTSIDE OF RANGE [0,1]!</color>\n";
            msg1 += "Please check your input values used for calling SoundManager.instance.SetVolume\n";
            msg1 += "After normalizing, You tried to set it to value: <color=yellow>" + scaledVolume + "</color>";
            Debug.LogError(msg1);
            return;
        }

        // VCA Found, set the value of current volume for serialization
        switch (targetVCA)
        {
            case SoundControllers.Music:
                _currentMusicVolumeValue = volume;
                break;
            case SoundControllers.SFX:
                _currentSFXVolumeValue = volume;
                break;
            case SoundControllers.Master:
                _currentMasterVolumeVal = volume;
                break;
        }

        // Set the volume of the specified bus
        obtainedVCA.setVolume(scaledVolume);
    }

    /// <summary> Stop all sounds playing </summary>
    public void StopAllSounds(FMOD.Studio.STOP_MODE mode) => _masterGroup.stopAllEvents(mode);

    /// <summary>
    /// Pauses ALL sounds in the game, use carefully and sparingly
    /// </summary>
    public void PauseAllSounds() => FMODUnity.RuntimeManager.PauseAllEvents(true);

    /// <summary>
    /// Resume's all events that are paused
    /// </summary>
    public void ResumeAllSounds() => FMODUnity.RuntimeManager.PauseAllEvents(false);

    /// <summary>
    /// Stops all sounds under the specified category
    /// </summary>
    public void StopSoundGroup(SoundGroups targetGroup, FMOD.Studio.STOP_MODE mode)
    {
        // Fetch the matching group
        bool valueFound = _soundGroupDictionary.TryGetValue(targetGroup, out FMOD.Studio.Bus obtainedGroup);

        // Check if the key did get a value
        if (!valueFound)
        {
            // Not found, log error
            string msg = "<color=red>ERROR! Target Group Specified does not exist within dictionary!</color>\n";
            msg += "Error thrown by calling SoundManager.instance.StopSoundGroup";
            Debug.LogError(msg);
            return;
        }
        // Else, group was found, stop all sounds in it
        obtainedGroup.stopAllEvents(mode);
    }

    /// <summary>
    /// Pauses all sounds under the specified category
    /// </summary>
    public void PauseSoundGroup(SoundGroups targetGroup)
    {
        // Fetch the matching group
        bool valueFound = _soundGroupDictionary.TryGetValue(targetGroup, out FMOD.Studio.Bus obtainedGroup);

        // Check if the key did get a value
        if (!valueFound)
        {
            // Not found, log error
            string msg = "<color=red>ERROR! Target Group Specified does not exist within dictionary!</color>\n";
            msg += "Error thrown by calling SoundManager.instance.PauseSoundGroup";
            Debug.LogError(msg);
            return;
        }
        // Else, group was found, pause all sounds in it
        obtainedGroup.setPaused(true);
    }

    /// <summary>
    /// Resumes all sounds under the specified category
    /// </summary>
    public void ResumeSoundGroup(SoundGroups targetGroup)
    {
        // Fetch the matching group
        bool valueFound = _soundGroupDictionary.TryGetValue(targetGroup, out FMOD.Studio.Bus obtainedGroup);

        // Check if the key did get a value
        if (!valueFound)
        {
            // Not found, log error
            string msg = "<color=red>ERROR! Target Group Specified does not exist within dictionary!</color>\n";
            msg += "Error thrown by calling SoundManager.instance.ResumeSoundGroup";
            Debug.LogError(msg);
            return;
        }
        // Else, group was found, resume all sounds in it
        obtainedGroup.setPaused(false);
    }

    #endregion

    #region Getters and Setters

    public float GetVCAVolume(SoundControllers targetVCA)
    {
        float volume = 0f;
        switch (targetVCA)
        {
            case SoundControllers.Master:
                _masterVCA.getVolume(out volume);
                return volume;
            case SoundControllers.Music:
                _musicVCA.getVolume(out volume);
                return volume;
            case SoundControllers.SFX:
                _sfxVCA.getVolume(out volume);
                return volume;
            default:
                Debug.LogError("ERROR! Target VCA Specified does not exist within dictionary!");
                return volume;
        }
    }

    #endregion
    
    // NOT IMPLEMENTED YET ******************************

    // TODO: Implement bank loading/de-loading for better memory?
    //public void LoadBank() { throw new System.NotImplementedException(); }
    //public void UnloadBank() { throw new System.NotImplementedException(); }
    
    

}
