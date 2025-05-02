using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.Rendering.FilterWindow;

public class FlytrapArmControl : MonoBehaviour
{
    private const float MIN_ATTACK_CD = 3f;
    private const float MAX_ATTACK_CD = 5f;
    private const float TURN_SPEED = 30f;
    private const float ATTACKING_TURN_SPEED = 10f;
	private const float ATTACK_ANGLE = 20f;
	private const float ATTACK_AGGRO_RANGE = 10f;
	private const float ATTACK_DISTANCE = 7f;
	private const float ATTACK_ANTIC_TIME = 2f;
	private const float ATTACK_TIME = 0.6f;
	private const float ATTACK_RECOVER_TIME = 1f;
	private const float DEATH_ANIM_TIME = 3f;

	private float _attackCooldown = 0f;
	private bool _attacking = false;
	private bool _lunging = false;
	private bool _skipAntic = false;
	private bool _dead = false;

    private LineRenderer _lineRenderer;
    private GameObject _hand;
    private Animator _handAnim;
	private Collider _hitbox;

    private bool doDebugLog = false;

    private void Start()
    {
		_lineRenderer = transform.Find("Vine").GetComponent<LineRenderer>();
		_hand = transform.Find("Hand").gameObject;
		_handAnim = _hand.GetComponent<Animator>();
		_hitbox = _hand.GetComponent<Collider>();
	}

	private void Update()
	{
		if(_dead) return;

		// Head follows player movements
		Vector3 target = PlayerObject.Instance.transform.position;
		Vector3 offset = target - transform.position;
		offset.y = 0f;
		float startAngle = transform.rotation.eulerAngles.y;
		float targetAngle = Vector3.SignedAngle(offset, Vector3.back, Vector3.down);

		// Normalize angle between -180f and 180f
		float difference = targetAngle - startAngle;
		if(Mathf.Abs(difference) > 180f) difference -= Mathf.Sign(difference) * 360f;
		float turnAmount = Mathf.Sign(difference) * (_attacking ? ATTACKING_TURN_SPEED : TURN_SPEED) * Time.deltaTime;
		float newAngle = startAngle + turnAmount;
		if(Mathf.Abs(turnAmount) > Mathf.Abs(difference))
		{
			newAngle = startAngle + difference;
		}
		// Only set angle if not in the lunging animation
		if(!_lunging) transform.rotation = Quaternion.Euler(0f, newAngle, 0f);

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

	public IEnumerator Death()
	{
		_dead = true;

		// Slow animation speed to a stop
		float timer = 0f;
		while(timer < DEATH_ANIM_TIME)
		{
			_handAnim.speed = Mathf.Lerp(1f, 0f, timer / DEATH_ANIM_TIME);
			timer += Time.deltaTime;
			yield return null;
		}
		_handAnim.speed = 0f;
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
		_lunging = true;
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
		_lunging = false;

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
