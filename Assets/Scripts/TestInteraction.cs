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
    [SerializeField] bool isWalking; // placeholder bool to change functions

    // Start is called before the first frame update
    void Start()
    {
        if (isWalking)
        {
            interactions.ChangeInteraction(Run); // Makes it so the function Run will be called with the interaction key
        }
        else { interactions.ChangeInteraction(Walk); }
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
}
