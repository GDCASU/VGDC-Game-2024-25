
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


/**
 * Written by Matthew Glos
 * 
 * the roomdata script, keeps track of information about the room including 
 * what type it is, the enemy spawnpool of the room, list of dependant spawners,
 * and active enemies.
 * 
 * provides unity event hooks for basic events like the player entering, leaving, staying
 * and defeating all the enemie in this room. 
 * 
 * 
 */

//container class for info about each object in the spawn pool
[Serializable]
public class ObjectInfo
{
    //prefab to be instantiated
    public GameObject prefab;

    //how many of this enemy can be spawned at a given time
    public int spawnCount;

    //enables infinite spawn. spawnCount will still effect the probablility of this object being selected
    public bool infiniteSpawn;

    //any tags that are applicable
    public List<string> tags = new List<string>();

}

public class RoomDataScript : MonoBehaviour
{
    //enumerating the different types of rooms, does nothing yet
    public enum RoomTypes
    {
        Lockdown,
        Combat,
        SafeRoom
    }


    [Header("RoomData")]
    //enumerates room types
    [SerializeField] public RoomTypes roomType = RoomTypes.Lockdown;

    //spawnpool for this room
    [SerializeField] public List<ObjectInfo> spawnPool = new List<ObjectInfo>();

    //keeps track of the potential spawn locations for this room
    [SerializeField] public List<GameObject> dependantSpawnerPool = new List<GameObject>();

    //max number of simoultaneous enemies in this room
    [SerializeField] public int spawnCap;

    [SerializeField] public float spawnrateLowerLimit, spawnrateUpperLimit;
    [NonSerialized] public float spawnTimer;

    [Header("Spawning Data, Dont Edit This")]
    //keeps track of how many of each enemy type the room still has to spawn
    [NonSerialized] public List<ObjectInfo> remainingEnemyQueue = new List<ObjectInfo>();

    //keeps track of the list of enemies that are active in the scene
    [NonSerialized]public List<GameObject> activePool = new List<GameObject>();

    [Header("PlayerInteraction")]

    //events that occur when the player enters the room
    [SerializeField] UnityEvent playerEnterEventsWhenNotCleared = new UnityEvent();
    [SerializeField] UnityEvent playerEnterEventsWhenCleared = new UnityEvent();
    [SerializeField] UnityEvent playerStayEventsWhenNotCleared = new UnityEvent();
    [SerializeField] UnityEvent playerStayEventsWhenCleared = new UnityEvent();
    [SerializeField] UnityEvent playerLeaveEventsWhenNotCleared = new UnityEvent();
    [SerializeField] UnityEvent playerLeaveEventsWhenCleared = new UnityEvent();

    [SerializeField] UnityEvent roomClearedEvents = new UnityEvent();


    private bool roomCleared = false;

    private void Start()
    {
        //initialize the list with the values derived from spawnpool
        foreach (var item in spawnPool)
        {
            for (int i = 0; i < item.spawnCount; i++)
                remainingEnemyQueue.Add(item);
        }

        spawnTimer = spawnrateUpperLimit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player" && other.GetType() == typeof(CharacterController))
        {
            if (roomCleared)
            {
                playerEnterEventsWhenCleared.Invoke();
            }
            else
            {
                playerEnterEventsWhenNotCleared.Invoke();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Player" && other.GetType() == typeof(CharacterController))
        {
            if (roomCleared)
            {
                playerStayEventsWhenCleared.Invoke();
            }
            else
            {
                playerStayEventsWhenNotCleared.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player" && other.GetType() == typeof(CharacterController))
        {
            if (roomCleared)
            {
                playerLeaveEventsWhenCleared.Invoke();
            }
            else
            {
                playerLeaveEventsWhenNotCleared.Invoke();
            }
        }
    }

    //when an enemy is created, add it to the active list so the room can keep track of it
    public void AddEnemy(GameObject enemy)
    {
        activePool.Add(enemy);
    }

    //when an enemy is killed, deactivated, etc make sure to remove it from the active list
    public void RemoveEnemy(GameObject enemy)
    {
        activePool.Remove(enemy);

        //if the active pool and remainingpoolofeachtype lists are both empty, this means the player has killed every enemy in the room 
        if (activePool.Count == 0 && remainingEnemyQueue.Count == 0)
        {
            roomCleared = true;
            roomClearedEvents.Invoke();
        }
    }
}