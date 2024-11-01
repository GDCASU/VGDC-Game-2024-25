using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerMovementTemp : MonoBehaviour
{
    public float speed;

    private CharacterController characterController;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //allow users to move along the x and y axis
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        UnityEngine.Vector3 movementDirection = new UnityEngine.Vector3(horizontalInput, 0, verticalInput);

        transform.Translate(movementDirection * speed *  Time.deltaTime);
        float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;

        //prevents characters from passing through walls
        characterController.SimpleMove(movementDirection * magnitude);
    }
}
