using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* -----------------------------------------------------------
 * Author: TJ (Yousuf)
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose: Allows the volume sliders to change the volume using SoundManager
 * 
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>
public class SoundSliderScript : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    [SerializeField] Slider volumeSlider;
    [SerializeField] SoundControllers sliderSoundGroup;
    [SerializeField] string volumePrefKey; // e.x. musicVolume, masterVolume, sfxVolume
    
    
    // Start is called before the first frame update
    // Checks if player has stored volume settings
    void Start()
    {
        if (!PlayerPrefs.HasKey(volumePrefKey))
        {
            PlayerPrefs.SetFloat(volumePrefKey, 1);
            Load();
        }
        else
        {
            Load();
        }
    }

    // Set to trigger when slider value changes
    public void VolumeChanged() 
    {
        SoundManager.Instance.SetVolume(sliderSoundGroup, volumeSlider.value, 1);
        Save();
    }

    // Loads volume settings
    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(volumePrefKey);
    }
    
    // Saves volume settings
    private void Save()
    {
        PlayerPrefs.SetFloat(volumePrefKey, volumeSlider.value);
    }
}
