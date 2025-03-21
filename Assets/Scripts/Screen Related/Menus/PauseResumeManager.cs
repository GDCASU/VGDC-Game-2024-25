using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Handle the resume and pausing of the pause menu
 */// --------------------------------------------------------


/// <summary>
/// Class that handles pausing and resuming the game from the pause menu
/// </summary>
public class PauseResumeManager : MonoBehaviour
{
    [Header("Events")]
    // These events are only meant to be triggered by the pause key, not the UI
    [SerializeField] private UnityEvent onPause; 
    [SerializeField] private UnityEvent onResume;
    
    
    void Start()
    {
        // Listen to the pause input of the player
        InputManager.OnPause += HandlePauseInput;
    }

    private void OnDestroy()
    {
        // Stop listening if destroyed
        InputManager.OnPause -= HandlePauseInput;
    }


    private void HandlePauseInput()
    {
        if (Time.timeScale <= 0)
        {
            // Game was paused, call resume
            onResume.Invoke();
            Time.timeScale = 1f;
        }
        else
        {
            // Game was running, call pause
            onPause.Invoke();
            Time.timeScale = 0f;
        }
    }
    
    /// <summary>
    /// Function that pauses the game via timeScale
    /// </summary>
    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    /// <summary>
    /// Function that resumes the game via timeScale
    /// </summary>
    public void PauseMenuResumeButton()
    {
        Time.timeScale = 1;
    }
}
