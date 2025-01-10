using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Events;
//*Author:
//*Jerry Mou Sep 20 2024
//*
//* Modified By:


public class PlayerData : Singleton<PlayerData>
{
    public float health { get; private set; }
    public int mana { get; private set; }
    public int experience { get; private set; }

    // Reference to the current player controller, set in PlayerController.Start
    private PlayerController _playerController;

    public PlayerController PlayerController
    {
        get
        {
            return _playerController;
        }
        set
        {
            _playerController = value;
			_playerController.GetComponent<DamageableEntity>().OnDamaged += OnDamaged;
		}
    }

	/// <summary>
	/// Handles damage taken
	/// </summary>
	private void OnDamaged(float damage, float multiplier, Elements element, EnemyStatusEffect statusEffect)
	{
        // Subtract health
        health -= damage * multiplier;
		if(health < 0)
        {
            // Death event
            health = 0;
        }
	}
}
