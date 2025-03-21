
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/**
 * Written by Matthew Glos
 * 
 * A very basic debug enemy made to test the spawnmanager mechanics to enure they are behaving correctly 
 * 
 * 
 */

public class DebugEnemyMovementScript : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private string label;

    private Transform playerTransform;
    private Transform cameraTransform;
    private TextMeshPro textMeshPro;

    public List<string> tags = new List<string>();

    public DebugEnemyMovementScript Init(int x, int y)
    {
        return this;
    }

    private void Start()
    {
        // Cache references to player and camera transforms
        playerTransform = GameObject.Find("Player").transform;
        cameraTransform = GameObject.Find("Main Camera").transform;

        // Get TextMeshPro component in children and set label text
        textMeshPro = GetComponentInChildren<TextMeshPro>();
        if (textMeshPro != null)
        {
            textMeshPro.text = label;
        }


    }

    private void Update()
    {
        // Rotate to face the player
        transform.LookAt(playerTransform.position);

        // Move towards the player
        transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);

        // Destroy enemy if too close to the player
        if (Vector3.Distance(transform.position, playerTransform.position) < 0.5f)
        {
            DestroySelf();
        }

        // Make label face the camera
        if (textMeshPro != null)
        {
            textMeshPro.transform.rotation = cameraTransform.rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

    //removes self from the room before destroying self
    private void DestroySelf()
    {
        
        Destroy(gameObject);
    }

    //if the spawnpool has any tags, handle how youd like them to be used here
    public void setTags(List<string> t)
    {
        tags = t;

        foreach (string tag in tags)
        {
            string key = tag.Split(':')[0];
            string value = tag.Split(':')[1];

            switch (key)
            {
                case "name":
                    label = value;
                    break;
                case "height":
                    transform.localScale = new Vector3(1, float.Parse(value), 1);
                    break;
            }

        }

    }


}