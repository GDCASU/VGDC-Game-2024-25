using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static EntityAIManager;

/// <summary>
/// Class that will handle the raycasts performed by the enemies using Unitys' Job System
/// </summary>
public class EntityAIManager : MonoBehaviour
{
    // Singleton reference
    public static EntityAIManager Instance;

    [Header("Settings")]
    [Tooltip("Should be a layer that only the player is in")]
    [SerializeField] private LayerMask playerLayerMask; 
    [Tooltip("Should be a layer with only things that block the enemy being able to see the player")]
    [SerializeField] private LayerMask obstructionMask;
    [Tooltip("The amount of results to return per spherecast done")]
    [SerializeField] [Range(1, 100)] private int maxHitsPerSphereCast;
    [Tooltip("The amount of results to return per raycast done")]
    [SerializeField] [Range(1, 100)] private int maxHitsPerRaycast;

    // Keep a list of all active enemies that need to perform raycasts
    private List<AIBrain> enemies = new List<AIBrain>();

    // References to multithreaded arrays
    private NativeArray<OverlapSphereCommand> sphereCastCommands;
    private NativeArray<ColliderHit> sphereCastResults;
    private NativeArray<bool> sphereCastFoundPlayer;

    // Since the amount of raycasts done is only the amount of enemies that found the player
    // We manage the memory per frame instead of using persistent allocation
    private NativeArray<RaycastCommand> raycastCommands;
    private NativeArray<RaycastHit> raycastResults;
    private NativeArray<bool> raycastFoundPlayer;

    // This array never changes size since its used to count how many sphere casts
    // Found the player
    public NativeArray<int> playerFoundCount;

    // Query Parameters for sphere casts
    private QueryParameters onlyPlayerQuery;
    private QueryParameters obstructionAndPlayerQuery;

    // Local Variables
    private int playerColliderInstanceID = 0;


    // TODO: IMPLEMENT LOGIC TO HANLDE THE OBSTRUCTION RAYCASTS AS WELL


    // Store the number of enemies using the FOV Job in case it changes
    private int previousEnemyCount = 0;

    private void Awake()
    {
        // Set the singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Allocate some empty Native Arrays for the sphere caster until enemies are on the scene
        AllocateNativeArrays();

        // playerFoundCount Gets special treatment since it always is of length 1
        playerFoundCount = new NativeArray<int>(1, Allocator.Persistent);

        // Construct queries
        onlyPlayerQuery = new QueryParameters
        {
            layerMask = playerLayerMask
        };
        obstructionAndPlayerQuery = new QueryParameters
        {
            layerMask = obstructionMask | playerLayerMask
        };
    }

    private void Start()
    {
        // Get the current player instance ID
        if (PlayerObject.Instance != null)
        {
            playerColliderInstanceID = PlayerObject.Instance.capsuleCollider.GetInstanceID();
        }
    }

