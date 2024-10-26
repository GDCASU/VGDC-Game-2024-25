using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

/* -----------------------------------------------------------
 * Author:
 * Sameer Reza
 * 
 * Modified By: 
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Play a one shot sound when the player collides with the object, used to test the FMOD system, also uses the event emitter system
 */// --------------------------------------------------------

[RequireComponent(typeof(StudioEventEmitter))] //need to have something to emit with
public class oneShotOnTriggerTest : MonoBehaviour
{
    private StudioEventEmitter _emitter;

    private void Start()
    {
        _emitter = AudioManager.Instance.CreateEventEmitter(FMODEvents.instance.coinCollect, this.gameObject);
        _emitter.Play();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _emitter.Stop();
            AudioManager.Instance.PlayOneShot(FMODEvents.instance.testSFX, transform.position);
        }
    }
}
