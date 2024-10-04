using System.Collections;
using System.Collections.Generic;
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
 * Destroy things
 */// --------------------------------------------------------


/// <summary>
/// Destroy an object after a given amount of seconds
/// </summary>
public class TimeToDestroy : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    [SerializeField] private int seconds;
    
    // Start is called before the first frame update
    void Start(){
        StartCoroutine(Testing());
        if(doDebugLog) Debug.Log("PepeStart");
    }

    IEnumerator Testing(){
        if(doDebugLog) Debug.Log("Start Coroutine");
        yield return new WaitForSeconds(seconds);
        if(doDebugLog) Debug.Log("DESTROOOOOOOOOOY");
        Destroy(this.gameObject);
    }

    
}
