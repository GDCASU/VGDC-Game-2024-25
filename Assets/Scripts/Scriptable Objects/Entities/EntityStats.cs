using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

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
    public int placeholder;
    [SerializeField] private DamageMultiplier[] damageMultipliers = new DamageMultiplier[]
    {
        new DamageMultiplier{ element = Element.Neutral, multiplier = 1f },
        new DamageMultiplier{ element = Element.Fungus, multiplier = 1f },
    };
    [HideInInspector]
    private Dictionary<Element, float> damageMultiplierDict;
    public Dictionary<Element, float> getDamageMultipliers()
    {
		// If the damage multiplier dict already exists, return it
		if(damageMultiplierDict != null)
		{
			return damageMultiplierDict;
		}

        damageMultiplierDict = new Dictionary<Element, float>();
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
    public Element element;
    public float multiplier;
}

