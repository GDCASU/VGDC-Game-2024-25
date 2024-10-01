using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Script to add visual help to the FOV and Proximity Settings of enemies
/// </summary>
[CustomEditor(typeof(AIDetection))]
public class AIDetectionEditor : Editor
{
    // Whenever the user clicks on the object within the hierarchy, show guidelines in scene view
    private void OnSceneGUI()
    {
        // Get the FOV Script
        AIDetection detectionScript = (AIDetection)target;

        // Draw the circle radious of the view radius sphere cast
        Handles.color = Color.yellow;
        Handles.DrawWireArc(detectionScript.transform.position, Vector3.up, Vector3.forward, 360, detectionScript.viewRadius);

        // Draw the circle radious of the alert radius sphere cast
        Handles.color = Color.yellow;
        Handles.DrawWireArc(detectionScript.transform.position, Vector3.up, Vector3.forward, 360, detectionScript.alertRadius);

        // Draw the circle representing the enemy's current view range
        Handles.color = Color.white;
        Handles.DrawWireArc(detectionScript.transform.position + (Vector3.up / 2), Vector3.up, Vector3.forward, 360, detectionScript.detectionRadius);

        // Compute the angle from foward to the limit of the angle
        Vector3 viewAngle01 = DirectionFromAngle(detectionScript.transform.eulerAngles.y, -detectionScript.viewAngle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(detectionScript.transform.eulerAngles.y, detectionScript.viewAngle / 2);

        // Draw the limit lines of the view angle in yellow
        Handles.color = Color.yellow;
        Handles.DrawLine(detectionScript.transform.position, detectionScript.transform.position + viewAngle01 * detectionScript.alertRadius);
        Handles.DrawLine(detectionScript.transform.position, detectionScript.transform.position + viewAngle02 * detectionScript.alertRadius);
        // If we can see the player, draw a green line towards them
        if (detectionScript.canSeePlayer)
        {
            Handles.color = Color.green;
            Handles.DrawLine(detectionScript.transform.position, detectionScript.playerRef.transform.position);
        }

        // Draw a red circle representing the proximity radius required to alert the enemy by "touch"
        Handles.color = Color.red;
        Handles.DrawWireArc(detectionScript.transform.position, Vector3.up, Vector3.forward, 360, detectionScript.proximityRadius);
    }

    /// <summary>
    /// Helper function to compute the angle from 0 to the edge of the view angle
    /// </summary>
    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
