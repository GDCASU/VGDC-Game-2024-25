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
 * Play a one shot sound when the player collides with the object, used to test the FMOD system
 */// --------------------------------------------------------

public class oneShotOnCollisionTest : MonoBehaviour
{


    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with " + collision.gameObject.name);
        if (collision.gameObject.name == "Player")
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.instance.testSFX, transform.position);
        }
    }
}
