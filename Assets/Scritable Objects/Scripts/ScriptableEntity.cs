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
 * Compartmentalize stats to not make new scripts for every entity
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>
public class ScriptableEntity : ScriptableObject
{
    [Header("Stats")]
    public int health;
    public int damage;
    public float baseSpeed;
    public float attackTime;
    public float detectionAngle;
}
