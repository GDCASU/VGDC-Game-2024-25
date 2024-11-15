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
     * Create a scene manager to manage transitions between scenes
     */// --------------------------------------------------------

    bool[] unlockedLevels;

    public Animator colorDip;
    public float transitionTime = 0.2f;

    /// <summary> temp variables to act as placeholders for scene testing </summary>
    public bool loadNextLevel;
    public bool loadPreviousLevel;
    public bool reloadLevel;
    public bool unlockNextLevel;

    public Vector3 tempStart;

    private void Start()
    {
        unlockedLevels = new bool[UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings]; // number of scenes in build
        unlockedLevels[0] = true; // menu
        unlockedLevels[1] = true; // level one

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (loadNextLevel) { LoadNextLevel(tempStart); }
        if (loadPreviousLevel) { LoadPreviousLevel(tempStart); }
        if (reloadLevel) { ReloadLevel(tempStart); }
        if (unlockNextLevel) { UnlockNextLevel(); }
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
        if (unlockedLevels[loadScene.buildIndex]) { StartCoroutine(ChangeScene(loadScene.buildIndex, startPosition)); }
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

        CharacterController cController = GameObject.Find("Player").GetComponent<CharacterController>();
        cController.enabled = false;
        GameObject.Find("Player").GetComponent<Transform>().position = target + new Vector3(0, 0, -0.5f);
        cController.enabled = true;


        colorDip.SetTrigger("Start");

    }
}
