using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    GameObject pauseObject; // menu to be toggled on and off during pause

    // Start is called before the first frame update
    void Start()
    {
        InputManager.OnPause += MenuChange;
        pauseObject = GameObject.Find("PauseComponents");
        pauseObject.SetActive(false);
    }
    /// <summary> Determines if the game should pause or unpause </summary>
    private void MenuChange() 
    {
        if (Time.timeScale == 0) { UnPause(); }
        else { Pause(); }
    }

    /// <summary> Handles all actions when the menu pauses </summary>
    private void Pause()
    {
        Time.timeScale = 0f;
        pauseObject.SetActive(true);
    }
    /// <summary> Handles all actions when the menu unpauses </summary>
    private void UnPause() {  
        Time.timeScale = 1f; 
        pauseObject.SetActive(false);
    }
}
