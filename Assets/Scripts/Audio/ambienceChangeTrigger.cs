using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Sameer Reza
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Use the trigger collider of the attached object to change the ambience parameter of the level
 */// --------------------------------------------------------

[RequireComponent(typeof(Collider))]
public class ambienceChangeTrigger : MonoBehaviour
{
    [Header("Parameter Change")]
    [SerializeField] private string parameterName; //set this to the name of the parameter in the FMOD event
    [SerializeField] private float parameterValue; //make sure this is in the valid range for the parameter


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance.setAmbienceParameter(parameterName, parameterValue);
        }
    }
}
