using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Script to be attached to AI enemies so they can detect the player 
/// if they enter their sight
/// </summary>
public class AIDetection : MonoBehaviour
{
    [Header("AI Brain Reference")]
    [SerializeField] private AIBrain aiBrain;

    [Header("Canvas Reference")]
    [SerializeField] private GameObject discoveringParent;
    [SerializeField] private RectTransform discoveringMask;
    [SerializeField] private RectTransform discoveringFill;
    [SerializeField] private GameObject detectedParent;
    [SerializeField] private RectTransform detectedMask;
    [SerializeField] private RectTransform detectedFill;

    [Header("DELETEME")]
    [SerializeField] private GameObject detectionCanvas;

    [Header("Main Settings")]
    public string targetTag;
    public LayerMask targetMask;
    public LayerMask obstructionMask;

    [Header("Detection Settings")]
    [SerializeField] private float timeToDetect; // Time it takes enemy to enter combat
    [SerializeField] private float holdDiscoverTime;  // Time it will take until enemy starts lowering their discovery meter
    [SerializeField] private float timeToForget;
    [SerializeField] private float holdDetectionTime;  // Time it will take until enemy starts lowering their detected meter

    [Header("FOV Settings")]
    [Range(0, 100)] public float viewRadius; // Default ring of vision of enemy
    [Range(0, 100)] public float alertRadius; // Enemy will gain greater detection radius if alert
    [Range(0, 360)] public float viewAngle;

    [Header("Proximity Settings")]
    [Range(0, 100)] public float proximityRadius;

    [Header("Current Values")]
    public GameObject playerRef;
    public bool canSeePlayer;

    [Header("Debugging")]
    public bool doDebugLog;
    [InspectorLabel("Warnings refer to the radius and angle warnings logged by the dependency checker")]
    public bool doLogWarnings;

    // Local Variables
    [HideInInspector] public float detectionRadius = 0;


    private void Start()
    {
        // Find player in the scene
        playerRef = GameObject.FindGameObjectWithTag(targetTag);

        // Check that the settings were correctly setup, if not, dont execute
        bool correctSetup = CheckDependencies();
        if (!correctSetup) return;

        // Set starting view radius
        detectionRadius = viewRadius;

        // Start Detection Timer Routine
        StartCoroutine(DiscoverRoutine());
    }

    // HACK: DELETE ME
    private void Update()
    {
        detectionCanvas.transform.rotation = Camera.main.transform.rotation;
    }

    /// <summary>
    /// Will handle counting until player is discovered
    /// </summary>
    /// <returns></returns>
    private IEnumerator DiscoverRoutine()
    {
        float count = 0;
        float holdCount = 0;
        Vector2 fillSizeDelta = new Vector2(discoveringMask.rect.width, 0);
        discoveringFill.sizeDelta = fillSizeDelta;

        // Compute rate of change
        float rateOfChange = discoveringMask.rect.height / timeToDetect;

        while (count < timeToDetect)
        {
            if (canSeePlayer)
            {
                // We can see the player, Count
                discoveringParent.SetActive(true);
                count += Time.deltaTime;
                holdCount = holdDiscoverTime;
                fillSizeDelta.y = rateOfChange * count;
                discoveringFill.sizeDelta = fillSizeDelta;
                yield return null;
                continue;
            }
            // We cant see the player anymore, first hold current discovery level for some time
            if (holdCount > 0)
            {
                holdCount -= Time.deltaTime;
                yield return null;
                continue;
            }
            else holdCount = 0;
            // Hold time has ended, start dropping discovery count if accumulated
            if (count > 0)
            {
                count -= Time.deltaTime;
                fillSizeDelta.y = rateOfChange * count;
                discoveringFill.sizeDelta = fillSizeDelta;
                yield return null;
                continue;
            }
            else 
            {
                // No player discovered
                count = 0;
                discoveringParent.SetActive(false);
                yield return null;
            }
        }
        // Player has been fully discovered, go to detected routine
        detectionRadius = alertRadius;
        discoveringParent.SetActive(false);
        // TODO: PLAY DETECTED SOUND
        StartCoroutine(DetectedRoutine());
    }

