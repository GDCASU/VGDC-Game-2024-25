using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
* Author:
* Chandler Van
* 
* Modified By:
*/// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Offer a simple framework to build NPCs off of
*/// --------------------------------------------------------

public class NPC : MonoBehaviour
{
    [Header("Drops")]
    public List<GameObject> drops;
    [SerializeField] private Transform dropPoint;

    /// <summary>
    /// Calls the NPC to drop its items (overridable)
    /// </summary>
    public virtual void DropItems()
    {
        foreach(GameObject drop in drops)
        {
            Instantiate(drop, dropPoint.position, Quaternion.identity);
        }
    }
}
