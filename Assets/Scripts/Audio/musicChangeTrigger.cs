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
 * Use the trigger collider of the attached object to change the music track for the levelcurrently playing
 */// --------------------------------------------------------
public class musicChangeTrigger : MonoBehaviour
{
    [Header("Music Track Change")]
    [SerializeField] private string trackName; //set this to the name of the track in level's LevelMusicData

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            AudioManager.Instance.setMusicTrack(trackName);
        }
    }
}
