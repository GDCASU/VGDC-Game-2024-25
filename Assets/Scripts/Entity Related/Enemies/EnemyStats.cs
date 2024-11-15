using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField]
    float enemyHealth, enemyMaxHealth = 100;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyHealth = enemyMaxHealth; 

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
