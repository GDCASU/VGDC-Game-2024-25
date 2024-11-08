using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;

/* -----------------------------------------------------------
 * Author: William Peng
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose: 
 * 
 */// --------------------------------------------------------


/// <summary>
/// Projectile that is fired at wherever the player clicks on the screen. Contains element type and damage
/// </summary>
public class Projectile : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

	// The target position for the projectile, set by PlayerController when this projectile is instantiated
	[HideInInspector] public Vector3 target;

	// Inspector modifiable values
	[SerializeField] private float speed = 10f;
	// The time in seconds this projectile can last, set to 10s as a hard cap
	[SerializeField] private float lifetime = 10f;
	// The element of this projectile
	[SerializeField] private Elements element = Elements.neutral;
	// The status effect this projectile inflicts
	[SerializeField] private EnemyStatusEffect status = EnemyStatusEffect.normal;
	// The amount of damage this projectile does
	[SerializeField] private float damage = 1f;

	// Private variables
	private Vector3 moveDir;

	private void OnEnable()
	{
        // Calculate movement vector
        moveDir = Vector3.Normalize(target - transform.position);
		Destroy(gameObject, lifetime);
	}

	private void Update()
    {
		// Move
		transform.Translate(moveDir * speed * Time.deltaTime);
	}

	private void OnTriggerEnter(Collider other)
	{
		if(doDebugLog) Debug.Log(gameObject.name + " hit " + other.gameObject.name);

		other.GetComponent<DamageableEntity>()?.TakeDamage(damage, element, status);

		// Destroy upon collision with terrain
		if(other.gameObject.layer == LayerMask.NameToLayer("Terrain"))
		{
			if(doDebugLog) Debug.Log(gameObject.name + " hit terrain, destroying self");
			Destroy(gameObject);
		}
	}
}
