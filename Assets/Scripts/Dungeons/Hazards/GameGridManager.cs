using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Handle the dictionary grid of the hazards in the game
 */// --------------------------------------------------------


/// <summary>
/// Class that handles the hazard grid of the game
/// </summary>
public class GameGridManager : MonoBehaviour
{
    // Singleton
    public static GameGridManager Instance;
    
    [Header("References")]
    public Tilemap tilemap;
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local Variables
    // Dictionary to track the hazards that have been placed as to avoid using raycasts or some other shit
    public Dictionary<Vector3Int, HazardTile> placedHazards = new Dictionary<Vector3Int, HazardTile>();

    private void Awake()
    {
        // Set the Singleton
        if (Instance != null && Instance != this)
        {
            // Already set, destroy this object
            Destroy(gameObject);
            return;
        }
        // Not set yet
        Instance = this;
    }

    void OnDestroy()
    {
        // Null singleton
        if (Instance == this) Instance = null;
    }

    /// <summary>
    /// Registers a hazard tile at its grid position.
    /// If a hazard already exists and is replaceable, removes it first.
    /// <para>Returns true if registration succeeds, false otherwise.</para>
    /// </summary>
    public bool RegisterHazard(HazardTile hazard)
    {
        Vector3Int cellPos = hazard.gridPos;
        if (placedHazards.TryGetValue(cellPos, out HazardTile existingHazard))
        {
            if (!existingHazard.isReplaceable) return false;
            // Else, Remove the replaceable tile.
            placedHazards.Remove(cellPos);
            Destroy(existingHazard.gameObject); 
        }
        placedHazards.Add(cellPos, hazard);
        return true;
    }
    
    /// <summary>
    /// De-registers a hazard tile at its grid position.
    /// Returns true if registration succeeds; false otherwise.
    /// </summary>
    public bool DeregisterHazard(HazardTile hazard)
    {
        if (!placedHazards.ContainsKey(hazard.gridPos)) return false; // did not contain it
        // Else it did contain it
        placedHazards.Remove(hazard.gridPos);
        return true;
    }
    
     /// <summary>
    /// Places hazard tiles in a circular pattern on the tilemap.
    /// </summary>
    /// <param name="radius">Radius (in cells) of the circle.</param>
    /// <param name="position">World position of the circle's center.</param>
    /// <param name="hazardPrefab">The hazard tile prefab to spawn.</param>
    /// <returns>A list of the HazardTile instances that were instantiated.</returns>
    public List<HazardTile> PlaceHazardTiles(int radius, Vector3 position, HazardTile hazardPrefab)
    {
        List<HazardTile> placedObjects = new List<HazardTile>();

        // Convert world position to cell coordinates.
        Vector3Int centerCell = tilemap.WorldToCell(position);
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                Vector3Int cellPos = new Vector3Int(centerCell.x + x, centerCell.y + y, centerCell.z);
                if (x * x + y * y <= radius * radius) // Square magnitude
                {
                    // Get the world position for this cell.
                    Vector3 worldPos = tilemap.GetCellCenterWorld(cellPos);
                    
                    // Instantiate the hazard prefab.
                    HazardTile newHazard = Instantiate(hazardPrefab, worldPos, Quaternion.identity, transform);
                    newHazard.InitializeTile();
                    placedObjects.Add(newHazard);
                    // The tile registers itself, so no need to do it here
                }
            }
        }
        // return a list of the placed tiles
        return placedObjects;
    }
    
    

}
