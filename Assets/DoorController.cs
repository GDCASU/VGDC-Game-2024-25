using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Written by Matthew Glos 
 * 
 * public interface to be used by other components to open and close the door. 
 * Recommended to invoke these methods using unity events in oher scripts including 
 * things like rooms, levers buttons etc.
 * 
 */
public class DoorController : MonoBehaviour
{
    Animator animator;

    [SerializeField] bool startOpen = false;
    void Start()
    {
        animator = GetComponent<Animator>();

        if (startOpen) openDoor();
    }

    public void openDoor() 
    {
        animator.SetBool("DoorOpen",true);    
    }
    public void closeDoor()
    {
        animator.SetBool("DoorOpen", false);
    }
}
