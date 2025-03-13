using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.TimeZoneInfo;
using AYellowpaper.SerializedCollections;

/* -----------------------------------------------------------
 * Author:
 * Cami Lee (Modified version of Scene transition Manager by
 * Matthew Glos)
 *
 * Modified By:
 * Ian Fletcher
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Create a scene manager to manage transitions between scenes -- cannot be a singelton because of stored data
 */// --------------------------------------------------------

// Enumerator to pair levels to their scene
// Make sure the number matches the scene number on the build index
public enum LevelNames
{
    MainMenu = 0,
    Level1 = 1,
    Level2 = 2,
}

public class LevelManager : MonoBehaviour
{
    // Singleton
    public static LevelManager Instance;
    
    [Header("Levels Data")]
    public bool isLevel1Completed = false;
    public bool isLevel2Unlocked = false;
    public bool isLevel2Completed = false;
    
    [Header("Fields")]
    public Animator colorDip;
    public float transitionTime = 0.2f;
    public Vector3 tempStart;
    [SerializeField] private GameObject _playerPrefab;
    
    // Local Variables
    private Coroutine asyncLoadingRoutine = null;

    #region Unity Functions

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
    }

    #endregion
    
    #region Loading Functions
    
    /// <summary> Loads next level and places the player at startPosition </summary>
    public void LoadNextLevel(Vector3 startPosition)
    {
        // FIXME: Had to take unlock checking, Redo
        ChangeScene(SceneManager.GetActiveScene().buildIndex + 1, startPosition);
    }
    
    /* FIXME: Redo this
    /// <summary>
    /// Load scene by specific name and place player in starting position
    /// </summary>
    /// <param name="SceneName"></param>
    /// <param name="target"></param>
    public void LoadSceneByName(string sceneName, Vector3 startPosition)
    {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath(sceneName);
        //Scene loadScene = SceneManager.GetSceneByName(sceneName);

        if (buildIndex == -1) { Debug.LogWarning("Scene '" + sceneName + "' not found in Build Settings."); }
        else if (unlockedLevels[buildIndex]) { StartCoroutine(ChangeScene(buildIndex, startPosition)); }
        else { Debug.Log("The current level is locked"); }
    }
    */

    /// <summary> Loads previous level / menu and places the player at startPosition </summary>
    public void LoadPreviousLevel(Vector3 startPosition)
    {
        ChangeScene(SceneManager.GetActiveScene().buildIndex - 1, startPosition);
    }

    /// <summary> Reloads the current level and places the player at startPosition </summary>
    public void ReloadLevel(Vector3 startPosition)
    {
        ChangeScene(SceneManager.GetActiveScene().buildIndex, startPosition);
    }

    #endregion

    #region Unlocking Levels

    /// <summary> Unlocks the level after the current level </summary>
    public void UnlockNextLevel()
    {
        // FIXME: Do level unlocking later on
        // unlockedLevels[SceneManager.GetActiveScene().buildIndex + 1] = true;
    }

    #endregion

    #region String Scene Changers

    /// <summary>
    /// Changes the scene to one specified by the targetScene string argument
    /// </summary>
    /// <param name="targetScene">Name of the scene in the project</param>
    public void ChangeScene(string targetScene)
    {
        // Dont execute if already loading
        if (asyncLoadingRoutine != null) return;
        asyncLoadingRoutine = StartCoroutine(ChangeSceneRoutine(targetScene));
    }

    /// <summary>
    /// Changes the scene to one specified by the targetScene string argument
    /// and places player at the vector target
    /// </summary>
    /// <param name="targetScene"> The name of the scene in the project </param>
    /// <param name="target"> the place to spawn the player at </param>
    public void ChangeScene(string targetScene, Vector3 target)
    {
        // Dont execute if already loading
        if (asyncLoadingRoutine != null) return;
        asyncLoadingRoutine = StartCoroutine(ChangeSceneRoutine(targetScene, target));
    }

    #endregion

    #region Build Index Scene Changers

    /// <summary>
    /// Changes the scene to the specified one via the buildIndex integer
    /// </summary>
    /// <param name="sceneIndex"> the index of the scene in build settings </param>
    public void ChangeScene(int sceneIndex)
    {
        // Dont execute if already loading
        if (asyncLoadingRoutine != null) return;
        asyncLoadingRoutine = StartCoroutine(ChangeSceneRoutine(sceneIndex));
    }

    /// <summary>
    /// Chanes the scene to the build index and places the player at the target vector
    /// </summary>
    /// <param name="sceneIndex"> The index of the scene in the build settings </param>
    /// <param name="target"> The location to place the player at when loaded </param>
    public void ChangeScene(int sceneIndex, Vector3 target)
    {
        // Dont execute if already loading
        if (asyncLoadingRoutine != null) return;
        asyncLoadingRoutine = StartCoroutine(ChangeSceneRoutine(sceneIndex, target));
    }

    #endregion

    #region Enumerator Scene Changers

    /// <summary>
    /// Changes the scene to the one specified by the enumerator
    /// <para> honestly this is just another way of changing through build index lel </para>
    /// </summary>
    /// <param name="level"> The enum of the target level </param>
    public void ChangeScene(LevelNames level)
    {
        // Dont execute if already loading
        if (asyncLoadingRoutine != null) return;
        asyncLoadingRoutine = StartCoroutine(ChangeSceneRoutine((int)level));
    }

    /// <summary>
    /// Changes the scene to the one specified by the enumerator and places the player at the vector target
    /// <para> honestly this is just another way of changing through build index lel </para>
    /// </summary>
    /// <param name="level"> The enum of the target level </param>
    /// /// <param name="target"> The position to place the player at </param>
    public void ChangeScene(LevelNames level, Vector3 target)
    {
        // Dont execute if already loading
        if (asyncLoadingRoutine != null) return;
        asyncLoadingRoutine = StartCoroutine(ChangeSceneRoutine((int)level, target));
    }

    #endregion

    #region Scene Routines

    private IEnumerator ChangeSceneRoutine(string targetScene)
    {
        AsyncOperation aSync = SceneManager.LoadSceneAsync(targetScene);
        while (!aSync.isDone)
        {
            yield return null;
        }
        // Null the coroutine field when finished
        asyncLoadingRoutine = null;
    }

    private IEnumerator ChangeSceneRoutine(int sceneIndex)
    {
        AsyncOperation aSync = SceneManager.LoadSceneAsync(sceneIndex);
        while (!aSync.isDone)
        {
            yield return null;
        }
        // Null the coroutine field when finished
        asyncLoadingRoutine = null;
    }

    /// <summary> Change Scene coroutine: starts fade </summary>
    private IEnumerator ChangeSceneRoutine(int sceneIndex, Vector3 target)
    {
        colorDip.SetTrigger("End");
        yield return new WaitForSeconds(transitionTime);
        AsyncOperation aSync = SceneManager.LoadSceneAsync(sceneIndex);

        while (!aSync.isDone)
        {
            yield return null;
        }

        // Create new player
        GameObject oldPlayer = GameObject.Find(TagDefinitions.PlayerObjectName);
        GameObject newPlayer = Instantiate(_playerPrefab);
        newPlayer.transform.position = target + new Vector3(0, 0, -0.5f);

        // Old player is not referenced anymore, so now it can be destroyed
        Destroy(oldPlayer);

        colorDip.SetTrigger("Start");

        // Null the coroutine field when finished
        asyncLoadingRoutine = null;
    }

    private IEnumerator ChangeSceneRoutine(string targetScene, Vector3 target)
    {
        colorDip.SetTrigger("End");
        yield return new WaitForSeconds(transitionTime);
        AsyncOperation aSync = SceneManager.LoadSceneAsync(targetScene);

        while (!aSync.isDone)
        {
            yield return null;
        }

        // Create new player
        GameObject oldPlayer = GameObject.Find(TagDefinitions.PlayerObjectName);
        GameObject newPlayer = Instantiate(_playerPrefab);
        newPlayer.transform.position = target + new Vector3(0, 0, -0.5f);

        // Old player is not referenced anymore, so now it can be destroyed
        Destroy(oldPlayer);

        colorDip.SetTrigger("Start");

        // Null the coroutine field when finished
        asyncLoadingRoutine = null;
    }

    #endregion



    // Function to quit the game
    public void QuitGame()
    {
        // FIXME: If we do implement saving of some kind, we need to call a save event here
        Application.Quit();
    }
}
