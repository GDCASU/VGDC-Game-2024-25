using System;
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
    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private Elements element;
    [SerializeField] private float damage = 1f;
    [SerializeField] private EnemyStatusEffect status; // The status effect this projectile inflicts
	
	// Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local variables
    [HideInInspector] public Vector3 moveDir = Vector3.zero;

    private void Start()
    {
	    // Destroy after lifetime passes
	    Destroy(gameObject, lifetime);
    }
    
	private void Update()
    {
		// Move
		transform.Translate(moveDir * (speed * Time.deltaTime));
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
