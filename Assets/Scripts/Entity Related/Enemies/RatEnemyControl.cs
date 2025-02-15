using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

/* -----------------------------------------------------------
 * Author: William
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose: Control script for rat enemies
 * 
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>
public class RatEnemyControl : MonoBehaviour
{
	[Header("Control")]
	[SerializeField] private float _minIdleTime = 3f;
	[SerializeField] private float _maxIdleTime = 5f;
	[SerializeField] private float _minRunTime = 0.25f;
	[SerializeField] private float _maxRunTime = 0.75f;
	[SerializeField] private float _attackTime = 0.5f;
	[SerializeField] private float _maxAttackHeight = 0.25f;
	[SerializeField] private float _runSpeed = 3f;

	// Use this bool to gate all your Debug.Log Statements please
	[Header("Debugging")]
	[SerializeField] private bool doDebugLog;

	private Animator _anim;
	private AIPath _aiPath;
	private GameObject _spriteObj;
	private Transform _target;
	private bool _attacking;
	private Vector3 _lookVector;

	void Start()
	{
		_anim = GetComponent<Animator>();
		_aiPath = GetComponent<AIPath>();
		_spriteObj = transform.Find("Sprite").gameObject;
		_target = transform.Find("Target");
		_target.parent = null;

		_aiPath.maxSpeed = _runSpeed;
		_aiPath.maxAcceleration = 999999999f;
		_lookVector = new Vector3(1f, 0f, 0f);
		GetComponent<AIDestinationSetter>().target = _target;
		StartCoroutine(Idle());
	}

	private void Update()
	{
		
	}

	private IEnumerator Idle()
	{
		if(doDebugLog) Debug.Log(gameObject.name + " Idle");

		yield return _anim.PlayBlocking("SideToIdle");

		_anim.Play(Random.Range(0, 2) == 0 ? "Dance" : "Idle");
		yield return new WaitForSeconds(Random.Range(_minIdleTime, _maxIdleTime));

		StartCoroutine(Wander());
	}

	private IEnumerator Wander()
	{
		if(doDebugLog) Debug.Log(gameObject.name + " Wander");

		// Skitt in some cardinal direction
		int dir = Random.Range(0, 4);
		if(dir == 0)
		{
			_lookVector = new Vector3(0f, 0f, -1f);
			yield return _anim.PlayBlocking("IdleToSide");
			_anim.Play("RunForward");
		}
		else if(dir == 1)
		{
			_lookVector = new Vector3(0f, 0f, 1f);
			yield return _anim.PlayBlocking("IdleToSide");
			_anim.Play("RunBack");
		}
		else
		{
			_lookVector = new Vector3(dir == 2 ? -1f : 1f, 0f, 0f);
			transform.localScale = new Vector3(dir == 2 ? 1f : -1f, 1f, 1f);
			yield return _anim.PlayBlocking("IdleToSide");
			_anim.Play("RunSide");
		}
		_target.position = transform.position + 50f * _lookVector;

		_aiPath.canMove = true;
		yield return new WaitForSeconds(Random.Range(_minRunTime, _maxRunTime));
		_aiPath.canMove = false;

		StartCoroutine(Attack());
		yield break;
	}

	private IEnumerator Attack()
	{
		// Pause and play antic animation
		_anim.Play("Idle");
		yield return new WaitForSeconds(1f);

		// Attack in some direction
		_attacking = true;
		int dir = Random.Range(0, 4);
		if(dir == 0)
		{
			_lookVector = new Vector3(0f, 0f, -1f);
			_anim.Play("LungeForward");
		}
		else if(dir == 1)
		{
			_lookVector = new Vector3(0f, 0f, 1f);
			_anim.Play("LungeBack");
		}
		else
		{
			_lookVector = new Vector3(dir == 2 ? -1f : 1f, 0f, 0f);
			transform.localScale = new Vector3(dir == 2 ? 1f : -1f, 1f, 1f);
			_anim.Play("LungeSide");
		}

		float timer = 0f;
		while(timer < _attackTime)
		{
			float percent = timer / _attackTime;

			// Move sprite up and down to have a parabolic arc
			float yPos = 0.2f + 0.25f * (1f - Mathf.Pow(2f * percent - 1f, 2f));
			_spriteObj.transform.localPosition = new Vector3(0f, yPos);

			transform.position += _runSpeed * _lookVector * Time.deltaTime;

			timer += Time.deltaTime;
			yield return null;
		}
		_spriteObj.transform.localPosition = new Vector3(0f, 0.2f);
		_attacking = false;

		StartCoroutine(Idle());
		yield break;
	}

	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log(collision);
		if(_attacking && collision.collider.CompareTag("Player"))
		{
			if(doDebugLog) Debug.Log(gameObject.name + " hit player");
			collision.gameObject.GetComponent<IDamageable>().TakeDamage(5, Elements.Neutral);
		}
	}
}