    void Update()
    {
        // Dont do anything if the enemy count is 0
        if (enemies.Count <= 0) return;

        // If the player instance is not set, dont execute
        if (PlayerObject.Instance == null) return;

        // Make sure the player instance ID is set, if not dont do anything
        if (playerColliderInstanceID == 0)
        {
            // ID is not set
            Debug.Log("Player collider instance ID has not been set!");
            return;
        }
        
        // Check if the number of enemies has changed
        if (previousEnemyCount != enemies.Count)
        {
            // It has changed, dispose of Native Arrays
            FreeNativeArrays();
            // Allocate new Arrays
            AllocateNativeArrays();
            // Update previous enemy count
            previousEnemyCount = enemies.Count;
        }
        else
        {
            // Enemy count remains the same, so only reset the arrays
            ResetNativeArrays();
        }

        // Set up the sphere cast commands
        for (int i = 0; i < previousEnemyCount; i++)
        {
            // Setup the command for each enemy
            AIBrain currentEnemy = enemies[i];
            Vector3 enemyPos = currentEnemy.gameObject.transform.position;
            float enemyDetectionRadius = currentEnemy.aiDetectionScript.detectionRadius;
            sphereCastCommands[i] = new OverlapSphereCommand(enemyPos, enemyDetectionRadius, onlyPlayerQuery);
        }

        // Perform the multithreaded Cast
        JobHandle sphereCastHandle = OverlapSphereCommand.ScheduleBatch(sphereCastCommands, sphereCastResults, 1, maxHitsPerSphereCast);
        // Pause main thread until it completes
        sphereCastHandle.Complete();

        // Reset the Found Counter before setting up the job
        playerFoundCount[0] = 0;
        // Go through the results list and store booleans if the enemy found the player through the sphere cast
        SphereCastFoundPlayerJob sphereFoundJob = new SphereCastFoundPlayerJob
        {
            sphereCastResults = sphereCastResults,
            playerFoundCount = playerFoundCount,
            sphereCastFoundPlayer = sphereCastFoundPlayer,
            playerColliderInstanceID = playerColliderInstanceID,
            maxHitsPerSphereCast = maxHitsPerSphereCast
        };
        JobHandle sphereFoundHandle = sphereFoundJob.Schedule(sphereCastFoundPlayer.Length, 64);
        sphereFoundHandle.Complete();

        // HACK: Multithread this one day as well

        // Go through results array and check if the player is in proximity
        for (int i = 0; i < sphereCastFoundPlayer.Length; i++)
        {
            if (!sphereCastFoundPlayer[i])
            {
                // Didnt find the player, set its can see player bool to false
                enemies[i].aiDetectionScript.canSeePlayer = false;
            }
        }

        // Allocate memory for the raycast commands and its results
        int playerFoundCountInt = playerFoundCount[0];
        raycastCommands = new NativeArray<RaycastCommand>(playerFoundCountInt, Allocator.TempJob);
        raycastResults = new NativeArray<RaycastHit>(playerFoundCountInt * maxHitsPerRaycast, Allocator.TempJob);
        raycastFoundPlayer = new NativeArray<bool>(playerFoundCountInt, Allocator.TempJob);

        // HACK: Should multithread this forloop as well at some point
        List<AIBrain> enemiesWhoFoundPlayer = new List<AIBrain>();
        for (int i = 0; i < sphereCastFoundPlayer.Length; i++)
        {
            // Check if this enemy found the player in its spherecast
            if (sphereCastFoundPlayer[i])
            {
                // They did find them, add them to the array
                enemiesWhoFoundPlayer.Add(enemies[i]);
            }
        }

        // Set up raycast commands
        for (int i = 0; i < playerFoundCountInt; i++)
        {
            // Setup the command for each enemy
            AIBrain currentEnemy = enemiesWhoFoundPlayer[i];
            Vector3 enemyPos = currentEnemy.gameObject.transform.position;
            float enemyDetectionRadius = currentEnemy.aiDetectionScript.detectionRadius;
            Vector3 direction = (PlayerObject.Instance.transform.position - currentEnemy.transform.position).normalized;
            raycastCommands[i] = new RaycastCommand(enemyPos, direction, obstructionAndPlayerQuery, enemyDetectionRadius);
        }

        // Perform raycasts to see if there's any obstruction
        JobHandle raycastHandle = RaycastCommand.ScheduleBatch(raycastCommands, raycastResults, 1, maxHitsPerRaycast);
        raycastHandle.Complete();

        // Figure out which enemies found the player
        RaycastFoundPlayerJob raycastFoundJob = new RaycastFoundPlayerJob
        {
            raycastResults = raycastResults,
            raycastFoundPlayer = raycastFoundPlayer,
            maxHitsPerRaycast = maxHitsPerRaycast,
            playerColliderInstanceID = playerColliderInstanceID
};
        JobHandle raycastFoundHandle = raycastFoundJob.Schedule(raycastFoundPlayer.Length, 64);
        raycastFoundHandle.Complete();

        // HACK: multithread this one day as well

        // Loop through the results, if an enemy has a line of sight to the player, set its canSeePlayer bool, else false
        for (int i = 0; i < enemiesWhoFoundPlayer.Count; i++)
        {
            // Check if this raycast wasnt stopped by obstruction
            if (!raycastFoundPlayer[i])
            {
                // Was obstructed
                enemiesWhoFoundPlayer[i].aiDetectionScript.canSeePlayer = false;
                continue;
            }

            // raycast did find player, check if is within field of vision
            Vector3 directionToTarget = raycastCommands[i].direction;
            bool canSee = CheckFOV(enemiesWhoFoundPlayer[i], directionToTarget);

            if (canSee)
            {
                // Enemy can see the player
                enemiesWhoFoundPlayer[i].aiDetectionScript.canSeePlayer = true;
            }
            else
            {
                // player wasnt within field of view
                enemiesWhoFoundPlayer[i].aiDetectionScript.canSeePlayer = false;
            }
        }

        // When raycasts are no longer needed, free the arrays from memory
        raycastCommands.Dispose();
        raycastResults.Dispose();
        raycastFoundPlayer.Dispose();

        /*

        // HACK: Testing logs
        for (int i = 0; i < sphereCastResults.Length; i++)
        {
            // Check if there was a hit
            if (sphereCastResults[i].instanceID == 0)
            {
                // No hits detected, skip to next one
                i += maxHitsPerSphereCast;
                continue;
            }
            // else there was a hit
            Debug.Log("We Hit = " + sphereCastResults[i].collider.name);
        }

        */
    }

