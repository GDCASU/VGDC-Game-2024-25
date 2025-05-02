using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneDoor : Interactable
{
    public int level;
    private void Start()
    {
        OnInteractionExecuted += EndLevel;
    }

    public void EndLevel()
    {
        if (level == 0) { SceneManager.LoadScene(2); }
        if (level == 1) { SceneManager.LoadScene(0); }
        if (level == 2) { SceneManager.LoadScene(4); }
    }
}
