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

    private Interactions interactions;
    private SpriteRenderer[] sprites;
    private GameObject[] spriteObjects;

    private void Awake()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
        spriteObjects = new GameObject[2] { sprites[0].gameObject, sprites[1].gameObject };
        interactions = GetComponent<Interactions>();
        interactions.ChangeInteraction(EnterDungeon);
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
