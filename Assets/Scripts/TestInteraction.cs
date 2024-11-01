using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
/* -----------------------------------------------------------
 * Author:
 * Cami Lee
 * 
 * Modified By: Eliza
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * An example script for Interactions. You may use this as a 
 * template for incorperating actions with the Interactions 
 * script. 
 */// --------------------------------------------------------



public class TestInteraction : MonoBehaviour
{
    [SerializeField] Interactions interactions;
    [SerializeField] bool isWalking; // placeholder bool to change functions
    private float pickupRange = 1.57f; // Range within which the player can pick up the item

    private Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player")?.transform; // Assumes player has the "Player" tag

        if (isWalking)
        {
            interactions.ChangeInteraction(Run); // Makes it so the function Run will be called with the interaction key
            Run();

        }
        else
        {
            interactions.ChangeInteraction(Walk);
            Walk();
        }
    }

    /// <summary> Sample Functions </summary>

    public void Run()
    {
        Debug.Log("is running");
    }

    public void Walk()
    {
        Debug.Log("is walking");
    }
    void Update()
    {
        // Skip update if player is not assigned
        if (player == null) return;

        //find distance of player with object
        float distance = Vector3.Distance(player.position, transform.position);

        //if player is within range, have message appear above the object
        if (distance <= pickupRange)
        {
            // Pick up (destroy) object if player presses 'E' while within range
            if (Input.GetKeyDown(KeyCode.E))
            {
                Destroy(gameObject);
            }
        }
    }
}
