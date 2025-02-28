using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
	[SerializeField] private int damage;
	[SerializeField] private Elements element;

	private void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			other.GetComponent<IDamageable>().TakeDamage(damage, element);
		}
	}
}
