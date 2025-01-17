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
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Displays a health bar which can be used for any assets.
 */// --------------------------------------------------------

public class FloatingHealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Camera camera;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    CanvasGroup canvasGroup;

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if (slider.value <= 0 || slider.value == slider.maxValue) { canvasGroup.alpha = 0; }
        else { canvasGroup.alpha = 1; }
        transform.rotation = camera.transform.rotation;
        transform.position = target.position + offset;
    }
}
