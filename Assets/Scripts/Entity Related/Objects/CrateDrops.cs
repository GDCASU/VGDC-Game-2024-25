using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateDrops : MonoBehaviour//I swear I will edit this later (needed to look this up)
{
    // The items in the box.
    // You assign these items to valid prefabs in the editor.
    // E.g. LettucePreb, BeerPrefab, etc.
    public GameObject[] items = new GameObject[3];

    // Set this value to true to cause the box to be destroyed
    // and the items created.
    public bool boxDestroyed = false;

    // Update is called once per frame
    void LateUpdate () 
    {
        if (boxDestroyed)
        {
            Vector3 position = transform.position;
            // Clone the objects that are "in" the box.
            foreach (GameObject item in items)
            {
                if (item != null)
                {
                    // Add code here to change the position slightly
                    // so the items are scattered a little bit.
                    Instantiate(item, position, Quaternion.identity);
                }
            }
            // Get rid of the box.
            Destroy(gameObject);
        }
    }
}
