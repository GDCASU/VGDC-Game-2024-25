using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vase : MonoBehaviour, IDamageable
{
  
    public GameObject waterSpawnPrefab;
    //This script checks to see if a pojectile has hit the vase object
    public void Start()
    {
        Debug.Log("2");
    }
    public ReactionType TakeDamage(int damage, Elements element)
    {
        // Compute damage through multiplier
        Debug.Log("HIT vase ");
        


        GameObject.Destroy(gameObject);


        // Send the element to the status handler and return a reaction if caused
        return ReactionType.Undefined;
    }
    void OnDestroy()
    {
        Instantiate(waterSpawnPrefab, transform.position, Quaternion.identity);
    }
}
