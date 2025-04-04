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
 * Draws the trajectory of a hazard pellet
 */// --------------------------------------------------------


/// <summary>
/// Class that handles the predicting of a hazard pellet, needs more work honestly
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    [Header("Settings")]
    // The maximum number of points the LineRenderer can have
    [SerializeField, Range(10, 100)] private int maxPoints = 50; 
    // The time increment used to calculate the trajectory
    [SerializeField, Range(0.01f, 0.5f)] private float increment = 0.025f; 
    // The raycast overlap between points in the trajectory,
    // this is a multiplier of the length between points. 2 = twice as long
    [SerializeField, Range(1.05f, 2f)] private float rayOverlap = 1.1f;
    [SerializeField] private LayerMask predictRayIgnoreMask;
    
    [Header("References")]
    [SerializeField] private Transform hitMarker; // The marker will show where the projectile will hit
    
    [Header("Readouts")]
    [InspectorReadOnly] public float timePassed;
    [InspectorReadOnly] public Vector3 hitMarkerLocation;
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local Variables
    private LineRenderer trajectoryLine;
    [HideInInspector] public int hazardAmount = 0;

    private void Start()
    {
        if (trajectoryLine == null)
            trajectoryLine = GetComponent<LineRenderer>();

        SetTrajectoryVisible(true);
    }

    // We draw an arrow on the foward vector as to see which way the pellet is going
    private void OnDrawGizmosSelected()
    {
        // Draw the transform.foward to help with alignment work
        Gizmos.color = Color.red;

        // Define the start and end points of the forward arrow
        Vector3 start = transform.position;
        Vector3 end = start + transform.forward * 2f; // Arrow length

        // Draw the main arrow line
        Gizmos.DrawLine(start, end);

        // Draw arrowhead
        DrawArrowHead(end, transform.forward);
    }
    
    /// <summary>
    /// Function that predicts the trajectory of a projectile
    /// </summary>
    public void PredictTrajectory(ProjectileProperties projectile, Vector3 currentBallPosition)
    {
        // Start simulation from the launch state
        Vector3 velocity = projectile.direction * (projectile.initialSpeed / projectile.mass);
        Vector3 position = projectile.initialPosition;
    
        List<Vector3> validPoints = new List<Vector3>();
        float simulationTime = 0f; // Accumulates simulated time as to not draw past lines

        for (int i = 0; i < maxPoints; i++)
        {
            simulationTime += increment; // Increase simulation time per step

            // Update velocity and position for this time step
            velocity = CalculateNewVelocity(velocity, projectile.drag, increment);
            Vector3 nextPosition = position + velocity * increment;
            float overlap = Vector3.Distance(position, nextPosition) * rayOverlap;

            // Check for collision
            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, overlap, ~predictRayIgnoreMask))
            {
                // Only add the hit point if it occurs after timePassed
                if (simulationTime >= timePassed)
                {
                    validPoints.Add(hit.point);
                }
                MoveHitMarker(hit);
                break;
            }

            // Only add points that are in the future (simulationTime) 
            // and are in front of the current ball position
            if (simulationTime >= timePassed && 
                Vector3.Dot((nextPosition - currentBallPosition).normalized, projectile.direction) > 0)
            {
                validPoints.Add(nextPosition);
            }

            position = nextPosition;
        }

        // Update the line renderer: if no valid future points, hide the trajectory.
        if (validPoints.Count > 0)
        {
            SetTrajectoryVisible(true);
            trajectoryLine.positionCount = validPoints.Count;
            trajectoryLine.SetPositions(validPoints.ToArray());
        }
        else
        {
            SetTrajectoryVisible(false);
        }
    }
    
    /// <summary>
    /// Helper func to compute velocity
    /// </summary>
    /// <param name="velocity"></param>
    /// <param name="drag"></param>
    /// <param name="increment"></param>
    /// <returns></returns>
    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
    {
        velocity += Physics.gravity * increment;
        velocity *= Mathf.Clamp01(1f - drag * increment);
        return velocity;
    }
    
    /// <summary>
    /// Helper func to move the hitmarker
    /// </summary>
    /// <param name="hit"></param>
    private void MoveHitMarker(RaycastHit hit)
    {
        hitMarker.gameObject.SetActive(true);

        // Offset marker from surface
        float offset = 0.025f;
        hitMarker.position = hit.point + hit.normal * offset;
        hitMarker.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);

        // Store the hit marker position in the field for additional use
        hitMarkerLocation = hitMarker.position;
    }

    /// <summary>
    /// Helper func to either show or hide the trajectory
    /// </summary>
    /// <param name="visible"></param>
    public void SetTrajectoryVisible(bool visible)
    {
        trajectoryLine.enabled = visible;
        hitMarker.gameObject.SetActive(visible);
    }
    
    /// <summary>
    /// Draws an arrowhead at the given position, pointing in the specified direction.
    /// </summary>
    private void DrawArrowHead(Vector3 position, Vector3 direction)
    {
        float arrowHeadSize = 0.3f; // Size of the arrowhead
        float arrowAngle = 20f; // Angle of arrowhead wings

        // Calculate left and right wing directions
        Quaternion leftRotation = Quaternion.AngleAxis(arrowAngle, Vector3.up);
        Quaternion rightRotation = Quaternion.AngleAxis(-arrowAngle, Vector3.up);
    
        Vector3 leftWing = leftRotation * -direction * arrowHeadSize;
        Vector3 rightWing = rightRotation * -direction * arrowHeadSize;

        // Draw arrowhead lines
        Gizmos.DrawLine(position, position + leftWing);
        Gizmos.DrawLine(position, position + rightWing);
    }
}
