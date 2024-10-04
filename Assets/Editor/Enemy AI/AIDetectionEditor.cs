using Pathfinding.Util;
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

        // Draw a red circle representing the proximity radius required to alert the enemy by "touch"
        Handles.color = Color.red;
        Handles.DrawWireArc(detectionScript.transform.position, Vector3.up, Vector3.forward, 360, detectionScript.proximityRadius);

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
            Handles.DrawLine(detectionScript.transform.position, detectionScript.playerTranformRef.position);
        }

        // Draw the foward looking arrow
        DrawFowardArrow(detectionScript);
    }

    /// <summary>
    /// Helper function to draw the foward vector that indicates where the enemy is looking at
    /// </summary>
    private void DrawFowardArrow(AIDetection target)
    {
        // Get the object's forward direction and position
        Vector3 forwardDirection = target.transform.forward;
        Vector3 position = target.transform.position;

        // Set arrow properties
        float arrowLength = 2f;  // Length of the arrow
        float arrowHeadLength = 0.5f; // Length of the arrowhead
        float arrowHeadAngle = 25f;   // Angle of the arrowhead

        // Draw an arrow to show the forward direction
        Handles.color = Color.green; // Arrow color
        Handles.DrawAAPolyLine(5f, position, position + forwardDirection * arrowLength);

        // Draw arrowhead at the end of the arrow
        Vector3 arrowEnd = position + forwardDirection * arrowLength;
        Vector3 rightArrowHead = Quaternion.LookRotation(forwardDirection) * Quaternion.Euler(0, arrowHeadAngle, 0) * Vector3.back;
        Vector3 leftArrowHead = Quaternion.LookRotation(forwardDirection) * Quaternion.Euler(0, -arrowHeadAngle, 0) * Vector3.back;

        Handles.DrawAAPolyLine(5f, arrowEnd, arrowEnd + rightArrowHead * arrowHeadLength);
        Handles.DrawAAPolyLine(5f, arrowEnd, arrowEnd + leftArrowHead * arrowHeadLength);
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
