using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

public class FlytrapArmControl : MonoBehaviour
{
    private const float MIN_ATTACK_CD = 1.5f;
    private const float MAX_ATTACK_CD = 3f;
    private const float TURN_SPEED = 15f;
    private const float MAX_ANGLE_OFFSET = 45f;
	private const float ATTACK_ANGLE = 15f;
	private const float ATTACK_AGGRO_RANGE = 8f;
	private const float ATTACK_DISTANCE = 4f;
	private const float ATTACK_ANTIC_TIME = 2f;
	private const float ATTACK_TIME = 0.3f;
	private const float ATTACK_RECOVER_TIME = 1f;

	private float _initialAngle;
	private float _attackCooldown = 0f;
	private bool _attacking = false;
	private bool _skipAntic = false;

    private LineRenderer _lineRenderer;
    private GameObject _hand;
    private Animator _handAnim;
	private Collider _hitbox;

    private bool doDebugLog = false;

    private void Start()
    {
        _initialAngle = transform.rotation.eulerAngles.y;
		_lineRenderer = transform.Find("Vine").GetComponent<LineRenderer>();
		_hand = transform.Find("Hand").gameObject;
		_handAnim = _hand.GetComponent<Animator>();
		_hitbox = _hand.GetComponent<Collider>();
	}

	private void Update()
	{
		// Head follows player movements
		Vector3 target = PlayerObject.Instance.transform.position;
		Vector3 offset = target - transform.position;
		offset.y = 0f;
		float startAngle = transform.rotation.eulerAngles.y;
		float targetAngle = Vector3.SignedAngle(offset, Vector3.back, Vector3.down);

		// Normalize angle between -180f and 180f
		float difference = Mathf.Rad2Deg * Mathf.Asin(Mathf.Sin(Mathf.Deg2Rad * (targetAngle - startAngle)));
		float turnAmount = Mathf.Sign(difference) * TURN_SPEED * Time.deltaTime;
		float newAngle = startAngle + turnAmount;
		if(Mathf.Abs(turnAmount) > Mathf.Abs(difference))
		{
			newAngle = startAngle + difference;
		}

		// Clamp turn angle to within 45f of starting rotation
		float differenceFromInitial = Mathf.Rad2Deg * Mathf.Asin(Mathf.Sin(Mathf.Deg2Rad * (newAngle - _initialAngle)));
		if(Mathf.Abs(differenceFromInitial) < MAX_ANGLE_OFFSET)
		{
			transform.rotation = Quaternion.Euler(0f, newAngle, 0f);
		}

		// Attack if player is in range
		if (doDebugLog) Debug.Log(offset.magnitude);
		_attackCooldown -= Time.deltaTime;
		if(!_attacking && _attackCooldown < 0f && Mathf.Abs(difference) < ATTACK_ANGLE && offset.magnitude < ATTACK_AGGRO_RANGE)
		{
			StartCoroutine(Attack());
		}

		// Force vine to follow position of head
		_lineRenderer.SetPosition(1, _hand.transform.localPosition);
	}

	private IEnumerator Attack()
    {
		_attacking = true;
		float x = _hand.transform.localPosition.x;
		float y = _hand.transform.localPosition.y;

		// Play antic (rear back)
		_handAnim.speed = 0.3f;
		float timer = 0f;
		if(!_skipAntic)
		{
			while(timer < ATTACK_ANTIC_TIME)
			{
				_hand.transform.localPosition = new Vector3(x, y, Mathf.Lerp(-1f, 0f, timer / ATTACK_ANTIC_TIME));
				timer += Time.deltaTime;
				yield return null;
			}
		}

		// Lunge towards player
		_handAnim.speed = 1f;
		_handAnim.Play("snap");
		_hitbox.enabled = true;
		float z = _hand.transform.localPosition.z;
		timer = 0f;
		while(timer < ATTACK_TIME)
		{
			_hand.transform.localPosition = new Vector3(x, y, Mathf.Lerp(z, -ATTACK_DISTANCE, timer / ATTACK_TIME));
			timer += Time.deltaTime;
			yield return null;
		}

		// Wait for animation to finish
		yield return new WaitWhile(() => _handAnim.IsPlaying("snap"));

		// Return to starting position
		_handAnim.Play("idle");
		_hitbox.enabled = false;
		timer = 0f;
		while(timer < ATTACK_RECOVER_TIME)
		{
			_hand.transform.localPosition = new Vector3(x, y, Mathf.Lerp(-ATTACK_DISTANCE, -1f, timer / ATTACK_RECOVER_TIME));
			timer += Time.deltaTime;
			yield return null;
		}

		// Begin attack cooldown
		_attackCooldown = Random.Range(MIN_ATTACK_CD, MAX_ATTACK_CD);
		_attacking = false;
	}

	private void OnTriggerStay(Collider other)
	{
		// Instantly attack if player walks into the head
		if(other.CompareTag("Player"))
		{
			_skipAntic = true;
			_attackCooldown = -1f;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			_skipAntic = false;
		}
	}
}
