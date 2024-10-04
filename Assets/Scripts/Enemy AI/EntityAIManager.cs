using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

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
    [SerializeField][Range(1, 100)] private int maxHitsPerSphereCast;

    // Keep a list of all active enemies that need to perform raycasts
    private List<AIBrain> enemies = new List<AIBrain>();

    // References to multithreaded arrays
    private NativeArray<OverlapSphereCommand> sphereCastCommands;
    private NativeArray<ColliderHit> sphereCastResults;
    private NativeArray<bool> sphereCastFoundPlayer;

    // This array never changes size since its used to count how many sphere casts
    // Found the player
    public NativeArray<int> playerFoundCount;

    // Query Parameters for sphere casts
    private QueryParameters onlyPlayerQuery;
    private QueryParameters obstructionAndPlayerQuery;

    // Local Variables
    private int playerColliderInstanceID = 0;

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

    void Update()
    {
        // Dont do anything if the enemy count is 0
        if (enemies.Count <= 0) return;

        // If the player instance is not set, dont execute, else get collider ID
        if (PlayerObject.Instance == null) return;
        else playerColliderInstanceID = PlayerObject.Instance.capsuleCollider.GetInstanceID();

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

        // Go through results array and check if the player is in proximity
        for (int i = 0; i < sphereCastFoundPlayer.Length; i++)
        {
            if (!sphereCastFoundPlayer[i])
            {
                // Didnt find the player, set its can see player bool to false
                enemies[i].aiDetectionScript.canSeePlayer = false;
            }
        }

        // Figure out what enemies found the player
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

        // Allocate memory for the raycast commands and its results, each enemy performs 3 raycasts:
        // One to center of player, one to the left and one to the right
        int playerFoundCountInt = playerFoundCount[0];
        NativeArray<RaycastCommand> raycastCommands = new NativeArray<RaycastCommand>(playerFoundCountInt * 3, Allocator.TempJob);
        NativeArray<RaycastHit> raycastResults = new NativeArray<RaycastHit>(playerFoundCountInt * 3, Allocator.TempJob);
        NativeArray<bool> raycastFoundPlayer = new NativeArray<bool>(playerFoundCountInt, Allocator.TempJob);
        NativeArray<float3> enemyPositions = new NativeArray<float3>(playerFoundCountInt, Allocator.TempJob);
        NativeArray<float> enemyDetectionRadiuses = new NativeArray<float>(playerFoundCountInt, Allocator.TempJob);

        // This part is very error prone, so its enclosed in a try and catch
        try
        {
            // Declare some variables for the raycast setup
            float sidecastOffset = 0.12f;
            float playerCapsuleRadius = PlayerObject.Instance.capsuleCollider.radius * PlayerObject.Instance.transform.localScale.x - sidecastOffset;
            Vector3 playerPos = PlayerObject.Instance.transform.position;

            // Get enemy data to perform raycasts
            for (int i = 0; i < enemiesWhoFoundPlayer.Count; i++)
            {
                AIBrain currentEnemy = enemiesWhoFoundPlayer[i];
                enemyPositions[i] = currentEnemy.gameObject.transform.position;
                enemyDetectionRadiuses[i] = currentEnemy.aiDetectionScript.detectionRadius;
            }

            // Setup the raycast commands through a job
            SetupRaycastsCommandsJob raycastSetupJob = new SetupRaycastsCommandsJob
            {
                // Variables
                sideCastOffset = sidecastOffset, // Used to separate the sidecasts from the center cast
                playerCapsuleRadius = playerCapsuleRadius,
                playerPos = playerPos,
                // Arrays
                raycastCommands = raycastCommands,
                enemyPositions = enemyPositions,
                enemyDetectionRadiuses = enemyDetectionRadiuses,
                // Queries
                obstructionAndPlayerQuery = obstructionAndPlayerQuery
            };
            JobHandle raycastSetupHandle = raycastSetupJob.Schedule(raycastCommands.Length, 64);
            raycastSetupHandle.Complete();

            // Perform raycasts to see if there's any obstruction
            JobHandle raycastHandle = RaycastCommand.ScheduleBatch(raycastCommands, raycastResults, 1, 1);
            raycastHandle.Complete();

            // Figure out which enemies found the player
            RaycastFoundPlayerJob raycastFoundJob = new RaycastFoundPlayerJob
            {
                raycastResults = raycastResults,
                raycastFoundPlayer = raycastFoundPlayer,
                castPerEnemy = 3,
                playerColliderInstanceID = playerColliderInstanceID
            };
            JobHandle raycastFoundHandle = raycastFoundJob.Schedule(raycastFoundPlayer.Length, 64);
            raycastFoundHandle.Complete();

            // IAN NOTE: This part could also be multithreaded, but I find it
            // to be a meaningless, tedious memory bomb just to do so...

            // Loop through the results, if an enemy has a line of sight to the player, set its canSeePlayer bool, else false
            for (int i = 0; i < playerFoundCountInt; i++)
            {
                // Check if this raycast wasnt stopped by obstruction
                if (!raycastFoundPlayer[i])
                {
                    // Was obstructed
                    enemiesWhoFoundPlayer[i].aiDetectionScript.canSeePlayer = false;
                    continue;
                }

                // raycast did find player, check if is within field of vision
                bool check1 = CheckFOV(enemiesWhoFoundPlayer[i], raycastCommands[i].direction, raycastResults[i].point);
                bool check2 = CheckFOV(enemiesWhoFoundPlayer[i], raycastCommands[i + 1].direction, raycastResults[i + 1].point);
                bool check3 = CheckFOV(enemiesWhoFoundPlayer[i], raycastCommands[i + 2].direction, raycastResults[i + 2].point);

                if (check1 || check2 || check3)
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

            // When finished, free the arrays from memory
            raycastCommands.Dispose();
            raycastResults.Dispose();
            raycastFoundPlayer.Dispose();
            enemyPositions.Dispose();
            enemyDetectionRadiuses.Dispose();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            if (raycastCommands.IsCreated) raycastCommands.Dispose();
            if (raycastResults.IsCreated) raycastResults.Dispose();
            if (raycastFoundPlayer.IsCreated) raycastResults.Dispose();
            if (enemyPositions.IsCreated) enemyPositions.Dispose();
            if (enemyDetectionRadiuses.IsCreated) enemyDetectionRadiuses.Dispose();
        }
    }

    void OnDestroy()
    {
        // Dispose of NativeArrays when the object is destroyed to avoid memory leaks
        FreeNativeArrays();
        // Free up the counter array since it gets special treatment
        if (playerFoundCount.IsCreated) playerFoundCount.Dispose();
    }

    /// <summary>
    /// Function to perform a check on the enemies to see if the player is within field of view
    /// </summary>
    private bool CheckFOV(AIBrain enemy, Vector3 directionToTarget, Vector3 targetPoint)
    {
        // Bool to check both within field of view and proximity
        bool FOVDetected = false;
        bool proximityDetected = false;

        // Compute distance to player
        float distanceToTarget = Vector3.Distance(enemy.transform.position, targetPoint);

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
            enemy.aiDetectionScript.isProximityTriggered = true;
        }
        else
        {
            enemy.aiDetectionScript.isProximityTriggered = false;
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

    public struct SetupRaycastsCommandsJob : IJobParallelFor // Length should be 3 times the amount of enemies (3 raycasts per enemy)
    {
        // Variables
        [ReadOnly] public float sideCastOffset; // Used to separate the sidecasts from the center cast
        [ReadOnly] public float playerCapsuleRadius;
        [ReadOnly] public float3 playerPos;

        // Arrays
        [WriteOnly] public NativeArray<RaycastCommand> raycastCommands;
        [ReadOnly] public NativeArray<float3> enemyPositions;
        [ReadOnly] public NativeArray<float> enemyDetectionRadiuses;

        // Queries
        [ReadOnly] public QueryParameters obstructionAndPlayerQuery;

        // Ian Note: Unity's burst and job system block writing to indexes other than the one
        // running withing Execute, so I have to run this per raycast command and not per enemy..

        public void Execute(int raycastIndex)
        {
            // Get the enemy index based on the raycast index
            int enemyIndex = raycastIndex / 3; // Every 3 raycasts belong to the same enemy

            // Set up the "up" vector for cross product later
            float3 upVector = new float3(0f, 1f, 0f);

            // Compute direction to center of player
            float3 dirCenter = math.normalize(playerPos - enemyPositions[enemyIndex]);

            // Cross product to create two points to the sides of the player
            float3 crossVect = math.cross(dirCenter, upVector);

            // Determine which raycast this is (center, point1, or point2)
            if (raycastIndex % 3 == 0)
            {
                // To center
                raycastCommands[raycastIndex] = new RaycastCommand(enemyPositions[enemyIndex], dirCenter, obstructionAndPlayerQuery, enemyDetectionRadiuses[enemyIndex]);
            }
            else if (raycastIndex % 3 == 1)
            {
                // To point 1
                float3 point1 = playerPos + playerCapsuleRadius * crossVect;
                float3 dirPoint1 = math.normalize(point1 - enemyPositions[enemyIndex]);
                raycastCommands[raycastIndex] = new RaycastCommand(enemyPositions[enemyIndex], dirPoint1, obstructionAndPlayerQuery, enemyDetectionRadiuses[enemyIndex]);
            }
            else if (raycastIndex % 3 == 2)
            {
                // To point 2
                float3 point2 = playerPos + (-playerCapsuleRadius) * crossVect;
                float3 dirPoint2 = math.normalize(point2 - enemyPositions[enemyIndex]);
                raycastCommands[raycastIndex] = new RaycastCommand(enemyPositions[enemyIndex], dirPoint2, obstructionAndPlayerQuery, enemyDetectionRadiuses[enemyIndex]);
            }
        }
    }


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
        [ReadOnly] public NativeArray<RaycastHit> raycastResults;
        [WriteOnly] public NativeArray<bool> raycastFoundPlayer;
        public int castPerEnemy;
        public int playerColliderInstanceID;

        public void Execute(int index)
        {
            // Iterate on the results of the array starting from index * castPerEnemy,
            // up to index + castPerEnemy
            // If we find a collider with an instance ID of 0, stop
            int startIdxOnResultArr = index * castPerEnemy;
            for (int i = startIdxOnResultArr; i < startIdxOnResultArr + castPerEnemy; i++)
            {
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

