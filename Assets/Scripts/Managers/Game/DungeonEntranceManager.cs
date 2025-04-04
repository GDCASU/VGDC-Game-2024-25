using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/* -----------------------------------------------------------
 * Author:
 * Matthew Glos
 *
 * Modified By:
 * Ian Fletcher
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Represents dungeon entrance, waits for player to collide 
 * with it and loads the given scene by name when that happens
 */// --------------------------------------------------------
public class DungeonEntranceManager : Interactable
{
    [SerializeField] public GameObject player;
    [SerializeField] public string targetSceneName;
    [SerializeField] public Vector3 targetPosition;
    [SerializeField] public bool isOpen = true;
    
    private SpriteRenderer[] sprites;
    private GameObject[] spriteObjects;

    private void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
        spriteObjects = new GameObject[2] { sprites[0].gameObject, sprites[1].gameObject };
        OnInteractionExecuted += EnterDungeon;
    }

    void OnDestroy()
    {
        // Unsub from events
        OnInteractionExecuted -= EnterDungeon;
    }

    private void EnterDungeon()
    {
        if (isOpen)
        {
            LevelManager.Instance.ChangeScene(targetSceneName, targetPosition);
        }
    }

    public void close() {
        spriteObjects[0].SetActive(false);
        spriteObjects[1].SetActive(true);
    }

    public void open() {
        spriteObjects[0].SetActive(true);
        spriteObjects[1].SetActive(false);
    }

    private void Update()
    {
        if (isOpen){ open(); } 
        else { close(); }
    }


}
