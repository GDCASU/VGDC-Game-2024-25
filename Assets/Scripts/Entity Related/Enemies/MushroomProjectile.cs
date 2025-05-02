using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomProjectile : MonoBehaviour
{
    [Header("Behavior")]
    [SerializeField] private float _minTimeToTarget = 1.5f;
    [SerializeField] private float _maxTimeToTarget = 2f;
    // Negative value - will determine how fast the projectile falls/how high it goes before starting to fall
    [SerializeField] private float _gravity = -3f;

    [Header("Util")]
    [SerializeField] private LayerMask _groundLayer;

	// The target, set by whatever enemy fires this projectile
	[HideInInspector] public Vector3 target;

    private Rigidbody _rb;
    private Vector3 _startPos;
    private float _timeToTarget;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _startPos = transform.position;
		_timeToTarget = Random.Range(_minTimeToTarget, _maxTimeToTarget);

		// Adjust the target to be the ground; we want the projectile to land on the ground and not directly on the player
        target = new Vector3(target.x, 0f, target.z);

        // Calculate and apply the desired starting velocity
        float xVel = (target.x - _startPos.x) / _timeToTarget;
        float yVel = -0.5f * _gravity * _timeToTarget;
		float zVel = (target.z - _startPos.z) / _timeToTarget;
        _rb.velocity = new Vector3(xVel, yVel, zVel);
	}

	private void FixedUpdate()
	{
        // Apply gravity
        _rb.AddForce(new Vector3(0f, _gravity, 0f), ForceMode.Acceleration);
	}
}
