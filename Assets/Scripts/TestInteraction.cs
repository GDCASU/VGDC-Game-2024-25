using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* -----------------------------------------------------------
 * Author:
 * Cami Lee
 * 
 * Modified By:
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

    // Start is called before the first frame update
    void Start()
    {
        interactions.ChangeInteraction(Run); // Makes it so the function Run will be called with the interaction key
    }

    /// <summary>
    /// Sample Function
    /// </summary>
    public void Run()
    {
        Debug.Log("is running");
    }
}