    /// <summary>
    /// Coroutine that handles the enemy once it has started aggression against player
    /// </summary>
    /// <returns></returns>
    private IEnumerator DetectedRoutine()
    {
        float count = timeToForget;
        float holdCount = 0;
        detectedParent.SetActive(true);
        Vector2 fillSizeDelta = new Vector2(detectedMask.rect.width, detectedMask.rect.height);
        detectedFill.sizeDelta = fillSizeDelta;

        // Compute rate of change
        float rateOfChange = detectedMask.rect.height / timeToForget;

        // Start chasing player since we discovered them
        aiBrain.destinationSetter.target = playerRef.transform;
        
        // Counts down until forget time has been met
        while (count > 0)
        {
            if (canSeePlayer)
            {
                // We can see the player, set counter to full
                aiBrain.destinationSetter.target = playerRef.transform;
                count = timeToForget;
                holdCount = holdDetectionTime;
                fillSizeDelta.y = rateOfChange * count;
                detectedFill.sizeDelta = fillSizeDelta;
                yield return null;
                continue;
            }
            // We cant see the player anymore
            aiBrain.destinationSetter.target = null;
            // Hold current discovery level for some time
            if (holdCount > 0)
            {
                holdCount -= Time.deltaTime;
                yield return null;
                continue;
            }
            else holdCount = 0;
            // Hold time has ended, start dropping detection count
            count -= Time.deltaTime;
            fillSizeDelta.y = rateOfChange * count;
            detectedFill.sizeDelta = fillSizeDelta;
            yield return null;
            continue;
        }
        // Player has escaped, enemy gave up the chase
        detectionRadius = viewRadius;
        detectedParent.SetActive(false);
        aiBrain.destinationSetter.target = null;
        aiBrain.seekerScript.CancelCurrentPathRequest();
        // Go back to discover routine
        StartCoroutine(DiscoverRoutine());
    }

    /// <summary>
    /// Function to be called when this enemy is alerted by another enemy of the location of the player
    /// </summary>
    private void AggressionAlert()
    {

    }

    private IEnumerator AggressionAlertRoutine()
    {
        // Compute a path from the enemy to player
        Path path = aiBrain.seekerScript.StartPath(transform.position, playerRef.transform.position);
        // Wait for path computation
        yield return StartCoroutine(path.WaitForPath());
        // Path has been computed, go to point
        aiBrain.seekerScript.StartPath(path);

        //aiBrain.seekerScript.
    }

    /// <summary>
    /// Helper functions to log helpful warning to the console. Returns a bool if some dependency wasnt accounted for
    /// </summary>
    private bool CheckDependencies()
    {
        string errorColor = "<color=red>";
        string warningColor = "<color=yellow>";
        string colorReset = "</color>";
        string objName = gameObject.name;
        bool messageChanged = false;
        bool isAllGood = true;
        string consoleMsg = errorColor + "ERRORS FOUND ON " + objName + "!\n" + colorReset;
        // Check that player was found
        if (playerRef == null)
        {
            consoleMsg += errorColor + "Player object wasnt found on scene! Maybe try checking the target tag.\n" + colorReset;
            isAllGood = false;
            messageChanged = true;
        }
        // Check target Mask
        if (targetMask == 0)
        {
            consoleMsg += errorColor + "Target Mask hasnt been set!\n" + colorReset;
            isAllGood = false;
            messageChanged = true;
        }
        // Check obstruction mask
        if (obstructionMask == 0)
        {
            consoleMsg += errorColor + "Obstruction Mask hasnt been!\n" + colorReset;
            isAllGood = false;
            messageChanged = true;
        }
        // Check detection radius
        if (detectionRadius <= 0 && doLogWarnings)
        {
            consoleMsg += warningColor + "Warning! Detection radius value is at 0 or less!" + colorReset;
            messageChanged = true;
        }
        // Check view angle
        if (viewAngle <= 0 && doLogWarnings)
        {
            consoleMsg += warningColor + "Warning! View angle value is at 0 or less!" + colorReset;
            messageChanged = true;
        }
        // Check proximity radius
        if (proximityRadius <= 0 && doLogWarnings)
        {
            consoleMsg += warningColor + "Warning! Proximity radius value is at 0 or less!" + colorReset;
            messageChanged = true;
        }
        // Log message to console if string was changed and return
        if (messageChanged) Debug.Log(consoleMsg);
        return isAllGood;
    }
}
