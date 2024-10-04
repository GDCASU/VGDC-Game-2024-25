using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
   [SerializeField] SpriteRenderer spriteRenderer;

   private Camera mainCamera;

   [Header("Lock Roataion")]
   [SerializeField] private bool lockX;
   [SerializeField] private bool lockY;
   [SerializeField] private bool lockZ;

   private Vector3 originalRotation;

   private void Awake(){
      originalRotation = spriteRenderer.transform.rotation.eulerAngles;
      mainCamera = Camera.main;
   }

   void LateUpdate(){
      //Face object toward camera
      spriteRenderer.transform.rotation = mainCamera.transform.rotation;

      //Lock object from rotating in specified direction
      Vector3 rotation = spriteRenderer.transform.rotation.eulerAngles;
      if (lockX) {rotation.x = originalRotation.x;}
      if (lockY) {rotation.y = originalRotation.y;}
      if (lockZ) {rotation.z = originalRotation.z;} 
      spriteRenderer.transform.rotation = Quaternion.Euler(rotation);
   }
}
