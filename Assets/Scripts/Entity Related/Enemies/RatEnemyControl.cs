using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	[SerializeField] private float _runSpeed = 3f;

	// Use this bool to gate all your Debug.Log Statements please
	[Header("Debugging")]
	[SerializeField] private bool doDebugLog;

	private Animator _anim;

	void Start()
	{
		_anim = GetComponent<Animator>();
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

		StartCoroutine(Idle());
		yield break;
	}

	private IEnumerator Attack()
	{

		yield break;
	}
}
