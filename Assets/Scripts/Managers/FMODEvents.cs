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
 * FMOD events to be used in the game, set up as a singleton so that other scripts can reference them and they can be changed in one place
 */// --------------------------------------------------------
public class FMODEvents : MonoBehaviour
{
    #region SFX

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootstepSFX { get; private set; }
    [field: SerializeField] public EventReference playerAttackSFX { get; private set; }

    [field: Header("Collectable SFX")]
    [field: SerializeField] public EventReference coinCollect { get; private set; }
    [field: SerializeField] public EventReference coinIdle { get; private set; }


    [field: Header("Test One Shot SFX")]
    [field: SerializeField] public EventReference testSFX { get; private set; }

    [field: Header("Test Level SFX")]
    [field: SerializeField] public EventReference levelBGMEvent { get; private set; }
    [field: SerializeField] public LevelMusicData levelBGMTracks { get; private set; }
    [field: SerializeField] public EventReference levelAmbience { get; private set; }

    #endregion

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("FMODEvents already exists, destroying duplicate");
            Destroy(gameObject);
        }
    }
}

