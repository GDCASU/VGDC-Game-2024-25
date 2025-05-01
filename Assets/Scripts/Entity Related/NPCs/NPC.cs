using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

/* -----------------------------------------------------------
* Author:
* Chandler Van
* 
* Modified By:
* Ian Fletcher
*/// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Offer a simple framework to build NPCs off of
*/// --------------------------------------------------------

[RequireComponent(typeof(Dialogue))]
public abstract class NPC : MonoBehaviour
{
    [Header("Drops")]
    public List<GameObject> drops;
    [SerializeField] public Transform dropPoint;
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local Variables
    protected Dialogue dialog;

    private void Awake()
    {
        dialog = GetComponent<Dialogue>();
        
    }

    private void OnEnable()
    {
        dialog.onDialogEnd.AddListener(OnDialogEnd);
        dialog.onDialogStart.AddListener(OnDialogStart);
        InputManager.OnAttack += _onPlayerAttack;
    }

    private void OnDisable()
    {
        dialog.onDialogEnd.RemoveListener(OnDialogEnd);
        dialog.onDialogStart.RemoveListener(OnDialogStart);

        InputManager.OnAttack -= _onPlayerAttack;
    }

    /// <summary>
    /// DropItems calls the NPC to drop its items (overridable)
    /// </summary>
    public virtual void DropItems()
    {
        foreach(GameObject drop in drops)
        {
            Instantiate(drop, dropPoint.position, Quaternion.identity);
        }
    }
    
    // Intermediate Function that gives OnPlayerAttack its distance and canSeePlayer parameters
    private void _onPlayerAttack()
    {
        PlayerController player = FindObjectOfType<PlayerController>();

        float distance = Vector3.Distance(player.transform.position, transform.position);

        bool canSeePlayer = Physics.Linecast(player.transform.position, transform.position);
        OnPlayerAttack(distance, canSeePlayer);
    }

    /// <summary>
    /// OnDialogEnd is called when dialog with given npc is finished
    /// </summary>
    public virtual void OnDialogEnd() { if (doDebugLog) Debug.Log("Dialog Ended!", this); }

    /// <summary>
    /// OnDialogStart is called when dialog with given npc is started
    /// </summary>
    public virtual void OnDialogStart() { if (doDebugLog) Debug.Log("Dialog Started!", this); }

    public virtual void OnPlayerAttack(float distance, bool canSeePlayer) { }
}
