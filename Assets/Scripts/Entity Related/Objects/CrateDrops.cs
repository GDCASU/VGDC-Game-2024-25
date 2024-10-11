using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateDrops : MonoBehaviour//I swear I will edit this later (needed to look this up)
//UPDATE by Eliza Chook: Made it so that it randomize the no. and type of objects dropped (as long as they have collectible tag, 
//have rigidbody component, and is added in the items array)
//Please edit items array in the crate prefabs

{
    // The items in the box.
    // You assign these items to valid prefabs in the editor.
    // E.g. LettucePreb, BeerPrefab, etc.
    //NOTE: place objects that are collectible and have rigid body components in items so that 
    //they are in the list of items that can be dropped
    public GameObject[] items;//change the value base on how many items meet the requirement

    // Min and max number of items that can be dropped
    public int minDrop = 1;
    public int maxDrop = 3;

    // Set this value to true to cause the box to be destroyed
    // and the items created.
    public bool boxDestroyed = false;

    // Update is called once per frame
    void LateUpdate()
    {
        if (boxDestroyed)
        {
            Vector3 position = transform.position;
            // Determine how many items to drop
            int dropCount = Random.Range(minDrop, maxDrop);

            // Select the items with the "Collectible" tag
            List<GameObject> collectibleItems = new List<GameObject>();
            foreach (GameObject item in items)
            {
                if (item.CompareTag("Collectible"))
                {
                    collectibleItems.Add(item);
                }
            }

            // Drop the determined number of items
            for (int i = 0; i < dropCount; i++)
            {
                if (collectibleItems.Count > 0)
                {
                    // Choose a random collectible item
                    GameObject randomItem = collectibleItems[Random.Range(0, collectibleItems.Count)];

                    // Slightly offset the position for scattered effect
                    Vector3 scatterPosition = position + new Vector3(Random.Range(-0.2f, 0.2f), 0, Random.Range(-0.2f, 0.2f));

                    // Instantiate the random item at the scattered position
                    Instantiate(randomItem, scatterPosition, Quaternion.identity);
                }
            }
            // Get rid of the box.
            Destroy(gameObject);
        }
    }
}
