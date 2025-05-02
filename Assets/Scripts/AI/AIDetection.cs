
using Pathfinding;
using System.Collections;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Script to be attached to AI enemies so they can detect the player 
/// if they enter their sight
/// </summary>
public class AIDetection : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject detectedUI;
    [SerializeField] private AIDestinationSetter aiDestinationSetter;
    [SerializeField] private AIPath aiPath;

    [Header("Main Settings")]
    public LayerMask targetMask;
    public float stopDistance;
    [Range(0, 100)] public float detectionRadius;
    

    [Header("Debugging")]
    public bool doDebugLog;
    
    // Local Variables
    private bool detected = false;

    private void Start()
    {
        StartCoroutine(DetectedRoutine());
        detectedUI.SetActive(false);
    }

    /// <summary>
    /// Coroutine that handles the enemy once it has started aggression against player
    /// </summary>
    /// <returns></returns>
    private IEnumerator DetectedRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        // Spherecast until we find the player
        while (!detected)
        {
            // Wait between sphere casts to not overwhelm the engine
            yield return wait;
            
            // Spherecast
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, targetMask);
            
            // If we did hit the player, we found them
            if (hitColliders.Length > 0) detected = true;
        }
        
        // Set the destination setter to the player
        aiDestinationSetter.target = PlayerObject.Instance.transform;
        // Show detected UI
        StartCoroutine(ShowDetectionRoutine());
        
        // Now start checking for the stop distance
        while (true)
        {
            // Check distance from player
            float distance = Vector3.Distance(this.transform.position, PlayerObject.Instance.transform.position);
            
            // If in stopping distance
            if (distance < stopDistance)
            {
                aiPath.canMove = false;
            }
            else
            {
                aiPath.canMove = true;
            }
            // Wait a frame
            yield return null;
        }
    }
    
    /// <summary>
    /// Routine to show the detected UI for a bit for a visual indicator
    /// </summary>
    /// <returns></returns>
    private IEnumerator ShowDetectionRoutine()
    {
        // Show the detected symbol for a while to indicate enemy is aware of player
        detectedUI.SetActive(true);
        yield return new WaitForSeconds(2f);
        detectedUI.SetActive(false);
    }
    
    
    /// <summary>
    /// Draw the detection radius in the scene view for better calibration
    /// </summary>
    /*
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.yellow;

        Vector3 forward = transform.forward; // Enemy's forward direction
        Vector3 startDirection = Quaternion.Euler(0, -360 / 2, 0) * forward;

        Handles.DrawWireArc(transform.position, Vector3.up, startDirection, 360, detectionRadius);
    }
    */
}
