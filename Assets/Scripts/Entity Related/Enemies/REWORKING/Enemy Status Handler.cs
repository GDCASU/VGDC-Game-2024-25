using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/* -----------------------------------------------------------
 * Author:
 * Davyd Yehudin
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Status effects handler, also handles reaction (only for non-player entities)
 */// --------------------------------------------------------


/// <summary>
/// 
/// </summary>
public enum ItemReactBehaviour
{
    objectUnreactiveAfter = 0,
    canReactAgain = 1
}
public enum EntityType
{
    Enemy = 0,
    Object = 1
}
public class EnemyStatusHandler : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    private int currentStatus = 0;
    [SerializeField] private int defaultStatus = 1;
    [SerializeField] private bool reactive = true;
    [SerializeField] private float statusLength = 5f;
    private float timePassed = 0;
    private Coroutine currentStatusCoroutine;
    [SerializeField] private int correctStatus = -1;
    [SerializeField] private UnityEvent onCorrectReaction;
    [SerializeField] private EntityType thisEntity = EntityType.Enemy;
    [SerializeField] private ItemReactBehaviour AfterReactionBehaviour = ItemReactBehaviour.canReactAgain;
    [SerializeField] private int burnDmg = 1;
    [SerializeField] private float burnDuration = 12f;
    [SerializeField] private float burnTickRate = 0.4f;
    [SerializeField] private float fireworkDmg = 50f;
    [SerializeField] private GameObject firework;
    private DamageableEntity something;


    void Awake()
    {
        something = this.GetComponent<DamageableEntity>(); //this will be used to deal damage, rename it if you need it

        if(something == null){
            throw new ArgumentNullException("Reaction Handler Could Not Find Damageable Entity, the entity's name is ", this.name);
        }

        if(!reactive){
            defaultStatus = 0;
        }
        currentStatus = defaultStatus;
        onCorrectReaction.AddListener(TestFunc);
        something = GetComponent<DamageableEntity>();
        something.OnDamaged += Handler;
    }

    //this is for testing please don't uncomment unless needed thx)

    /*void Update()
    {
        if(Input.GetKeyDown(KeyCode.O)){
            Handler(0,0,Elements.fire, EnemyStatusEffect.burning);
        }
    }*/

    private void Handler(float damage, float multiplier, Elements element, StatusEffect app){
        //this line makes sure that we only apply new status effects, all status effects are prime numbers, so remember to not only add them to the enum, but also make sure they have the correct number
        bool newStatus = (currentStatus % (int)app != 0);
        if(newStatus) currentStatus = currentStatus * (int)app; 
        //if a coroutine exists we just reset the time that passed
        if(currentStatusCoroutine != null){
            timePassed = 0;
        }

        //For enemy reactions

        if(thisEntity == EntityType.Enemy && newStatus){
            switch(currentStatus){
            /*
            Main idea is simple
            if there was a new status applied
            we check to see what status the entity has after the application
            if a coroutine already exists, we stop it (more of a safeguard than anything)
            after that we begin the coroutine of the status effect we need, or we call a function (depends) 
            */
            case 2:
                if(currentStatusCoroutine != null) StopCoroutine(currentStatusCoroutine);
                currentStatusCoroutine = StartCoroutine(burning());
                return;
            case 3:
                if(currentStatusCoroutine != null) StopCoroutine(currentStatusCoroutine);
                currentStatusCoroutine = StartCoroutine(spored());
                return;
            case 6:
                if(currentStatusCoroutine != null){
                    StopCoroutine(currentStatusCoroutine);
                    currentStatusCoroutine = null;
                }
                fireworks();
                currentStatus = defaultStatus;
                return;
            default:
                return;
        }
        }

        //For object reactions


        //if the object does not have a status timer, we make it
        //if there was one, we already made sure that the status lasts (lines 89-91)
        if(thisEntity == EntityType.Object && currentStatusCoroutine == null){
            currentStatusCoroutine = StartCoroutine(StatusTimer());
        }

        //triggers an event if the object has the correct status
        if(currentStatus == correctStatus && thisEntity == EntityType.Object){
            onCorrectReaction.Invoke();
            switch(AfterReactionBehaviour){
                case ItemReactBehaviour.objectUnreactiveAfter:
                    defaultStatus = 0;
                    currentStatus = defaultStatus;
                    if(currentStatusCoroutine != null){
                        StopCoroutine(currentStatusCoroutine);
                        currentStatusCoroutine = null;
                    }
                    break;
                case ItemReactBehaviour.canReactAgain:
                    if(currentStatusCoroutine != null){
                        StopCoroutine(currentStatusCoroutine);
                        currentStatusCoroutine = null;
                    }
                    break;
                default:
                    Debug.Log("INCORRECT ENUM FOR ITEM REACTION HANDLER");
                    break;
            }
            
        }
    }

    //template
/*    private IEnumerator myTimer()
    {
        timePassed = 0;
        while(true)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= 5)
            {
                currentStatus = defaultStatus;
                break;
            }
            yield return null; // Wait a frame
        }
        myTimerCo = null;
    }
*/

    //Test for objects
    private void TestFunc(){
        if(doDebugLog) Debug.Log("TRIGGERED");
    }

    //Coroutine (singular) for objects

    IEnumerator StatusTimer()
    {
        timePassed = 0;
        while (true)
        {
            if(timePassed >= statusLength) break;
            timePassed += Time.deltaTime;
            yield return null;
        }
        currentStatus = (int)defaultStatus;
        currentStatusCoroutine = null;
        yield break;
    }

    //Coroutines for enemies

    IEnumerator burning()
    {
        while(true){
            if(timePassed >= burnDuration) break;
            //deal burnDmg
            if(doDebugLog) Debug.Log("OH MY GOD I AM BURNING FOR " + burnDmg + " DMG AHHHHHHHHHHHHHHHH");
            timePassed += burnTickRate;
            yield return new WaitForSeconds(burnTickRate);
        }
        currentStatus = (int)defaultStatus;
        currentStatusCoroutine = null;
        yield break;
    }
    IEnumerator spored()
    {
        timePassed = 0;
        while(true){
            if(timePassed >= statusLength) break;
            //change the speed of the entity
            timePassed += Time.deltaTime;
            yield return null;
        }
        //change the speed of the entity back
        currentStatus = (int)defaultStatus;
        currentStatusCoroutine = null;
        yield break;
    }
    void fireworks()
    {
        //deal a lot of damage (flat)
        //launch 8 projectiles, prolly better to use enemy projectile script, so just a stub here for now
    }
}
