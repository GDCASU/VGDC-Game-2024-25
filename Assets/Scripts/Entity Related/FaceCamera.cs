using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Jacob Kauffman, 
 * 
 * Edited:
 * Matthew Glos
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Attach to any sprite objects that need to be oriented towards the camera
 * allows for locking of each axis, and initial offsets
 */// --------------------------------------------------------
public class FaceCamera : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("Lock Roataion")]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    [SerializeField] private float offset_x;
    [SerializeField] private float offset_y;
    [SerializeField] private float offset_z;

    private Vector3 originalRotation;

    private void Awake(){
        originalRotation = spriteRenderer.transform.rotation.eulerAngles;
    }

    void LateUpdate(){
        //Face object toward camera

        //commented out code here is the old function to orient towards the camera
        //spriteRenderer.transform.LookAt(mainCamera.transform.position, Vector3.up);
        spriteRenderer.transform.rotation = mainCamera.transform.rotation;

        //Lock object from rotating in specified direction
        Vector3 rotation = spriteRenderer.transform.rotation.eulerAngles;


        if (lockX) {rotation.x = originalRotation.x;}
        if (lockY) {rotation.x = originalRotation.x;}
        if (lockZ) {rotation.x = originalRotation.x;}

        rotation.x += offset_x;
        rotation.y += offset_y;
        rotation.z += offset_z;

        spriteRenderer.transform.rotation = Quaternion.Euler(rotation);
    }
}
