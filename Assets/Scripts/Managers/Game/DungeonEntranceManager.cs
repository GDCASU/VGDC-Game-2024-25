using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] public GameObject player;
    [SerializeField] public string targetSceneName;
    [SerializeField] public Vector3 targetPosition;
    [SerializeField] public bool isOpen = true;

    private Scene_Transition_Script sceneManager;

    private void Awake()
    {
        sceneManager = GameObject.Find("Scene_Transition_Manager").GetComponent<Scene_Transition_Script>();
        open();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && isOpen) {
            sceneManager.LoadSceneByName(targetSceneName,targetPosition);
        }
    }

    public void close() {
        isOpen = false;
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        sprites[1].enabled = true;
        sprites[0].enabled = false;
    }

    public void open() {
        isOpen = true;
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        sprites[1].enabled = false;
        sprites[0].enabled = true;
    }

    private void Update()
    {
        if (isOpen){
            open();
        } else
        {
            close();
        }
    }


}
