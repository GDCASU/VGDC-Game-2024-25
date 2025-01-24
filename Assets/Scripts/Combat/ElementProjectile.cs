using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
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
public class ElementProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private Elements element;
    
	// Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local variables
    [HideInInspector] public string ownerTag = "";
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
        
        // Dont collide if on the same tag
        if (other.CompareTag(ownerTag)) return; // Same tag, dont damage owner
        
        // Try to damage the other object
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable == null)
        {
            // Didnt contain the interface
            Destroy(gameObject);
            return;
        }
        // Otherwise it did, deal damage and see if we need to generate a reaction
        ReactionType reaction = damageable.TakeDamage(damage,element);
        if (reaction == ReactionType.Undefined)
        {
            // No reaction to process
            Destroy(gameObject);
            return;
        }
        
        // There is a reaction do perform
        // TODO: GENERATE REACTION IN THE WORLD
        switch (reaction)
        {
            case ReactionType.Fireworks:
                //
                break;
        }
        
        // Destroy projectile
        Destroy(gameObject);
	}
}
