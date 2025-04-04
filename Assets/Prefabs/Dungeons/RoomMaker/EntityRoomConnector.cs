using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * 
 * Written by Matthew Glos
 * 
 * 
 * Add this to any entity that gets spawned in by the spawnmanager to ensure that entity stores a reference to its 
 * room and is able to remove itself from the room when necessary. 
 * 
 * 
 */
public class EntityRoomConnector : MonoBehaviour
{
    public GameObject room;
    private void OnDestroy()
    {
        room.GetComponent<RoomDataScript>().RemoveEnemy(gameObject);
    }
    public void setRoom(GameObject r)
    {
        room = r;

    }
}
