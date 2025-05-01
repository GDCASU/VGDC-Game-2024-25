using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
* Author:
* Cami
* 
* Modified By:
*/// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Connecting script to wiffleNPC to trigger actions when player 
 * gets close to a trigger collider box
*/// --------------------------------------------------------

public class StartWiffleAction : Interactable
{
    [SerializeField] WiffleNPC wiffle;
    [SerializeField] Transform startLocation;
    [SerializeField] Transform stopLocation;
    [SerializeField] Transform dropLocation;
    public int index; // the location index that the wiffle will travel between
    
    private void Start()
    {
        OnFocusEnter += PerformAction;
    }

    private void PerformAction()
    {
        wiffle.SwitchLocation(index, startLocation.position, stopLocation.position, dropLocation);
    }

    private void OnDestroy()
    {
        OnFocusEnter -= PerformAction;
    }
}
