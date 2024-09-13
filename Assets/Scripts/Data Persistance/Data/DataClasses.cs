using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Classes to store data written to file
 */// --------------------------------------------------------

[System.Serializable]
public class GameData
{
    // Levels Unlocked

    // Levels Complete

    // Weapons

    // Abilities

    // Misc
    public bool testBool;

    /// <summary>
    /// Constructor that will be called when creating a new game
    /// </summary>
    public GameData()
    {
        // Bools are false by default, so no need to set them here
    }
}

[System.Serializable]
public class ConfigData
{
    // Sound
    public int masterVolumeValue;
    public int sfxVolumeValue;
    public int musicVolumeValue;

    // Cursor

    // Framerate

    // Gameplay

    // Graphics

    /// <summary>
    /// Constructor that will be called when creating a new game
    /// </summary>
    public ConfigData()
    {
        masterVolumeValue = 50;
        sfxVolumeValue = 50;
        musicVolumeValue = 50;
    }
}
