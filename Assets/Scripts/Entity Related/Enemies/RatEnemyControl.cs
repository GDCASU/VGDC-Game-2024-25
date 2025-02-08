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
	private GameObject _spriteObj;
	private bool _attacking;

	void Start()
	{
		_anim = GetComponent<Animator>();
		_spriteObj = transform.Find("Sprite").gameObject;
		StartCoroutine(Idle());
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
		Vector3 move = new Vector3();
		if(dir == 0)
		{
			move = new Vector3(0f, 0f, -1f);
			yield return _anim.PlayBlocking("IdleToSide");
			_anim.Play("RunForward");
		}
		else if(dir == 1)
		{
			move = new Vector3(0f, 0f, 1f);
			yield return _anim.PlayBlocking("IdleToSide");
			_anim.Play("RunBack");
		}
		else
		{
			move = new Vector3(dir == 2 ? -1f : 1f, 0f, 0f);
			transform.localScale = new Vector3(dir == 2 ? 1f : -1f, 1f, 1f);
			yield return _anim.PlayBlocking("IdleToSide");
			_anim.Play("RunSide");
		}
		float timer = Random.Range(_minRunTime, _maxRunTime);
		while(timer > 0f)
		{
			transform.position += _runSpeed * move * Time.deltaTime;
			timer -= Time.deltaTime;
			yield return null;
		}
		StartCoroutine(Attack());
		yield break;
	}

	private IEnumerator Attack()
	{
		// Pause and play antic animation
		_anim.StopPlayback();
		yield return new WaitForSeconds(1f);

		// Attack in some direction
		_attacking = true;
		int dir = Random.Range(0, 4);
		Vector3 move = new Vector3();
		if(dir == 0)
		{
			move = new Vector3(0f, 0f, -1f);
			_anim.Play("LungeForward");
		}
		else if(dir == 1)
		{
			move = new Vector3(0f, 0f, 1f);
			_anim.Play("LungeBack");
		}
		else
		{
			move = new Vector3(dir == 2 ? -1f : 1f, 0f, 0f);
			transform.localScale = new Vector3(dir == 2 ? 1f : -1f, 1f, 1f);
			_anim.Play("LungeSide");
		}

		float timer = 0f;
		while(timer < _attackTime)
		{
			float percent = timer / _attackTime;

			// Move sprite up and down to have a parabolic arc
			float yPos = 0.25f * (1f - Mathf.Pow(2f * percent - 1f, 2f));
			_spriteObj.transform.localPosition = new Vector3(0f, yPos);

			transform.position += _runSpeed * move * Time.deltaTime;

			timer += Time.deltaTime;
			yield return null;
		}
		_spriteObj.transform.localPosition = new Vector3(0f, 0f);
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
