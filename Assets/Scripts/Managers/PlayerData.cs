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
    private PlayerObject player;
    public float health;
    public int mana;
    public int experience;

	private DamageableEntity _damageableEntity;

    public PlayerObject Player { 
        get { return player; } 
        set 
        {
            OnPlayerConnectToPlayerData.Invoke();
            player = value; 
        }
    }


    private PlayerController playerController;

    public PlayerController PlayerController { get { return playerController; } set { playerController = value; } }


    public UnityEvent OnPlayerConnectToPlayerData;


    //public InventorySystem inventory{get; set;}



    // Start is called before the first frame update
    void Start()
    {
        OnPlayerConnectToPlayerData.AddListener(CheckMultiPlayerCase);

		_damageableEntity = GetComponent<DamageableEntity>();

		_damageableEntity.OnDamaged += OnDamaged;
	}

    // Handles damage taken
	private void OnDamaged(float damage, float multiplier, Elements element)
	{
        // Subtract health
        health -= damage * multiplier;
		if(health < 0)
        {
            // Death event
            health = 0;
        }
	}

	// Update is called once per frame
	void Update()
    {

    }


    void CheckMultiPlayerCase() {
        if (player != null)
            Debug.LogWarning("It seems there is two player on the scene.");
    }
}
