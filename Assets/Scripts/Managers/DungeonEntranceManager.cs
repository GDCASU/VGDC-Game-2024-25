using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
/* -----------------------------------------------------------
 * Author:
 * Matthew Glos
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Represents dungeon entrance, waits for player to collide 
 * with it and loads the given scene by name when that happens
 */// --------------------------------------------------------
public class DungeonEntranceManager : MonoBehaviour
{
    //publicly accessible values for tweaking
    [SerializeField] public GameObject player;
    [SerializeField] public string targetSceneName;
    [SerializeField] public Vector3 targetPosition;
    [SerializeField] public bool isOpen = true;
    [SerializeField] public float lerpAmount;
    [SerializeField] SpriteRenderer openSprite, closedSprite, toolTipSprite;

    //private values to keep track of this dungeon entrance's state
    private bool playerInProximity =false;
    private Scene_Transition_Script sceneManager;
    

    private void Awake()
    {
        sceneManager = GameObject.Find("Scene_Transition_Manager").GetComponent<Scene_Transition_Script>();
        open();
    }

    //Detects when the player is close to the entrance
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && isOpen) { playerInProximity = true; } 
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player && isOpen) { playerInProximity = false; }
    }

    //handles visual update of the dungeon entrance
    public void close() {
        isOpen = false;
        closedSprite.enabled = true;
        openSprite.enabled = false;
    }

    //handles visual update of the dungeon entrance
    public void open() {
        isOpen = true;
        closedSprite.enabled = false;
        openSprite.enabled = true;
    }

    private void Update()
    {
        //actively checks and updates if the publicly accessible bool isOpen changes
        if (isOpen) { open(); }
        else { close(); }



        //basic animation popup for the input tooltip, just lerps between the target scale and current scale
        toolTipSprite.transform.localScale = new Vector3(Mathf.Lerp(toolTipSprite.transform.localScale.x, Convert.ToInt32(playerInProximity)*2, lerpAmount), 2, 2);

        //loads target scene only when the key E is pressed, the player is in proximity, and the door is open
        if (Input.GetKeyDown(KeyCode.E) && playerInProximity && isOpen)
        {
            sceneManager.LoadSceneByName(targetSceneName, targetPosition);
        }
    }


}
