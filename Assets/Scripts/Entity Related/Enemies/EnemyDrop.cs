using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Davyd Yehudin
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Making entities drop items on death, while adding a random force to them on spawn
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>

//class for help, maybe replacable. name is a WIP, change to a better one
[System.Serializable]
public class dropStuff{
    public GameObject dropObj;
    public int minDropAmount;
    public int maxDropAmount;
    public float dropChance;
}

public class EnemyDrop : MonoBehaviour
{
    
    public float dropForceMult = 2f;
    [SerializeField]
    public List<dropStuff> dropList;
    [SerializeField]private bool doDebugLog;

    //instantiates the drops, tries to launch them NEEDS SMALL CHANGES SO THAT ITEMS DON'T GO TO OUTER SPACE
    void drop(GameObject dropObj){
        UnityEngine.Vector3 summonPos = new UnityEngine.Vector3(this.GetComponent<Transform>().position.x, this.GetComponent<Transform>().position.y, this.GetComponent<Transform>().position.z);
        GameObject a = Instantiate(dropObj, this.GetComponent<Transform>().position, UnityEngine.Quaternion.identity);
        UnityEngine.Vector3 randomForce = new UnityEngine.Vector3(UnityEngine.Random.Range(-1f,1f),UnityEngine.Random.Range(2f,3f),UnityEngine.Random.Range(-1f, 1f));
        a.GetComponent<Rigidbody>().AddForce(randomForce * dropForceMult);
    }

    //goes through each drop and its chance, calls drop on success
    public void onDeath(){
        foreach(dropStuff i in dropList){
            if(doDebugLog) Debug.Log("PepeGotToSpawnChance");
            if(UnityEngine.Random.Range(0f,100f) <= i.dropChance){
                if(doDebugLog) Debug.Log("PepeSummon");
                int a = UnityEngine.Random.Range(i.minDropAmount, i.maxDropAmount);
                for(int j = 0; j < a; j++) drop(i.dropObj);
            }
        }
    }

    //For testing

    void Start(){
        StartCoroutine(Testing());
        if(doDebugLog) Debug.Log("PepeStart");
    }

    IEnumerator Testing(){
        while(true){
        onDeath();
        yield return new WaitForSeconds(2);
        if(doDebugLog) Debug.Log("PepeAgain");
        }
    }

    
}
