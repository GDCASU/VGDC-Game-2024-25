using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Handles the connection between the file manager and the
 * game objects
 */// --------------------------------------------------------

/// <summary>
/// Singleton class to be called for saving and loading
/// </summary>
public class SerializedDataManager : MonoBehaviour
{
    // Singleton
    public static SerializedDataManager instance { get; private set;}

    // Event that will be raised telling objects to start saving if the application is quit
    public static event System.Action StartSavingEvent;

    // Inspector variables
    [Header("Settings")]
    [SerializeField] private bool doDebugLog;
    [SerializeField] private string saveFileName;
    [SerializeField] private string configFileName;

    // Folder Name
    private string saveFolderName = "VGDC2024-25 SaveData";

    // Variables
    [HideInInspector] public bool hasLoaded;
    public GameData gameData { get; private set; }
    public ConfigData configData { get; private set; }
    private FileDataHandler fileDataHandler;
    [HideInInspector] public int objectsWithDataOpened = 0;

    private void Awake()
    {
        // Handle Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Load Data
        fileDataHandler = new FileDataHandler(saveFolderName, saveFileName, configFileName);
        LoadGame();
        hasLoaded = true;
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void NewConfigs()
    {
        this.configData = new ConfigData();
    }

    public void LoadGame()
    {
        // Load any saved data from a file using the data handler
        this.gameData = fileDataHandler.LoadGameData();
        this.configData = fileDataHandler.LoadConfigData();
        
        // if no game data is found, create a new game
        if (this.gameData == null)
        {
            if (doDebugLog) Debug.Log("<color=yellow>No game data found, initializing to default values</color>");
            NewGame();
        }

        // if no config data found, create new ones
        if (this.configData == null)
        {
            if (doDebugLog) Debug.Log("<color=yellow>No config data found, initializing to default values</color>");
            NewConfigs();
        }

        // Debugging
        if (doDebugLog) Debug.Log("Loaded data to objects");
    }

    // On destroy is called after on application quit
    public void OnApplicationQuit()
    {
        // Call the saving event on all objects so they save before writting to file
        StartSavingEvent?.Invoke();
        // Save all data to file once the game quit
        SaveData();
    }

    public void SaveData()
    {
        // Save the data to a file using the data handler
        fileDataHandler.SaveGameData(gameData);
        fileDataHandler.SaveConfigData(configData);
    }

    #region Unlocking Functions
    // Interface functions will reside here, stuff like unlock gatling, cheats or scenes


    #endregion
}
