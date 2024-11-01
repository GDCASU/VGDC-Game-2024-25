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
    public int health;
    public int mana;
    public int experience;
    public PlayerObject Player { 
        get { return player; } 
        set 
        {
            // TODO: fix or remove this, since it causes a NullReferenceException when the PlayerSetReference script triggers this setter
            // OnPlayerConnectToPlayerData.Invoke();
            player = value; 
        } }


    private PlayerController playerController;

    public PlayerController PlayerController { get { return playerController; } set { playerController = value; } }


    public UnityEvent OnPlayerConnectToPlayerData;


    //public InventorySystem inventory{get; set;}



    // Start is called before the first frame update
    void Start()
    {
        OnPlayerConnectToPlayerData.AddListener(CheckMultiPlayerCase);
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
