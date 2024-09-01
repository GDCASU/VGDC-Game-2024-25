using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Stop Unity from deleting the persistent objects on change 
 * of scene.
 */// --------------------------------------------------------


/// <summary>
/// Class that will protect all objects that are meant to be present on all scenes
/// </summary>
public class PersistentObjectsManager : MonoBehaviour
{
    // There's nothing to access here, but DontDestroyOnLoad only works on root
    // Objects, so if we need to nest them, they need to be in an object were
    // DontDestroyOnLoad is called at root
    public static PersistentObjectsManager Instance;

    private void Awake()
    {
        // Handle Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
    }
}
