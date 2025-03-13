using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    void Start()
    {
        // Set the fields on load
        fullscreenToggle.isOn = Screen.fullScreen;
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
        // TODO: Change brightness through post processing
    }
}
