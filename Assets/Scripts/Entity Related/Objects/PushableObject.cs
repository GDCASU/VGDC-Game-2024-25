using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Sameer Reza
 *
 * Modified By:
 *
 */
// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * A pushable object that can be pushed by the player by interaction or via a projectile.
 * Uses a grid manager to track its position and movement.
 */
// --------------------------------------------------------
public class PushableObject : MonoBehaviour
{
    [SerializeField]
    private Interactions interactions;

    [SerializeField]
    private GridManager gridManager;

    [SerializeField]
    private float detectionHeight = 2f;

    [SerializeField]
    private float fallbackRadius = 0.5f;

    private Vector2Int currentGridPosition;
    private Collider objectCollider;
    private BoxCollider detectionVolume;

    private void Start()
    {
        // First check if assigned in inspector
        if (gridManager == null)
        {
            // Then try to find in parent hierarchy
            gridManager = GetComponentInParent<GridManager>();

            // If still null, fall back to scene search
            if (gridManager == null)
            {
                gridManager = FindObjectOfType<GridManager>();
                Debug.LogWarning(
                    $"PushableObject '{gameObject.name}' had to search entire scene for GridManager. Consider assigning directly or placing as child of grid."
                );
                if (gridManager == null)
                {
                    Debug.LogError(
                        $"PushableObject '{gameObject.name}' could not find GridManager in scene or parent hierarchy."
                    );
                }
            }
        }

        currentGridPosition = gridManager.WorldToGridPosition(transform.position);

        // Set up interaction
        if (interactions != null)
        {
            interactions.ChangeInteraction(OnInteract);
        }

        // Get base collider (for size reference)
        objectCollider = GetComponent<Collider>();

        // Create detection volume
        CreateDetectionVolume();

        // Make sure the object can't be pushed by physics if it has a collider - they should just be move by intended interaction
        if (objectCollider != null)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
            }
            rb.isKinematic = true; // Prevents physics from moving the object
            rb.useGravity = false;
        }
    }

    private void CreateDetectionVolume()
    {
        GameObject volumeObj = new GameObject("ProjectileDetectionVolume");
        volumeObj.transform.SetParent(transform);
        volumeObj.transform.localPosition = Vector3.zero;
        detectionVolume = volumeObj.AddComponent<BoxCollider>();
        detectionVolume.isTrigger = true;

        if (objectCollider != null)
        {
            // Match base collider size and extend height
            Bounds bounds = objectCollider.bounds;
            detectionVolume.size = new Vector3(
                bounds.size.x,
                bounds.size.y + detectionHeight,
                bounds.size.z
            );
            // Center the volume above the object
            detectionVolume.center = new Vector3(0, (detectionHeight / 2f), 0);
        }
        else
        {
            // Use fallback size
            detectionVolume.size = new Vector3(
                fallbackRadius * 2f,
                fallbackRadius * 2f + detectionHeight,
                fallbackRadius * 2f
            );
            detectionVolume.center = new Vector3(0, (detectionHeight / 2f), 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        ElementProjectile projectile = other.GetComponent<ElementProjectile>();
        if (projectile != null)
        {
            TryPush(projectile.transform.forward);
        }
    }

    public void OnInteract()
    {
        // Calculate push direction based on player position
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;
        Vector3 pushDirection = (transform.position - playerPos).normalized;
        TryPush(pushDirection);
    }

    public void OnAttackHit(Vector3 attackDirection)
    {
        TryPush(attackDirection);
    }

    private void TryPush(Vector3 direction)
    {
        // Convert 3D direction to grid direction (rounds to nearest cardinal direction)
        Vector2Int gridDirection = new Vector2Int(
            Mathf.RoundToInt(direction.x),
            Mathf.RoundToInt(direction.z)
        );

        Vector2Int targetPosition = currentGridPosition + gridDirection;

        // Check if target position is within grid bounds and unoccupied
        if (
            gridManager.IsValidPosition(targetPosition)
            && !gridManager.IsCellOccupied(targetPosition)
        )
        {
            // Update grid
            gridManager.SetCellOccupied(currentGridPosition, false);
            currentGridPosition = targetPosition;
            gridManager.SetCellOccupied(currentGridPosition, true);

            // Move object
            transform.position = gridManager.GridToWorldPosition(currentGridPosition);
        }
    }
}
