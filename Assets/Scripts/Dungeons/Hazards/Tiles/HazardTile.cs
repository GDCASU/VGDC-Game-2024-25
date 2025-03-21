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
 * Manages the singular hazard tile
 */// --------------------------------------------------------


/// <summary>
/// class that handles a hazard tile
/// </summary>
public class HazardTile : MonoBehaviour
{
    [Header("References")]
    public MeshRenderer meshRenderer;
    
    [Header("Settings")]
    public Elements element;
    public bool isReplaceable = true;
    public bool isPermanent = false;

    [Header("Optional")] 
    [SerializeField, Range(0f, 100f)] private float duration;

    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // The grid position of this hazard.
    public Vector3Int gridPos;

    private void Start()
    {
        InitializeTile();
    }

    private void OnDestroy()
    {
        // De-register
        GameGridManager.Instance.DeregisterHazard(this);
    }

    /// <summary>
    /// Initializes the hazard tile by snapping it to the grid and registering it.
    /// </summary>
    private void InitializeTile()
    {
        if (GameGridManager.Instance == null)
        {
            if (doDebugLog) Debug.LogWarning("GameGridManager instance not found!");
            return;
        }

        // Convert world position to grid cell and snap to center.
        gridPos = GameGridManager.Instance.tilemap.WorldToCell(transform.position);
        transform.position = GameGridManager.Instance.tilemap.GetCellCenterWorld(gridPos);

        // Attempt to register this hazard.
        bool registered = GameGridManager.Instance.RegisterHazard(this);
        if (!registered)
        {
            if (doDebugLog) Debug.Log($"HazardTile at {gridPos} could not be registered and will be destroyed.");
            Destroy(gameObject);
            return;
        }

        // If the hazard is not permanent, start the countdown.
        if (!isPermanent)
        {
            StartCoroutine(DurationRoutine());
        }
    }

    /// <summary>
    /// Coroutine to handle timed destruction of the hazard.
    /// </summary>
    private IEnumerator DurationRoutine()
    {
        yield return new WaitForSeconds(duration);
        if (GameGridManager.Instance && GameGridManager.Instance.placedHazards.ContainsKey(gridPos))
        {
            GameGridManager.Instance.placedHazards.Remove(gridPos);
        }
        Destroy(gameObject);
    }
}
