using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Script that will create a hazard, honestly needs more work
 */// --------------------------------------------------------


/// <summary>
/// Script used to make hazards upon hit with a projectile
/// </summary>
public class HazardCreator : MonoBehaviour, IDamageable
{
    [Header("References")] 
    [SerializeField] private Transform _hazardSpawnPoint;
    [SerializeField] private GameObject modelToDestroy;
    [SerializeField] private CapsuleCollider capsuleCollider;
    [SerializeField] private GameObject hazardProjectilePrefab;
    [SerializeField] private GameObject hazardTilePrefab;
    
    [Header("Settings")]
    [SerializeField] private int hazardCount = 3; // How many of the hazards to shoot out
    [SerializeField] private int hazardCellCount = 1; // How many of the grid cells to fill with hazard
    [SerializeField, Range(-90f, 90f)] private float verticalAngle = 0f; // Adjusts launch verticality
    [SerializeField, Range(0.0f, 10.0f)] private float force = 5f; 
    [SerializeField, Range(0.0f, 10.0f)] private float mass = 1f; 
    [SerializeField, Range(0.0f, 10.0f)] private float drag = 0f;
    [SerializeField, Range(0.0f, 10.0f)] private float angularDrag = 0.05f;
    
    [Header("Prediciton Settings")]
    [SerializeField, Range(10, 100)] private int maxPoints = 50; // Number of points in trajectory
    [SerializeField, Range(0.01f, 0.5f)] private float increment = 0.025f; // Time step per iteration
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    /// <summary>
    /// Draw on the inspector some green lines to help visualize where the hazards will go
    /// </summary>
    private void OnDrawGizmos()
    {
        if (hazardCount < 1) return;

        Gizmos.color = Color.green;

        float spreadAngle = Mathf.PI * 2 / hazardCount;

        for (int h = 0; h < hazardCount; h++)
        {
            float angleOffset = h * spreadAngle; // Unique horizontal angle per hazard
            Quaternion horizontalRotation = Quaternion.Euler(0, Mathf.Rad2Deg * angleOffset, 0);
            Quaternion verticalRotation = Quaternion.Euler(verticalAngle, 0, 0);
            Vector3 direction = horizontalRotation * (verticalRotation * _hazardSpawnPoint.forward);
    
            // Calculate initial velocity
            Vector3 velocity = direction * (force / mass);
            
            // Draw the trajectory given computed predictions
            DrawTrajectory(_hazardSpawnPoint.position, velocity);
        }

        // Draw start sphere
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(_hazardSpawnPoint.position, 0.1f);
    }

    private void DrawTrajectory(Vector3 startPos, Vector3 startVelocity)
    {
        Vector3 position = startPos;
        Vector3 velocity = startVelocity;

        for (int i = 0; i < maxPoints; i++)
        {
            Vector3 nextPosition = position + velocity * increment;
            Gizmos.DrawLine(position, nextPosition);

            // Apply physics calculations
            velocity += Physics.gravity * increment; 
            velocity *= Mathf.Clamp01(1f - drag * increment); 

            position = nextPosition;
        }
    }

    /// <summary>
    /// Function so projectiles can break the pot
    /// </summary>
    public ReactionType TakeDamage(int damage, Elements element)
    {
        Destroy(modelToDestroy);
        Destroy(capsuleCollider);
        
        // Spawn hazard pellets
        float spreadAngle = Mathf.PI * 2 / hazardCount;
        for (int h = 0; h < hazardCount; h++)
        {
            float angleOffset = h * spreadAngle;
            Quaternion horizontalRotation = Quaternion.Euler(0, Mathf.Rad2Deg * angleOffset, 0);
            Vector3 direction = horizontalRotation * _hazardSpawnPoint.forward;
            Quaternion hazardRotation = Quaternion.LookRotation(direction);
            GameObject hazardSpawner = Instantiate(hazardProjectilePrefab, _hazardSpawnPoint.position, hazardRotation);
            
            // Set the data
            ProjectileThrow projectileThrow = hazardSpawner.GetComponent<ProjectileThrow>();
            projectileThrow.hazardAmount = hazardCount;
            projectileThrow.force = force;
            projectileThrow.verticalAngle = verticalAngle;
            projectileThrow.objectToThrow.drag = drag;
            projectileThrow.objectToThrow.mass = mass;
            projectileThrow.objectToThrow.angularDrag = angularDrag;
        }
    
        return ReactionType.Undefined;
    }
}
