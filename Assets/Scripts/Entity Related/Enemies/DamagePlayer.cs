using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamagePlayer : MonoBehaviour
{
	[SerializeField] private int damage;
	[SerializeField] private Elements element;

	[SerializeField] private UnityEvent onPlayerHit;

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			other.GetComponent<IDamageable>().TakeDamage(damage, element, transform.position);
			onPlayerHit?.Invoke();
		}
	}
}
