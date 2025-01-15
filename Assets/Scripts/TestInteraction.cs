using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/* -----------------------------------------------------------
 * Author:
 * Cami Lee
 * 
 * Modified By: Eliza Chook
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
    [SerializeField] bool rangeOfObj; // placeholder bool to change functions

    // Start is called before the first frame update
    void Start()
    {
        if (rangeOfObj)
        {
            interactions.ChangeInteraction(OutOfRange); // Makes it so the function OutOfRange will be called with the interaction key
        }
        else { interactions.ChangeInteraction(InRange); }
    }

    /// <summary> Sample Functions </summary>
    public void OutOfRange()
    {
        Debug.Log("is out of range");
    }

    public void InRange()
    {
        Debug.Log("is in range");
        Destroy(gameObject);//destroy object (move to inventory) when player uses repective inputKey ('E' for keyboard, 'A' for console)

        //insert move to inventory here
    }

}
