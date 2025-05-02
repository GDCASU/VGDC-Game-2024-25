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
 * Purpose:
 * Interface to damage entities
 */// --------------------------------------------------------


/// <summary>
/// The interface the entities implement to recieve damage
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Deal damage through interface. Returns a reaction if provoked
    /// so the projectile can do changes to the environment
    /// </summary>
    public abstract ReactionType TakeDamage(int damage, Elements element);

    public abstract ReactionType TakeDamage(int damage, Elements element, Vector3 position);
}
