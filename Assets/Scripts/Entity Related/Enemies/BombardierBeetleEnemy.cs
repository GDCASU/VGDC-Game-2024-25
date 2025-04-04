using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Pathfinding;

/* -----------------------------------------------------------
 * Author:
 * Jose Grijalva
/* -----------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Handles the Bombardier Beetle enemy type
 */// --------------------------------------------------------

/*
 * TO DO:
 * - Complete this C# script using the EntityScript.cs for reference.
 * - Add a Capsule Collider 2D to the Bombardier Beetle prefab.
 * - Add other necessary components to the Bombardier Beetle prefab.
 */


public class BombardierBeetleEnemy : MonoBehaviour
{
    [Header("Control")]
    [SerializeField] private float _meleeAttackDistance = 4f; // Assuming 2f is 1 zone. 2f & 2 zones = 4f because the attack
                                                              // range takes up between 2 zones.
    [SerializeField] private float _rangedAttackCooldown = 3f; // Google Doc does not specify the cooldown time for the ranged attack
                                                               // so it is assumed that it is 3f like with the Big Mushroom Guy's cooldown.
    [SerializeField] private float _rangedAttackRadius;



    private AIPath _aiPath;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

/*
public class DamageMultiplierForBombardierBeetle
{
    public float fireMult = 0.3f; // Fire damage is 70% less effective.
    public float neutralMult = 1.5f; // Neutral damage is 50% more effective.
    public float fungalMult = 1f; // Fungal damage is normal.

    public int CalculateDamage(int damage, Element element)
    {
        return 0;
    }
}
*/