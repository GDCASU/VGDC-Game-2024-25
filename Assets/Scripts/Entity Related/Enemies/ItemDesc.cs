using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Davyd Yehudin
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Give description to items
 */// --------------------------------------------------------


/// <summary>
/// Just contains a bunch of variables for the name, quantity, and other stuff my favourite team - design - comes up with for dropped items
/// </summary>
public class ItemDesc : MonoBehaviour
{
    [SerializeField] string Name;
    [SerializeField]int quantity;
    [SerializeField]int otherStuff;
}
