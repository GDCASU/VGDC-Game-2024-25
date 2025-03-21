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
 * Throws a hazard pellet given parameters
 */// --------------------------------------------------------


/// <summary>
/// Class that handles the throwing of a projectile and then predicts its path
/// </summary>
[RequireComponent(typeof(TrajectoryPredictor))]
public class ProjectileThrow : MonoBehaviour
{
    [Header("Projectile")]
    public Rigidbody objectToThrow;
    
    [Header("References")]
    [SerializeField] private TrajectoryPredictor trajectoryPredictor;
    [SerializeField] private HazardTile hazardTilePrefab;
    
    [Header("Settings")]
    [Range(0.0f, 50.0f)] public float force;
    [Range(-45f, 45f)] public float verticalAngle = 0f; 
    
    [Header("Optional")]
    [SerializeField] private Transform StartPosition;
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local variables
    [HideInInspector] public int hazardAmount = 0;

    void OnEnable()
    {
        // Check if a start position was set
        if (StartPosition == null)
            StartPosition = transform;
    }
    
    // Fire the hazard creator
    void Start()
    {
        // Reset position before throwing
        objectToThrow.transform.position = transform.position;
        trajectoryPredictor.hazardAmount = hazardAmount;
        
        // Calculate vertical rotation using the hazard's own right vector
        Quaternion verticalRotation = Quaternion.AngleAxis(verticalAngle, transform.right);
        Vector3 launchDirection = verticalRotation * transform.forward;

        // Apply force in the modified direction
        objectToThrow.AddForce(launchDirection * force, ForceMode.Impulse);
        
        // Start flood routine
        StartCoroutine(FloodFillRoutine());
    }
    
    /// <summary>
    /// Routine that will handle the prediction flood and filling of the target tile at the destination
    /// </summary>
    /// <returns></returns>
    private IEnumerator FloodFillRoutine()
    {
        // Wait for 5 frames so the hitmarker stabilizes
        for (int i = 0; i < 5; i++) yield return null;
        
        // Get the cell at the position 
        Vector3Int cellLocation = GameGridManager.Instance.tilemap.WorldToCell(trajectoryPredictor.hitMarkerLocation);
        
        // Now generate a ghost hazard puddle that waits for the target to reach destination
        GameGridManager.Instance.PlaceHazardTiles(hazardAmount, cellLocation, hazardTilePrefab);
    }

    void Update()
    {
        trajectoryPredictor.PredictTrajectory(ProjectileData(), objectToThrow.position);
    }

    ProjectileProperties ProjectileData()
    {
        ProjectileProperties properties = new ProjectileProperties();
        // Use the ball's current velocity instead of the static launch direction
        properties.direction = objectToThrow.velocity.normalized;
        properties.initialPosition = objectToThrow.position;
        properties.initialSpeed = objectToThrow.velocity.magnitude;
        properties.mass = objectToThrow.mass;
        properties.drag = objectToThrow.drag;
        return properties;
    }
}

/// <summary>
/// Helper struct to send data through the predict trajectory function without a million parameters
/// </summary>
public struct ProjectileProperties
{
    public Vector3 direction;
    public Vector3 initialPosition;
    public float initialSpeed;
    public float mass;
    public float drag;
}