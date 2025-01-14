using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.TimeZoneInfo;

public class LevelManager : MonoBehaviour
{
    /* -----------------------------------------------------------
    * Author:
     * Cami Lee (Modified version of Scene transition Manager by
     * Matthew Glos)
     * 
     * Modified By:
     * 
     */// --------------------------------------------------------

    /* -----------------------------------------------------------
     * Pupose:
     * Create a scene manager to manage transitions between scenes -- cannot be a singelton because of stored data
     */// --------------------------------------------------------

    bool[] unlockedLevels;

    public Animator colorDip;
    public float transitionTime = 0.2f;

    public Vector3 tempStart;

    [SerializeField] private GameObject _playerPrefab;

    private void Start()
    {
        unlockedLevels = new bool[UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings]; // number of scenes in build
        unlockedLevels[0] = true; // menu
        unlockedLevels[1] = true; // level one

        DontDestroyOnLoad(gameObject);
    }

    /// <summary> Loads next level if unlocked and places the paleyr at startPosition </summary>
    public void LoadNextLevel(Vector3 startPosition)
    {
        if (unlockedLevels[SceneManager.GetActiveScene().buildIndex]) {
            StartCoroutine(ChangeScene(SceneManager.GetActiveScene().buildIndex + 1, startPosition));
        }
        else { Debug.LogError("Level hasn't been unlocked"); }
    }

    /// <summary>
    /// Load scene by specific name and place player in starting position
    /// </summary>
    /// <param name="SceneName"></param>
    /// <param name="target"></param>
    public void LoadSceneByName(string sceneName, Vector3 startPosition)
    {
        Scene loadScene = SceneManager.GetSceneByName(sceneName);

        if (loadScene.buildIndex == -1) { Debug.LogWarning("Scene '" + sceneName + "' not found in Build Settings."); }
        else if (unlockedLevels[loadScene.buildIndex]) { StartCoroutine(ChangeScene(loadScene.buildIndex, startPosition)); }
        else { Debug.Log("The current level is locked"); }
    }

    /// <summary> Loads previous level / menu and places the player at startPosition </summary>
    public void LoadPreviousLevel(Vector3 startPosition)
    {
        StartCoroutine(ChangeScene(SceneManager.GetActiveScene().buildIndex - 1, startPosition));
    }

    /// <summary> Reloads the current level and places the player at startPosition </summary>
    public void ReloadLevel(Vector3 startPosition)
    {
        StartCoroutine(ChangeScene(SceneManager.GetActiveScene().buildIndex, startPosition));
    }

    /// <summary> Unlocks the level after the current level </summary>
    public void UnlockNextLevel()
    {
        unlockedLevels[SceneManager.GetActiveScene().buildIndex + 1] = true;
    }

    /// <summary> Change Scene coroutine: starts fade </summary>
    IEnumerator ChangeScene(int sceneIndex, Vector3 target)
    {
        colorDip.SetTrigger("End");
        yield return new WaitForSeconds(transitionTime);
        AsyncOperation aSync = SceneManager.LoadSceneAsync(sceneIndex);

        while (!aSync.isDone)
        {
            yield return null;
        }

        // Create new player
        GameObject oldPlayer = GameObject.Find("Player");
        GameObject newPlayer = Instantiate(_playerPrefab);
        newPlayer.transform.position = target + new Vector3(0, 0, -0.5f);

        // Old player is not referenced anymore, so now it can be destroyed
        Destroy(oldPlayer);


        colorDip.SetTrigger("Start");

    }
}