    void OnDestroy()
    {
        // Dispose of NativeArrays when the object is destroyed to avoid memory leaks
        FreeNativeArrays();
        // Free up the counter array since it gets special treatment
        if (playerFoundCount.IsCreated) playerFoundCount.Dispose();
    }

    // HACK: This function should use multithreading as well
    /// <summary>
    /// Function to perform a check on the enemies to see if the player is within field of view
    /// </summary>
    private bool CheckFOV(AIBrain enemy, Vector3 directionToTarget)
    {
        // Bool to check both within field of view and proximity
        bool FOVDetected = false;
        bool proximityDetected = false;

        // Compute distance to player
        float distanceToTarget = Vector3.Distance(enemy.transform.position, PlayerObject.Instance.transform.position);

        // Check if the player is within the field of view
        if (!(Vector3.Angle(enemy.transform.forward, directionToTarget) > enemy.aiDetectionScript.viewAngle / 2))
        {
            // Player is inside field of view
            FOVDetected = true;
        }

        // Check if player is close enough for proximity detection
        if (distanceToTarget <= enemy.aiDetectionScript.proximityRadius)
        {
            proximityDetected = true;
        }

        // All checks have been made, evaluate the result

        if (proximityDetected || FOVDetected)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Adds an enemy to the AI manager
    /// </summary>
    public void AddEnemy(AIBrain enemy)
    {
        enemies.Add(enemy);
    }

    /// <summary>
    /// Removes an enemy from the AI manager
    /// </summary>

    public void RemoveEnemy(AIBrain enemy)
    {
        enemies.Remove(enemy);
    }

    /// <summary>
    /// Helper function to free the memory used by Native Arrays
    /// </summary>
    private void FreeNativeArrays()
    {
        if (sphereCastCommands.IsCreated) sphereCastCommands.Dispose();
        if (sphereCastResults.IsCreated) sphereCastResults.Dispose();
        if (sphereCastFoundPlayer.IsCreated) sphereCastFoundPlayer.Dispose();
    }

    /// <summary>
    /// Helper function to allocate new Native Arrays, cant be called without freeing the native arrays first
    /// </summary>
    private void AllocateNativeArrays()
    {
        // Check that native arrays were disposed off first
        if (sphereCastCommands.IsCreated || sphereCastResults.IsCreated || sphereCastFoundPlayer.IsCreated)
        {
            Debug.LogError("ERROR! YOU ARE SUPPOSED TO FREE THE NATIVE ARRAYS BEFORE CALLING ALLOC");
            return;
        }
        
        // Allocate sphere cast commands
        sphereCastCommands = new NativeArray<OverlapSphereCommand>(enemies.Count, Allocator.Persistent);

        // Allocate sphere cast results
        sphereCastResults = new NativeArray<ColliderHit>(enemies.Count * maxHitsPerSphereCast, Allocator.Persistent);

        // Allocate sphere cast found player
        sphereCastFoundPlayer = new NativeArray<bool>(enemies.Count, Allocator.Persistent);
    }

    /// <summary>
    /// Helper function to reset the Native Arrays using multithreading
    /// </summary>
    private void ResetNativeArrays()
    {
        // Schedule reset for sphereCastCommands array
        ResetSphereCastCommandsJob resetCommandsJob = new ResetSphereCastCommandsJob
        {
            sphereCastCommands = sphereCastCommands
        };
        JobHandle resetCommandsHandle = resetCommandsJob.Schedule(sphereCastCommands.Length, 64);

        // Schedule reset for sphereCastResults array
        ResetSphereCastResultsJob resetResultsJob = new ResetSphereCastResultsJob
        {
            sphereCastResults = sphereCastResults
        };
        JobHandle resetResultsHandle = resetResultsJob.Schedule(sphereCastResults.Length, 64);

        // Schedule reset for sphereCastFoundPlayer array
        ResetSphereCastFoundPlayerJob resetFoundJob = new ResetSphereCastFoundPlayerJob
        {
            sphereCastFoundPlayer = sphereCastFoundPlayer
        };
        JobHandle resetFoundHandle = resetFoundJob.Schedule(sphereCastFoundPlayer.Length, 64);

        // Ensure all jobs complete
        JobHandle.CompleteAll(ref resetCommandsHandle, ref resetResultsHandle, ref resetFoundHandle);
    }

    #region JOBS

    /// <summary>
    /// Array Reset Job for the Sphere Cast Commands
    /// </summary>
    [BurstCompile]
    public struct ResetSphereCastCommandsJob : IJobParallelFor
    {
        public NativeArray<OverlapSphereCommand> sphereCastCommands;

        public void Execute(int index)
        {
            // Set each command to default
            sphereCastCommands[index] = default(OverlapSphereCommand);
        }
    }

    /// <summary>
    /// Array Reset Job for the Sphere Cast Results
    /// </summary>
    [BurstCompile]
    public struct ResetSphereCastResultsJob : IJobParallelFor
    {
        public NativeArray<ColliderHit> sphereCastResults;

        public void Execute(int index)
        {
            // Set each result to default
            sphereCastResults[index] = default(ColliderHit);
        }
    }

    /// <summary>
    /// Array Reset Job for the Sphere Cast Results
    /// </summary>
    [BurstCompile]
    public struct ResetSphereCastFoundPlayerJob : IJobParallelFor
    {
        public NativeArray<bool> sphereCastFoundPlayer;

        public void Execute(int index)
        {
            // Set each result to default
            sphereCastFoundPlayer[index] = false;
        }
    }

    /// <summary>
    /// Job to figure out what enemies found the player through the sphere cast
    /// </summary>
    [BurstCompile]
    public struct SphereCastFoundPlayerJob : IJobParallelFor
    {
        public NativeArray<ColliderHit> sphereCastResults;
        public NativeArray<bool> sphereCastFoundPlayer;
        public int maxHitsPerSphereCast;
        public int playerColliderInstanceID;

        // Shared counter for how many times the player was found
        public NativeArray<int> playerFoundCount;

        public void Execute(int index)
        {
            // Iterate on the results of the array starting from index * maxHits,
            // up to index + maxHits
            // If we find a collider with an instance ID of 0, stop
            int startIdxOnResultArr = index * maxHitsPerSphereCast;
            for (int i = startIdxOnResultArr; i < startIdxOnResultArr + maxHitsPerSphereCast; i++)
            {
                // Check if its an invalid collider, break loop if so and report false as found
                if (sphereCastResults[i].instanceID == 0)
                {
                    // Collider list ended, player was not found
                    sphereCastFoundPlayer[index] = false;
                    return;
                }
                // Check if its the player, if so, break loop and report true as found
                if (sphereCastResults[i].instanceID == playerColliderInstanceID)
                {
                    // Player was found
                    sphereCastFoundPlayer[index] = true;
                    // Increment the counter
                    // TODO: Check if this is the best way to count in a parallel job
                    // Wouldnt this raise the chance of races?
                    playerFoundCount[0] = playerFoundCount[0] + 1;
                    return;
                }
            }

            // SphereCast found other objects, but none of them were the player
            sphereCastFoundPlayer[index] = false;
        }
    }

    /// <summary>
    /// Job to figure out what enemies found the player through a raycast
    /// </summary>
    [BurstCompile]
    public struct RaycastFoundPlayerJob : IJobParallelFor
    {
        public NativeArray<RaycastHit> raycastResults;
        public NativeArray<bool> raycastFoundPlayer;
        public int maxHitsPerRaycast;
        public int playerColliderInstanceID;

        public void Execute(int index)
        {
            // Iterate on the results of the array starting from index * maxHits,
            // up to index + maxHits
            // If we find a collider with an instance ID of 0, stop
            int startIdxOnResultArr = index * maxHitsPerRaycast;
            for (int i = startIdxOnResultArr; i < startIdxOnResultArr + maxHitsPerRaycast; i++)
            {
                // Check if its an invalid collider, break loop if so and report false as found
                if (raycastResults[i].colliderInstanceID == 0)
                {
                    // Collider list ended, player was not found
                    raycastFoundPlayer[index] = false;
                    return;
                }
                // Check if its the player, if so, break loop and report true as found
                if (raycastResults[i].colliderInstanceID == playerColliderInstanceID)
                {
                    // Player was found
                    raycastFoundPlayer[index] = true;
                    return;
                }
            }

            // SphereCast found other objects, but none of them were the player
            raycastFoundPlayer[index] = false;
        }
    }

    #endregion
}

