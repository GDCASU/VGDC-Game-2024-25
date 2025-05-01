using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

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
public class HazardTile : MonoBehaviour, IDamageable
{
    // Class to categorize the tiles
    public enum TileType
    {
        Water = 1,
        Grass = 2,
        Fungal = 3,
        Rock = 4,
        GrassFire = 5,
        Barrier = 6,
    }
    
    [Header("References")]
    public MeshRenderer meshRenderer;
    
    [Header("Settings")]
    public TileType tileType;
    public bool isReplaceable = true;
    public bool isPermanent = false; // If set to false, will only last for the duration set

    [Header("For non permanent")] 
    [SerializeField, Range(0f, 100f)] private float duration;

    [Header("For barrier tiles")] 
    [SerializeField] private UnityEvent onBarrierDestroyed;
    [SerializeField] private UnityEvent<Elements> onDamageTaken;

    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // The grid position of this hazard.
    [Header("Readouts")]
    [InspectorReadOnly] public Vector3Int gridPos;
    [InspectorReadOnly] public bool initialized = false;

    private void Start()
    {
        // Initialize tile
        InitializeTile();
        // If the hazard is not permanent, start the countdown.
        if (!isPermanent)
        {
            StartCoroutine(DurationRoutine());
        }
    }

    private void OnDestroy()
    {
        if (!GameGridManager.Instance) return; // Make sure is not null
        // Check if we need to de-register it
        if (GameGridManager.Instance.placedHazards.TryGetValue(gridPos, out HazardTile tile))
        {
            if (tile == this) GameGridManager.Instance.DeregisterHazard(this);
        }
        // For barriers
        if (tileType == TileType.Barrier)
        {
            onBarrierDestroyed?.Invoke();
        }
    }

    /// <summary>
    /// Initializes the hazard tile by snapping it to the grid and registering it.
    /// Called by the grid manager
    /// </summary>
    public void InitializeTile()
    {
        // Make sure the game grid singleton is set
        if (GameGridManager.Instance == null)
        {
            if (doDebugLog) Debug.LogWarning("GameGridManager instance not found!");
            return;
        }
        
        // Dont run if already initialized
        if (initialized) return;

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
        
        // Finished creating tile
        initialized = true;
    }

    /// <summary>
    /// Coroutine to handle timed destruction of the hazard.
    /// </summary>
    private IEnumerator DurationRoutine()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    public ReactionType TakeDamage(int damage, Elements element)
    {
        onDamageTaken?.Invoke(element);
        return ReactionType.Undefined;
    }
}
