using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/* -----------------------------------------------------------
 * Author:
 * Chandler Van
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Script for the second boss logic
 */// --------------------------------------------------------


/// <summary>
/// Class for the EelBoss
/// </summary>
[RequireComponent(typeof(ElementStatusHandler), typeof(Animator))]
public class EelBoss : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private ElementStatusHandler elementStatusHandler;
    [SerializeField] private EelBossSettings settings;
    [SerializeField] private Transform burstFirePoint;

    [Header("Events")]
    [SerializeField] private UnityEvent onDeathStart;
    [SerializeField] private UnityEvent onDeathEnd;

    [Header("Readouts")]
    [InspectorReadOnly][SerializeField] private int currentHealth;
    [InspectorReadOnly][SerializeField] private int barriersBroken = 0;
    [InspectorReadOnly][SerializeField] private int phase = 1;

    private PlayerController player;
    private Animator animator;

    private GameObject lazerAimGameObject;
    private GameObject lazerBeamGameObject;

    private float currentSpawnTime;
    private float currentAttackTime;
    private int lastAttack; // burst = 0, lazer = 1
    private int attackStreak = 0;

    private int gamCount = 0;
    private bool attacking = false;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // count down cooldowns
        currentSpawnTime -= Time.deltaTime;

        if (!attacking)
            currentAttackTime -= Time.deltaTime;

        // handle spawn cooldown
        if (currentSpawnTime <= 0 && settings.gamPrefab != null && (gamCount < settings.maxGams || settings.maxGams < -1))
        {
            // reset timer
            if (barriersBroken > 0)
                currentSpawnTime = Random.Range(settings.minSpawnTime * (settings.gamSpawnIncreasePerBarrier * barriersBroken),
                                                settings.maxSpawnTime * (settings.gamSpawnIncreasePerBarrier * barriersBroken));
            else
                currentSpawnTime = Random.Range(settings.minSpawnTime, settings.maxSpawnTime);

            gamCount++;

            // chose gam position
            Vector3 spawnDirection = transform.position
                                     + (Quaternion.AngleAxis(Random.Range(0f,360f), Vector3.up) * transform.forward).normalized * settings.gamSpawnRadius;

            // spawn gam
            Instantiate(settings.gamPrefab, spawnDirection, Quaternion.identity);
        }

        // handle attack cooldown
        if(currentAttackTime <= 0)
        {
            attacking = true;

            // get chances for each attack
            float lazerChance = settings.lazerAttackChance + (attackStreak * settings.frequencyOffset * (lastAttack == 1 ? -1 : 1));
            float burstChance = settings.burstAttackChance + (attackStreak * settings.frequencyOffset * (lastAttack == 0 ? -1 : 1));

            float choice = Random.Range(0f, 1f);

            if (lazerChance <= burstChance) // Lazer Attack has the lower chance of happening
                if (choice <= lazerChance) // Lazer Attack
                {
                    animator.SetTrigger("lazerAim");
                    if (lastAttack == 1) // Last attack was lazer attack
                        attackStreak++;
                    else
                    {
                        lastAttack = 1;
                        attackStreak = 0;
                    }
                }
                else // Burst Attack
                {
                    animator.SetTrigger("burstAttack");
                    if (lastAttack == 0)
                        attackStreak++;
                    else
                    {
                        lastAttack = 0;
                        attackStreak = 0;
                    }
                }
            else if (choice <= burstChance) // Burst Attack has the lower chance of happening
            {
                animator.SetTrigger("burstAttack");
                if (lastAttack == 0)
                    attackStreak++;
                else
                {
                    lastAttack = 0;
                    attackStreak = 0;
                }
            }
            else
            {
                animator.SetTrigger("lazerAim");
                if (lastAttack == 1)
                    attackStreak++;
                else
                {
                    lastAttack = 1;
                    attackStreak = 0;
                }
            }

            float minTime = settings.minAttackTime * (phase == 2 ? 1 / settings.aggressionIncreasePercent : 1);
            float maxTime = settings.maxAttackTime * (phase == 2 ? 1 / settings.aggressionIncreasePercent : 1);

            currentAttackTime = Random.Range(minTime, maxTime);
        }
    }

    public void OnGamDeath()
    {
        gamCount--;
    }

    public ReactionType TakeDamage(int damage, Elements element)
    {
        // Ignore damage if barriers are still active

        // Compute damage through multiplier
        int newDamage = settings.damageMultiplier.ComputeDamage(damage, element);
        // Ignore if zero/immune
        if (newDamage <= 0) return ReactionType.Undefined;
        // Damage health
        int previousHealth = currentHealth;
        currentHealth -= newDamage;
        /*if (currentHealth <= 0)
        {
            // Render damage
            HitpointsRenderer.Instance.PrintDamage(transform.position, currentHealth, Color.red);
        }
        else
        {
            HitpointsRenderer.Instance.PrintDamage(transform.position, newDamage, Color.red);
        }*/
        // Update health bar
        //healthBar.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            // Enemy died
            currentHealth = 0;
            OnDeath();
            // FIXME: Return undefined?
            return ReactionType.Undefined;
        }
        // Send the element to the status handler and return a reaction if caused
        return elementStatusHandler.HandleElementStatus(element);
    }

    public IEnumerator DelayedFunction(float delay, System.Action func)
    {
        yield return new WaitForSeconds(delay);
        func.Invoke();
    }

    public void BurstAttack()
    {
        // Play SFX here

        burstFirePoint.LookAt(player.transform.position);

        float angleOffset = settings.burstAttackSpreadAngle / settings.burstProjectileCount;

        // Spawn and init projectiles
        for (int i = 0; i < settings.burstProjectileCount; i++)
        {
            Vector3 projectileDirection = Quaternion.AngleAxis((-settings.burstAttackSpreadAngle / 2) + angleOffset * i, Vector3.up) * burstFirePoint.forward;

            GameObject elementProjectile = Instantiate(settings.burstProjectilePrefab, burstFirePoint.position, Quaternion.identity);
            EelBurstProjectile projectile = elementProjectile.GetComponent<EelBurstProjectile>();
            projectile.moveDir = projectileDirection;
            projectile.ownerTag = gameObject.tag;
        }

        attacking = false;
    }

    public void SpawnLazerAim()
    {
        if (lazerAimGameObject != null)
            return;

        // Play sfx here
        
        lazerAimGameObject = Instantiate(settings.aimPrefab, burstFirePoint.position, Quaternion.identity);

        Vector3 targetPosition = player.transform.position;
        targetPosition.y = burstFirePoint.position.y;

        lazerAimGameObject.transform.LookAt(targetPosition);

        StartCoroutine(DelayedFunction(settings.aimTime, () => animator.SetTrigger("lazerFire")));
    }

    public void SpawnLazerBeam()
    {
        if (lazerBeamGameObject != null)
            return;

        // Play SFX here

        lazerBeamGameObject = Instantiate(settings.lazerPrefab, lazerAimGameObject.transform.position, lazerAimGameObject.transform.rotation);

        Destroy(lazerAimGameObject.gameObject);
        lazerAimGameObject = null;

        // Start timer to end lazer
        StartCoroutine(DelayedFunction(settings.lazerTime, () => animator.SetTrigger("lazerEnd")));

    }

    public void EndLazerBeam()
    {
        // Play SFX here??? (idk if theres a sfx for this part)

        lazerBeamGameObject.GetComponent<EelLazerBeam>().StartDestroyLaser();
        lazerBeamGameObject = null;

        attacking = false;
    }

    public void OnBarrierBreak()
    {
        barriersBroken++;

        HazardTile[] possibleBarriers = FindObjectsOfType<HazardTile>();

        foreach(HazardTile tile in possibleBarriers)
            if (tile.tileType == HazardTile.TileType.Barrier)
                return;

        phase = 2;
        // possible animation here?
    }

    // TODO: Finish eel death logic
    private void OnDeath()
    {
        StopAllCoroutines();
        animator.SetTrigger("eelDeath");
        onDeathStart?.Invoke();
        // Some other logic here
    }

    public void OnDeathFinish()
    {
        onDeathEnd?.Invoke();
    }
}

