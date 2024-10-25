using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSceneChange : MonoBehaviour
{
    [SerializeField] private GameObject _playerPrefab;

    // Called at the beginning and for each scene change
    void Start()
    {
        PlayerData.Instance.Player = this.gameObject.GetComponent<PlayerObject>();

        // When the scene is changed, create a new player and destroy this one
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    private void ChangedActiveScene(Scene current, Scene next) {
        Instantiate(_playerPrefab);
        Destroy(this.gameObject);
    }

    private void OnDestroy() {
        SceneManager.activeSceneChanged -= ChangedActiveScene;
    }
}
