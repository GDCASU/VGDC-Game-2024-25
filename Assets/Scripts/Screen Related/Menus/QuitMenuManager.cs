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
 * Purpose:
 * Handle the quit button on both the pause and the main menu
 */// --------------------------------------------------------


/// <summary>
/// Class that handles the quit of the game
/// </summary>
public class QuitMenuManager : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    /// <summary>
    /// Function called to quit the game
    /// </summary>
    public void QuitGame()
    {
        // TODO: Add saving progress here
        Application.Quit();
    }

    /// <summary>
    /// Function to quit to the main menu, used by the pause menu
    /// </summary>
    public void QuitToMainMenu()
    {
        LevelManager.Instance.ChangeScene(LevelNames.MainMenu);
    }
}
