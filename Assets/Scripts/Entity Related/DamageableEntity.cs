using AssetUsageDetectorNamespace;
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
 * Purpose: Provide a system for any entity to detect damage instances and respond to its element
 * 
 */// --------------------------------------------------------


/// <summary>
/// Provides an event for whenever an elemental projectile hits the entity.
/// </summary>
public class DamageableEntity : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
	[SerializeField] private DamageMultiplier[] _damageMultipliers = new DamageMultiplier[]
	{
		new DamageMultiplier{ element = Elements.neutral, multiplier = 1f },
		new DamageMultiplier{ element = Elements.fire, multiplier = 1f },
		new DamageMultiplier{ element = Elements.water, multiplier = 1f },
		new DamageMultiplier{ element = Elements.sparks, multiplier = 1f },
	};
	private Dictionary<Elements, float> _damageMultiplierDict;

	/// <summary>
	/// Invoked when the entity receives an instance of damage from an elemental projectile
	/// </summary>
	public event DamageEvent OnDamaged;
	/// <summary>
	/// 
	/// </summary>
	/// <param name="damage">The base damage of the projectile</param>
	/// <param name="multiplier">The damage multiplier this entity has for the element type</param>
	/// <param name="element">The type of element of the projectile</param>
    public delegate void DamageEvent(float damage, float multiplier, Elements element);

	private void Start()
	{
		_damageMultiplierDict = Utils.GetDamageMultipliers(_damageMultipliers);
	}

	private void OnTriggerEnter(Collider other)
	{
		ElementalProjectile elementalProjectile = other.GetComponent<ElementalProjectile>();
		if(elementalProjectile)
        {
            if(doDebugLog)
            {
                Debug.Log("Entity " + gameObject.name + " hit by " + other.gameObject.name);
            }
            if(OnDamaged != null)
            {
                OnDamaged.Invoke(elementalProjectile.damage, _damageMultiplierDict[elementalProjectile.type], elementalProjectile.type);
            }
        }
	}
}
