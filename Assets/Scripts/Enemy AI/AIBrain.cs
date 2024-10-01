using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that manages all of the AI behaviours done by an entity
/// </summary>
public class AIBrain : MonoBehaviour
{
    [Header("Script References")]
    public AIDetection aiDetectionScript;
    public AIDestinationSetter destinationSetter;
    public AIPath AIPathScript;
    public Seeker seekerScript;

    [Header("Settings")]
    public float enemyMaxSpeed;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // Register Entity with the manager
        // HACK: Should probably re-adapt this so we can do non-enemy entities
        // if (enemy) ... else if (NPC)....
        EntityAIManager.Instance.AddEnemy(this);

        // Settings
        AIPathScript.maxSpeed = enemyMaxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        // Un-Register enemy in the Entity AI Manager
        EntityAIManager.Instance.RemoveEnemy(this);
    }
}
