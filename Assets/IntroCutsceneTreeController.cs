using UnityEngine;

public class TreeLooper : MonoBehaviour
{
    public float baseSpeed = 10f;
    public float resetPositionX = -15f;
    public float startPositionX = 15f;
    public Camera mainCamera;

    private float parallaxFactor;
    public bool paralaxEnabled = true;
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        float distanceZ = transform.position.z - mainCamera.transform.position.z;
        parallaxFactor = 1f / Mathf.Clamp(distanceZ, 1f, 100f); // Avoid divide by 0 or super high speeds
    }

    void Update()
    {
        // Move the tree left at parallax speed
        if(paralaxEnabled)
        transform.position += Vector3.left * baseSpeed * parallaxFactor * Time.deltaTime;
        else transform.position += Vector3.left * baseSpeed * Time.deltaTime;
        // Loop the tree back if it's offscreen
        if (transform.position.x <= resetPositionX)
        {
            Vector3 pos = transform.position;
            pos.x = startPositionX;
            transform.position = pos;
        }
    }
}
