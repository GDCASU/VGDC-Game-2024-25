using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSetReference : MonoBehaviour
{
    void Start()
    {
        PlayerData.Instance.Player = this.gameObject.GetComponent<PlayerObject>();
    }
}
