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
 * Chandler Van
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
    
    [Header("For Water Attack")]
    [SerializeField] private HazardTile waterTile;
    [SerializeField] private int waterRadius;
    [SerializeField] private float verticalSpeed;
    [SerializeField] private int numBounces;
    
	// Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    [Header("Readouts")]
    [InspectorReadOnly] private int currBounces = 0;
    
    // Local variables
    [HideInInspector] public string ownerTag = "";
    [HideInInspector] public Vector3 moveDir = Vector3.zero;
    

    private void Start()
    {
	    // Destroy after lifetime passes
	    Destroy(gameObject, lifetime);
        
        // Move differently if water
        if (element == Elements.Water)
        {
            StartCoroutine(WaterTrajectory());
        }
        else
        {
            StartCoroutine(NormalTrajectory());
        }
        
    }


    /// <summary>
    /// The regular trajectory of projectiles
    /// </summary>
    /// <returns></returns>
    private IEnumerator NormalTrajectory()
    {
        while (true)
        {
            transform.Translate(moveDir * (speed * Time.deltaTime));
            yield return null;
        }
    }

    private IEnumerator WaterTrajectory()
    {
        Vector3 startPos = transform.position;
        Vector3 groundPos = GameGridManager.Instance.transform.position;
        bool movingDown = true;
        while (true)
        {
            // Move foward
            transform.Translate(moveDir * (speed * Time.deltaTime));
            if (movingDown)
            {
                // Move down
                transform.Translate(Vector3.down * (verticalSpeed * Time.deltaTime));
            }
            else
            {
                // Move up
                transform.Translate(Vector3.up * (verticalSpeed * Time.deltaTime));
            }
            
            // Check for ground
            if (transform.position.y < groundPos.y && movingDown)
            {
                // Collided with ground, create water and move up
                GameGridManager.Instance.PlaceHazardTiles(waterRadius, transform.position, waterTile);
                currBounces++;
                movingDown = false;
            }
            // Check for ceiling
            if (transform.position.y > startPos.y)
            {
                // Reached height of movement
                movingDown = true;
            }

            if (currBounces >= numBounces)
            {
                Destroy(gameObject);
                yield break;
            }
            yield return null;
        }
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
            // Other Gameobject was not Damageable

            // Element Specific Behaviours
            switch (element)
            {
                // This is just here for possible future changes
                default:
                    break;
            }

            // Might move this into the switch later in possible future changes
            Destroy(gameObject);
            return;
        }
        // Otherwise it did, deal damage and see if we need to generate a reaction
        ReactionType reaction = damageable.TakeDamage(damage,element, transform.position);
        
        // There is a reaction do perform
        // TODO: GENERATE REACTION IN THE WORLD
        switch (reaction)
        {
            case ReactionType.Fireworks:
                //
                break;

            case ReactionType.Undefined:
                break;
        }

        // Element Specific End Behaviours
        switch (element)
        {
            case Elements.Fire:
                break;

            default:
                Destroy(gameObject);
                break;
        }
    }
}
