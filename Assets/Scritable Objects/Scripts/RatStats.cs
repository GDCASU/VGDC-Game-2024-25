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
 * To allow design easy editing of the rat stats
 */// --------------------------------------------------------


/// <summary>
/// Class that handles the data settings of rats
/// </summary>
[CreateAssetMenu(fileName = "RatStats", menuName = "ScriptableObjects/Entities/RatStats")]
public class RatStats : ScriptableEntity
{
    [Header("Settings")] 
    public float minIdleTime;
    public float maxIdleTime;
    public float minRunTime;
    public float maxRunTime;
    public float maxAttackHeight;
}
