using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Handles the graphic menu on the pause and main menu
 */// --------------------------------------------------------


/// <summary>
/// Class that handles a graphics menu
/// </summary>
public class GraphicsMenuManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TextMeshProUGUI brightnessValueText;
    [SerializeField] private TextMeshProUGUI brightnessShadowText;
    [SerializeField] private Volume postProcessingVolume;
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local variables
    private float brightnessMiddlePoint = 0f; // Middle point will set the postExposure to zero
    private ColorAdjustments colorAdjustments;
    
    void Start()
    {
        // Set the fields on load
        fullscreenToggle.isOn = Screen.fullScreen;
        
        // Get the middle point of the brightness slider
        brightnessMiddlePoint = Mathf.Round(brightnessSlider.maxValue / 2f);
        
        // FIXME: Unity apparently cant update the profile in real time, not working rn
        /*
        // Get Color Adjustments from Post-Processing Volume
        if (postProcessingVolume.profile.TryGet(out colorAdjustments))
        {
            // Load saved brightness or default to middle point
            float savedBrightness = brightnessMiddlePoint; // TODO: Get brightness stored in data later on
            brightnessSlider.value = savedBrightness; // Convert range (-10 to 10) → (0 to 20)
            SetVolumeBrightness(brightnessSlider.value);
        }
        else
        {
            Debug.LogError("Color Adjustments effect not found in the Post-Processing Volume!");
        }
        */
    }
    
    /// <summary>
    /// Function that changes the status of the fullscreen
    /// </summary>
    public void SetFullscreen()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
    }
    
    /// <summary>
    /// Function that changes the brightness of the screen
    /// </summary>
    public void ChangeBrightness()
    {
        float value = brightnessSlider.value;
        string valueStr = value.ToString("F0");
        brightnessValueText.text = valueStr;
        brightnessShadowText.text = valueStr;
        SetVolumeBrightness(value);
    }
    
    /// <summary>
    /// Function to change the brightenss of the screen through the volume post processing effects
    /// </summary>
    /// <param name="value"></param>
    private void SetVolumeBrightness(float value)
    {
        // FIXME: Unity apparently cant update the profile in real time, not working rn
        if (colorAdjustments != null)
        {
            float brightnessValue = (value - brightnessMiddlePoint);
            colorAdjustments.postExposure.value = brightnessValue;
            // TODO: Save settings
        }
    }
}
