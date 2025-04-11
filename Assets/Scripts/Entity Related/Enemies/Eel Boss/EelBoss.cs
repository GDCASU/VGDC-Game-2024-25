using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ElementStatusHandler), typeof(Animator))]
public class EelBoss : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private ElementStatusHandler elementStatusHandler;
    [SerializeField] private EelBossSettings settings;
    [SerializeField] private Transform burstFirePoint;


    [Header("Readouts")]
    [InspectorReadOnly] private int currentHealth;
    [InspectorReadOnly] private int barriersBroken = 1;
    [InspectorReadOnly] private int phase = 1;

    private PlayerController player;
    private Animator animator;

    private GameObject lazerAimGameObject;
    private GameObject lazerBeamGameObject;

    private float currentSpawnTime;
    private float currentAttackTime;
    private int lastAttack; // burst = 0, lazer = 1
    private int attackStreak = 0;

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

        if(!attacking)
            currentAttackTime -= Time.deltaTime;

        // handle spawn cooldown
        if(currentSpawnTime <= 0 && settings.gamPrefab != null)
        {
            currentSpawnTime = Random.Range(settings.minSpawnTime, settings.maxSpawnTime);

            GameObject newMinion = Instantiate(settings.gamPrefab, transform.position, Quaternion.identity);
        }

        // handle attack cooldown
        if(currentAttackTime <= 0)
        {
            attacking = true;

            float lazerChance = settings.lazerAttackChance + (attackStreak * settings.frequencyOffset * (lastAttack == 1 ? -1 : 1));
            float burstChance = settings.burstAttackChance + (attackStreak * settings.frequencyOffset * (lastAttack == 0 ? -1 : 1));

            float choice = Random.Range(0, 1);

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
            currentAttackTime = Random.Range(settings.minAttackTime, settings.maxAttackTime);
        }
    }

    public ReactionType TakeDamage(int damage, Elements element)
    {
        // Ignore damage if barriers are still active
        //if (FindObjectsOfType<Barrier>().Length > 0) return;

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
        burstFirePoint.LookAt(player.transform.position);

        float angleOffset = settings.burstAttackSpreadAngle / settings.burstProjectileCount;

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

        float angleB = Vector3.Angle(transform.forward, player.transform.position - transform.position);

        float horizontalOffset = Mathf.Sin(angleB * Mathf.Deg2Rad) * Vector3.Distance(transform.position, player.transform.position);

        lazerAimGameObject = Instantiate(settings.aimPrefab, transform.position + transform.right * horizontalOffset, Quaternion.identity);
        lazerAimGameObject.transform.right = transform.right;

        StartCoroutine(DelayedFunction(settings.aimTime, () => animator.SetTrigger("lazerFire")));
    }

    public void SpawnLazerBeam()
    {
        if (lazerBeamGameObject != null)
            return;

        lazerBeamGameObject = Instantiate(settings.lazerPrefab, lazerAimGameObject.transform.position, lazerAimGameObject.transform.rotation);

        Destroy(lazerAimGameObject.gameObject);
        lazerAimGameObject = null;

        // Start timer to end lazer
        StartCoroutine(DelayedFunction(settings.lazerTime, () => animator.SetTrigger("lazerEnd")));

    }

    public void EndLazerBeam()
    {
        Destroy(lazerBeamGameObject.gameObject);
        lazerBeamGameObject = null;

        attacking = false;
    }

    public void OnBarrierBreak()
    {
        barriersBroken++;
    }

    // TODO: Add eel death logic
    private void OnDeath()
    {
        StopAllCoroutines();
        animator.SetTrigger("eelDeath");
        // Some other logic here
    }
}

