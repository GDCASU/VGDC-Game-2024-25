using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Handles the file writting and reading
 */// --------------------------------------------------------

/// <summary>
/// Handler class that will take care of storing the data to a file in the streaming assets folder
/// </summary>
public class FileDataHandler
{
    private string _dataFileName; // File name of the Player Saved Data
    private string _configFileName; // File name for the player's config
    private string _dataFolderPath; // SaveData Folder path

    private string _saveFilePath; // Path to save data file
    private string _configFilePath; // Path to save data file

    public FileDataHandler(string dataFolderName, string dataFileName, string configFileName)
    {
        this._dataFileName = dataFileName;
        this._configFileName = configFileName;

        // Get the path to the save file folder
        string path = ResolveDataPath();
        // Set paths
        this._dataFolderPath = Path.Combine(path, dataFolderName);
        this._saveFilePath = Path.Combine(_dataFolderPath, dataFileName);
        this._configFilePath = Path.Combine(_dataFolderPath, _configFileName);

        try
        {
            // Figure out if the directory already exists
            if (!Directory.Exists(_dataFolderPath))
            {
                // Doesnt exist, create it
                Directory.CreateDirectory(_dataFolderPath);
            }
        }
        catch (Exception e)
        {
            // Failed to create the save directory
            Debug.LogError("Error occured when trying to create the save file directory at: " + _dataFolderPath + "\n" + e);
        }
    }

    /// <summary>
    /// Will load the player's game data from file
    /// </summary>
    public GameData LoadGameData()
    {
        GameData loadedData = null;
        
        // Check if the save data file exists
        if (File.Exists(_saveFilePath))
        {
            try
            {
                // load the save file data from the file
                string dataToLoad;
                using (FileStream stream = new FileStream(_saveFilePath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // De-serialize data from the json file to a GameObject
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load the save file from directory: " + _saveFilePath + "\n" + e);
            }
        }
        
        return loadedData;
    }

    /// <summary>
    /// Will load the player's config data from file
    /// </summary>
    public ConfigData LoadConfigData()
    {
        ConfigData loadedData = null;

        // Check if the configs data file exists
        if (File.Exists(_configFilePath))
        {
            try
            {
                // load the configs file data from the file
                string dataToLoad;
                using (FileStream stream = new FileStream(_configFilePath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // De-serialize data from the json file to a GameObject
                loadedData = JsonUtility.FromJson<ConfigData>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to load the configs file from directory: " + _configFilePath + "\n" + e);
            }
        }

        return loadedData;
    }

    /// <summary>
    /// Saves the game data class to a file
    /// </summary>
    public void SaveGameData(GameData data)
    {
        try
        {
            // Serialize the game data object into json format
            string dataToStore = JsonUtility.ToJson(data, true);

            // Write the serialized data to file
            using (FileStream stream = new FileStream(_saveFilePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            // Something happened when trying to write to the save file
            Debug.LogError("Error occured when trying to save data to file: " + _saveFilePath + "\n" + e);
        }
    }

    /// <summary>
    /// Saves the config data class to a file
    /// </summary>
    public void SaveConfigData(ConfigData data)
    {
        try
        {
            // Serialize the configs data object into json format
            string dataToStore = JsonUtility.ToJson(data, true);

            // Write the serialized data to file
            using (FileStream stream = new FileStream(_configFilePath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            // Something happened when trying to write to the config file
            Debug.LogError("Error occured when trying to save configs to file: " + _configFilePath + "\n" + e);
        }
    }

    // Figure out were to create the serialized files
    private string ResolveDataPath()
    {
        switch (Application.platform)
        {
            // If on the editor, save the data within the asset folder
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.LinuxEditor:
                return Application.dataPath;
            // If on the compiled game, used persistent data path
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.OSXPlayer:
            case RuntimePlatform.LinuxPlayer:
            case RuntimePlatform.WebGLPlayer:
                return Application.persistentDataPath;
        }
        // ERROR: Something happened while getting the save data file
        string msg = "PATH TO SAVED GAMED FAILED TO RESOLVE\n";
        msg += "Error thrown on FileDataHandler.cs";
        throw new Exception(msg);
    }

}
