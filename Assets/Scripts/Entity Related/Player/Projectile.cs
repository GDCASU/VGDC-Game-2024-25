using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author: William Peng
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose: Projectile that is fired at wherever the player clicks on the screen
 * 
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>
public class Projectile : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

	// The target position for the projectile, set by PlayerController when this projectile is instantiated
	[HideInInspector] public Vector3 target;

	// Inspector modifiable values
	[SerializeField] private float speed = 1f;
	// The time in seconds this projectile can last
	[SerializeField] private float lifetime = 5f;

    // Private variables
    private Vector3 moveDir;

	private void Start()
	{
		// Calculate movement vector
		moveDir = Vector3.Normalize(target - transform.position);
	}

	// Update is called once per frame
	void Update()
    {
        if(lifetime > 0f)
        {
            // Move
            transform.Translate(moveDir * speed * Time.deltaTime);
            lifetime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
