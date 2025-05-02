using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Matthew Glos
 *
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Controls the behavior of the movable object
 */// --------------------------------------------------------
public class PushableObjectController : MonoBehaviour, IDamageable
{
    [SerializeField] private float pushSpeed = 5f; // Speed of movement
    [SerializeField] private float moveAmount = 1f; // Distance to move
    [SerializeField] private float squishAmount = 0.2f; // Amount to squish/stretch
    [SerializeField] private float wallCheckDistance = 0.6f; // Distance to check for walls
    [SerializeField] private LayerMask wallLayer; // Layer for walls
    [SerializeField] private List<Elements> elementMask; // Layer for elements
    [SerializeField] private GameObject player; //player, moves away from this object

    private Vector3 targetPos;
    private Vector3 originalScale;
    private Vector3 moveDirection;
    private Vector3 squishedScale;

    private bool isMoving = false;

    private void Start()
    {
        targetPos = transform.position;
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * pushSpeed);
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * pushSpeed);

            // Check if the object is close enough to the target position to stop moving
            if (Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                isMoving = false;
                transform.localScale = originalScale;
            }
        }
    }

    public ReactionType TakeDamage(int damage, Elements element, Vector3 dir)
    {
        if (!elementMask.Contains(element)) return ReactionType.Undefined;
        Vector3 hitDirection = GetRelativeDirection(transform.position, player.transform.position);

        // Check if there's a wall behind the cube in the hit direction
        if (IsWallInDirection(-hitDirection))
        {
            moveDirection = hitDirection;
        }
        else
        {
            moveDirection = -hitDirection;
        }

        // Set the target position
        targetPos += moveDirection * moveAmount;
        Squish();
        isMoving = true;

        return ReactionType.Undefined;
    }


    private Vector3 GetRelativeDirection(Vector3 from, Vector3 to)
    {
        Vector3 direction = (to - from).normalized;
        Vector3 localDirection = transform.InverseTransformDirection(direction);

        // Determine the dominant axis (X or Z) and the direction
        if (Mathf.Abs(localDirection.x) > Mathf.Abs(localDirection.z))
        {
            return localDirection.x > 0 ? Vector3.right : Vector3.left;
        }
        else
        {
            return localDirection.z > 0 ? Vector3.forward : Vector3.back;
        }
    }

    private bool IsWallInDirection(Vector3 direction)
    {
        // Perform a raycast in the specified direction to check for walls
        RaycastHit hit;

        Debug.DrawRay(transform.position, direction.normalized*wallCheckDistance, Color.red,5);
        if (Physics.BoxCast(transform.position,new Vector3(.5f,.5f,.5f), direction, Quaternion.identity,wallCheckDistance, wallLayer))
        {
            return true;
        }
        return false;
    }

    private void Squish()
    {
        // Immediately set the scale to a squished or stretched value
        if (moveDirection == Vector3.forward || moveDirection == Vector3.back)
        {
            
            // Stretch along X and squish along Z
            squishedScale = new Vector3(
                originalScale.x + squishAmount,
                originalScale.y,
                originalScale.z - squishAmount
            );
        }
        else if (moveDirection == Vector3.right || moveDirection == Vector3.left)
        {
            // Stretch along Z and squish along X
            squishedScale = new Vector3(
                originalScale.x - squishAmount,
                originalScale.y,
                originalScale.z + squishAmount
            );
        }

        transform.localScale = squishedScale;
    }

    ReactionType IDamageable.TakeDamage(int damage, Elements element)
    {
        throw new System.NotImplementedException();
    }
}