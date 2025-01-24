using UnityEngine;
using UnityEngine.Serialization;

/* -----------------------------------------------------------
 * Author:
 * Jacob Kaufman-Warner
 *
 * Modified By:
 * Ian Fletcher
 */ // --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Make the target object billboard towards the camera
 */ // --------------------------------------------------------

/// <summary>
/// Script used to make an object billboard towards the camera
/// </summary>
public class FaceCamera : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private GameObject targetObject;

    [Header("Lock Roataion")] 
    [SerializeField] private bool lockX;
    [SerializeField] private bool lockY;
    [SerializeField] private bool lockZ;

    // Local variables
    private Quaternion originalRotation;

    private void Awake()
    {
        originalRotation = targetObject.transform.rotation;
    }

    void LateUpdate()
    {
        // Face object toward camera
        if (Camera.main)
        {
            targetObject.transform.rotation = Camera.main.transform.rotation;
        }
        
        //Lock object from rotating in specified direction
        Quaternion rotation = targetObject.transform.rotation;
        if (lockX) rotation.x = originalRotation.x;
        if (lockY) rotation.y = originalRotation.y;
        if (lockZ) rotation.z = originalRotation.z;
        targetObject.transform.rotation = rotation;
    }
}