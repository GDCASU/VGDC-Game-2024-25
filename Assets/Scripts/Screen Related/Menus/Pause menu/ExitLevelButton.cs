using FMOD.Studio;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneName; // scene can be specified in the inspector
    private LevelManager levelManager;

    private void Start()
    {
        levelManager = GameObject.Find("Level Manager").GetComponent<LevelManager>();  
        if (levelManager == null) { Debug.LogWarning("No 'Level Manager' found."); }
    }

    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            LevelManager.Instance.ChangeScene(sceneName, Vector3.zero);
        }
        else
        {
            Debug.LogWarning("Scene name is empty!");
        }
    }
}
