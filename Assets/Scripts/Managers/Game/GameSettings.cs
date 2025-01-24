using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
 * Set up some global configurations on the game
 */// --------------------------------------------------------

/// <summary>
/// Class that holds the settings of the game
/// </summary>
public class GameSettings : MonoBehaviour, IDataPersistance
{
    public static GameSettings Instance;        // Singleton reference

    [Header("Cursor Settings")]
    [SerializeField] private bool hideCursor;
    [SerializeField] private bool lockCursor;
    [SerializeField] private bool confineCursor;

    [Header("Frame Rate")]
    [SerializeField] private bool capFrameRate;
    [SerializeField] private int targetFrameRate = 60;

    [Header("Cheats")]
    public bool areCheatsUnlocked;
    public bool isPistolLethal;
    public bool isHealth100;

    private void Awake()            
    {
        // Set the Singleton
        if (Instance != null && Instance != this)
        {
            // Already set, destroy this object
            Destroy(gameObject);
            return;
        }
        // Not set yet
        Instance = this;

        // Subscribe to saving events
        SerializedDataManager.StartSavingEvent += SaveData;
    }

    private void Start()
    {
        // Load Data
        LoadData();
    }

    public void SaveData()
    {
        // Save data to file Here

        // Unsubscribe from events
        SerializedDataManager.StartSavingEvent -= SaveData;
    }

    public void LoadData()
    {
        // Load data from configs here

        // Set variables
        if (capFrameRate)
            SetFrameRate(targetFrameRate);

        if (hideCursor)
            HideCursor(hideCursor);

        if (lockCursor)
            LockCursor(lockCursor);

        if (confineCursor)
            ConfineCursor(confineCursor);
    }

    /// <summary>
    /// Sets the target frame rate that the game will run at.
    /// </summary>
    /// <param name="frameRate"> Target frame rate </param>
    public void SetFrameRate(int frameRate)
    {
        Application.targetFrameRate = frameRate;
    }

    public void HideCursor(bool toggle)
    {
        Cursor.visible = !toggle;
    }

    public void LockCursor(bool toggle)
    {
        if (toggle)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    public void ConfineCursor(bool toggle)
    {
        if (toggle)
            Cursor.lockState = CursorLockMode.Confined;
        else
            Cursor.lockState = CursorLockMode.None;
    }
}
