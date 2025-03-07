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
 * Purpose:
 * Hold all the combat related definitions in one place
 * This file is not meant to contain monobehaviours, just things like enums or static classes
 */// --------------------------------------------------------

/// <summary>
/// Enum representing all the different elements of the game
/// </summary>
public enum Elements
{
    Undefined,
    Neutral,
    Fire,
    Water,
    Fungal,
    Sparks
}

/// <summary>
/// Enum defining all the possible statuses that can be inflicted on enemies
/// </summary>
public enum StatusEffect
{
    Undefined,
    Burning,
    Spored,
    Charged
}

/// <summary>
/// Enum that defines the reactions possbile from element combinations
/// </summary>
public enum ReactionType
{
    Undefined,
    Fireworks,
}

/// <summary>
/// Helper class to retrieve the status effect given an element or vice versa
/// </summary>
public static class ElementStatusEffectsCaster
{
    /// <summary>
    /// Returns the element that caused the status effect
    /// </summary>
    public static Elements GetElementFromStatusEffect(StatusEffect statusEffect)
    {
        switch (statusEffect)
        {
            case StatusEffect.Burning:
                return Elements.Fire;
            case StatusEffect.Spored:
                return Elements.Fungal;
            case StatusEffect.Charged:
                return Elements.Sparks;
            default:
                Debug.LogError("ERROR! STATUS EFFECT NOT DEFINED IN COMBAT DEFINITIONS");
                return Elements.Undefined;
        }
    }

    /// <summary>
    /// Returns the Status Effect that element causes
    /// </summary>
    public static StatusEffect GetStatusEffectFromElements(Elements element)
    {
        switch (element)
        {
            case Elements.Fire:
                return StatusEffect.Burning;
            case Elements.Fungal:
                return StatusEffect.Spored;
            case Elements.Sparks:
                return StatusEffect.Charged;
            default:
                Debug.LogError("ERROR! ELEMENT NOT DEFINED IN COMBAT DEFINITIONS");
                return StatusEffect.Undefined;
        }
    }
}

/// <summary>
/// Class to retrieve reactions given elements
/// </summary>
public static class ReactionDefinitions
{
    private static readonly Dictionary<(Elements, Elements), ReactionType> reactionDictionary = new()
    {
        // Fireworks
        { (Elements.Fire, Elements.Fungal), ReactionType.Fireworks },
        { (Elements.Fungal, Elements.Fire), ReactionType.Fireworks },
    };

    /// <summary>
    /// Function to try and see if the two elements provoke a reaction, returns undefined if not
    /// </summary>
    public static ReactionType TryGetReaction(Elements element1, Elements element2)
    {
        bool containsPair = reactionDictionary.ContainsKey((element1, element2));
        if (!containsPair) return ReactionType.Undefined; // Wasnt defined
        
        // Else it is defined, return it
        reactionDictionary.TryGetValue((element1, element2), out ReactionType value);
        return value;
    }

    /// <summary>
    /// Function to try and see if the two Status Effects provoke a reaction, returns undefined if not
    /// </summary>
    public static ReactionType TryGetReaction(StatusEffect statusEffect1, StatusEffect statusEffect2)
    {
        // Cast the status effects to elements
        Elements castedElement1 = ElementStatusEffectsCaster.GetElementFromStatusEffect(statusEffect1);
        Elements castedElement2 = ElementStatusEffectsCaster.GetElementFromStatusEffect(statusEffect2);
        // Use the already defined function to get the reaction
        return TryGetReaction(castedElement1, castedElement2);
    }
    
    /// <summary>
    /// Function to try and see if a Statu Effect and an element provoke a reaction, returns undefined if not
    /// </summary>
    public static ReactionType TryGetReaction(Elements element, StatusEffect statusEffect)
    {
        // Cast the status effects to elements
        Elements castedElement = ElementStatusEffectsCaster.GetElementFromStatusEffect(statusEffect);
        // Use the already defined function to get the reaction
        return TryGetReaction(element, castedElement);
    }
}











