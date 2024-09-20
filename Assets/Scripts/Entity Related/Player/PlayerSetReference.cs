using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetReference : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerData.Instance.player = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
