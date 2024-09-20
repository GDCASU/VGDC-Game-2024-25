using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] SpriteRenderer spriteRenderer;

    [Header("Lock Roataion")]
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    private Vector3 originalRotation;

    private void Awake(){
        originalRotation = spriteRenderer.transform.rotation.eulerAngles;
    }

    void LateUpdate(){
        //Face object toward camera
        spriteRenderer.transform.LookAt(mainCamera.transform.position, Vector3.up);

        //Lock object from rotating in specified direction
        Vector3 rotation = spriteRenderer.transform.rotation.eulerAngles;
        if (lockX) {rotation.x = originalRotation.x;}
        if (lockY) {rotation.x = originalRotation.x;}
        if (lockZ) {rotation.x = originalRotation.x;} 
        spriteRenderer.transform.rotation = Quaternion.Euler(rotation);
    }
}
