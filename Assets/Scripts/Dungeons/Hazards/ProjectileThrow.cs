using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * 
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(TrajectoryPredictor))]
public class ProjectileThrow : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private Rigidbody objectToThrow;
    
    [Header("References")]
    [SerializeField] private TrajectoryPredictor trajectoryPredictor;
    
    [Header("Settings")]
    [SerializeField, Range(0.0f, 50.0f)] private float force;
    
    [Header("Optional")]
    [SerializeField] private Transform StartPosition;
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    void OnEnable()
    {
        // Check if a start position was set
        if (StartPosition == null)
            StartPosition = transform;
    }

    void Start()
    {
        // Fire the hazard creator
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            objectToThrow.transform.position = transform.position;
            objectToThrow.AddForce(StartPosition.forward * force, ForceMode.Impulse);
        }
        
        Predict();
    }

    void Predict()
    {
        trajectoryPredictor.PredictTrajectory(ProjectileData());
    }

    ProjectileProperties ProjectileData()
    {
        ProjectileProperties properties = new ProjectileProperties();
        Rigidbody r = objectToThrow.GetComponent<Rigidbody>();

        properties.direction = StartPosition.forward;
        properties.initialPosition = StartPosition.position;
        properties.initialSpeed = force;
        properties.mass = r.mass;
        properties.drag = r.drag;

        return properties;
    }
}
