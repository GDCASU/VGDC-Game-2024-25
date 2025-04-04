using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Handles the Gameplay Menu options in the main screen and on the pause menu
 */// --------------------------------------------------------


/// <summary>
/// Class that handles a Gameplay menu
/// </summary>
public class GameplayMenuManager : MonoBehaviour
{
    [Header("Gameplay")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private TextMeshProUGUI DPIValue;
    
    [Header("Optional")]
    [SerializeField] private TextMeshProUGUI DPIShadow;
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Start is called before the first frame update
    void Start()
    {
        // On load, set the loaded values into the fields
        // TODO: Get sensitivity value
    }
    
    /// <summary>
    /// Resets game progress
    /// </summary>
    public void ResetProgress()
    {
        // TODO: Finish this function
    }

    /// <summary>
    /// Changes the mouse sensitivity to the value of the slider
    /// </summary>
    public void ChangeSensitivity()
    {
        float sliderVal = sensitivitySlider.value;
        string sliderValStr = sliderVal.ToString("F0");
        if (DPIShadow != null) DPIShadow.text = sliderValStr;
        DPIValue.text = sliderValStr;
        // TODO: Change Sensitivity?
    }

 
}
