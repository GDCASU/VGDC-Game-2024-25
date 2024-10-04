using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author: William
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose: Holds enemy stats
 * 
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>
[CreateAssetMenu(fileName = "EntityStats", menuName = "ScriptableObjects/EntityStats")]
public class EntityStats : ScriptableObject
{
    [Header("Data")]
    [SerializeField] private DamageMultiplier[] damageMultipliers = new DamageMultiplier[]
    {
        new DamageMultiplier{ element = Elements.fire, multiplier = 1f },
        new DamageMultiplier{ element = Elements.neutral, multiplier = 1f },
    };
    [HideInInspector]
    private Dictionary<Elements, float> damageMultiplierDict;
    public Dictionary<Elements, float> getDamageMultipliers()
    {
		// If the damage multiplier dict already exists, return it
		if(damageMultiplierDict != null)
		{
			return damageMultiplierDict;
		}

        damageMultiplierDict = new Dictionary<Elements, float>();
		// Create damage multiplier dict based on values set in the Unity inspector
		foreach(DamageMultiplier dm in damageMultipliers)
		{
			damageMultiplierDict.Add(dm.element, dm.multiplier);
		}
		return damageMultiplierDict;
	}
}

[Serializable]
public struct DamageMultiplier
{
    public Elements element;
    public float multiplier;
}

