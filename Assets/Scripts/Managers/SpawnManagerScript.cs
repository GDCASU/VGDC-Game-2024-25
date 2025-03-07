using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Written by Matthew Glos
 * 
 * 
 * Controls all the spawn events in a room
 * 
 * 
 */
public class SpawnManagerScript : MonoBehaviour
{
    public List<GameObject> activeRooms = new List<GameObject>();

    private void Update()
    {
        BasicSpawnProcedure();
    }

    private void BasicSpawnProcedure()
    {
        foreach (GameObject ar in activeRooms)
        {
            RoomDataScript rd = ar.GetComponent<RoomDataScript>();

            // Decrement timer
            if (rd.spawnTimer >= 0)
            {
                rd.spawnTimer -= Time.deltaTime;
            }

            // Skip if spawn timer has not elapsed
            if (rd.spawnTimer >= 0) continue;

            // Reset the spawn timer
            rd.spawnTimer = UnityEngine.Random.Range(
                rd.spawnrateLowerLimit,
                rd.spawnrateUpperLimit
            );

            // Skip if the room's spawn cap has been reached
            if (rd.activePool.Count >= rd.spawnCap) continue;

            // Skip if there are no remaining enemies to spawn
            if (rd.remainingEnemyQueue.Count == 0) continue;

            // Get a random spawn point and spawn the enemy
            GameObject spawnPoint = rd.dependantSpawnerPool[
                UnityEngine.Random.Range(0, rd.dependantSpawnerPool.Count)
            ];

            SpawnEnemy(rd, ar, spawnPoint);
        }
    }

    public void spawnAtEveryDependantSpawner(RoomDataScript rd)
    {
        foreach (GameObject ds in rd.dependantSpawnerPool)
        {
            // Skip if there are no remaining enemies to spawn
            if (rd.remainingEnemyQueue.Count == 0) return;

            SpawnEnemy(rd, rd.gameObject, ds);
        }
    }

    private void SpawnEnemy(RoomDataScript rd, GameObject room, GameObject spawnPoint)
    {
        if (rd.remainingEnemyQueue.Count == 0) return;

        ObjectInfo chosenObject = rd.remainingEnemyQueue[
            UnityEngine.Random.Range(0, rd.remainingEnemyQueue.Count)
        ];

        GameObject inst = Instantiate(
            chosenObject.prefab,
            spawnPoint.transform.position,
            Quaternion.identity
        );

        // Set tags if applicable
        var movementScript = inst.GetComponent<DebugEnemyMovementScript>();
        if (movementScript != null)
        {
            movementScript.setTags(chosenObject.tags);
        }

        // Ensure the instantiated object has an EntityRoomConnector component
        var connector = inst.GetComponent<EntityRoomConnector>();
        if (connector == null)
        {
            connector = inst.AddComponent<EntityRoomConnector>();
        }
        connector.setRoom(room);

        // Add this enemy to the room's active list
        rd.AddEnemy(inst);

        // Remove the object from remaining queue if not infinite spawn
        if (!chosenObject.infiniteSpawn)
        {
            rd.remainingEnemyQueue.Remove(chosenObject);
        }
    }

    public void addActiveRoom(GameObject room)
    {
        activeRooms.Add(room);
    }

    public void removeActiveRoom(GameObject room)
    {
        activeRooms.Remove(room);
    }
}
