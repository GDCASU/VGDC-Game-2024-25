using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
/* -----------------------------------------------------------
 * Eduted by: Eliza Chook
 
 * Pupose: An interact script attached to objects to allow players to view lore notes 
 * by pressing 'E' on the keyboard or 'A' on console to stop all movement (pause game) and open a UI canvas showing the notes. 
 * Players press 'E'/'A' again to exit lore note and resume movement. (Works similar to TestInteraction)
 */// --------------------------------------------------------



public class LoreNote : Interactable
{
    [SerializeField] bool rangeOfObj; // placeholder bool to change functions
    private float fixedDeltaTime;//game's flow of time
    public GameObject targetObject; // Reference to the object with the Canvas
    [SerializeField] public Canvas canvas;//reference canvas UI (lore notes)

    // Start is called before the first frame update
    void Start()
    {
        //record current rate of frame/second in game
        this.fixedDeltaTime = Time.fixedDeltaTime;

        if (rangeOfObj)
        {
            OnFocusExit += OutOfRange; // Makes it so the function OutOfRange will be called with the interaction key
        }
        else { OnFocusEnter += InRange; }
    }

    void OnDestroy()
    {
        OnFocusExit -= OutOfRange;
        OnFocusEnter -= InRange;
    }

    /// <summary> Sample Functions </summary>
    public void OutOfRange()//do nothing when out of range
    {
        Debug.Log("is out of range");
    }

    public void InRange()//when in range
    {
        Debug.Log("is in range");
        if (Time.timeScale != 0)//pause game with inputKey ('T' for keyboard, 'A' for console)
        {
            Time.timeScale = 0;
            Debug.Log("Time Stopped");
            canvas.gameObject.SetActive(true);//enable lore note UI
        }
        else
        {
            Time.timeScale = 1;//resume game with inputKey ('T' for keyboard, 'A' for console)
            Debug.Log("Resumed");
            canvas.gameObject.SetActive(false); //disable lore note UI
        }

    }

}
