using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] float speed = 100f;

    void Update()
    {
        if(Input.GetKey(KeyCode.E)){
            transform.Rotate(0, speed * Time.deltaTime, 0);
        } else if(Input.GetKey(KeyCode.Q)){
            transform.Rotate(0, -speed * Time.deltaTime, 0);
        }
    }
}
