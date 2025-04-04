using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Dummy class to make hazard testing easier
 */// --------------------------------------------------------


/// <summary>
/// Class to be deleted, helps debug hazard system
/// </summary>
public class TempHazardCreator : MonoBehaviour
{
    [Header("Referneces")]
    public HazardTile hazardPrefab;
    
    [Header("Settings")]
    public int hazardRadius;
    
    [Header("Button")] 
    public bool doCreate = false;
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    private void Update()
    {
        if (doCreate)
        {
            GameGridManager.Instance.PlaceHazardTiles(hazardRadius, transform.position, hazardPrefab);
            
            doCreate = false;
        }
    }
}
