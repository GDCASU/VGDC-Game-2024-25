using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/* -----------------------------------------------------------
 * Author:
 * Alexander Black
 * 
 * Modified By:
 * Ian Fletcher
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Displays a health bar which can be used for any assets.
 */// --------------------------------------------------------

public class FloatingHealthBar : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Slider slider;

    // Local variables
    private Image fillImage = null;

    private void Awake()
    {
        fillImage = slider.fillRect.GetComponent<Image>();
    }

    /// <summary>
    /// Function to call to set the health bar value
    /// </summary>
    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }

    /// <summary>
    /// Function to call to change the color of the health bar
    /// </summary>
    public void SetHealthBarColor(Color color)
    {
        fillImage.color = color;
    }

    public void ResetHealthBarColor()
    {
        fillImage.color = Color.white;
    }
}
