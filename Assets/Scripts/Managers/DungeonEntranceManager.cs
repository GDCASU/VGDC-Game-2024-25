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
 * 
 * Modified By:
 * Cami Lee
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
    [SerializeField] SpriteRenderer openSprite, closedSprite;

    //private values to keep track of this dungeon entrance's state
    private Scene_Transition_Script sceneManager;

    [SerializeField] Interactions interactions;


    private void Awake()
    {
        sceneManager = GameObject.Find("Scene_Transition_Manager").GetComponent<Scene_Transition_Script>();
        open();
    }

    void Enter() { 
        //loads target scene only when the key E is pressed, the player is in proximity, and the door is open
        sceneManager.LoadSceneByName(targetSceneName, targetPosition);
    }

    void Closed()
    {
        Debug.Log("The door is locked");
    }

    //handles visual update of the dungeon entrance
    public void close() {
        interactions.ChangeInteraction(Closed);
        isOpen = false;
        closedSprite.enabled = true;
        openSprite.enabled = false;
    }

    //handles visual update of the dungeon entrance
    public void open() {
        interactions.ChangeInteraction(Enter);
        isOpen = true;
        closedSprite.enabled = false;
        openSprite.enabled = true;
    }

    private void Update()
    {
        //actively checks and updates if the publicly accessible bool isOpen changes
        if (isOpen) { open(); }
        else { close(); }
    }
}
