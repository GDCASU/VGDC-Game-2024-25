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
    [InspectorReadOnly] private int aggressionLevel = 1;

    private PlayerController player;
    private Animator animator;

    private GameObject lazerAimGameObject;
    private GameObject lazerBeamGameObject;

    private float currentSpawnTime;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // count down cooldowns
        currentSpawnTime -= Time.deltaTime;

        // handle spawn cooldown
        if(currentSpawnTime <= 0)
        {
            currentSpawnTime = Random.Range(settings.minSpawnTime, settings.maxSpawnTime);

            GameObject newMinion = Instantiate(settings.gamPrefab, transform.position, Quaternion.identity);
        }


        // handle attack cooldown
        
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

    public void BurstAttack()
    {
        burstFirePoint.LookAt(player.transform.position);

        float angleOffset = settings.burstAttackSpreadAngle / settings.burstProjectileCount;

        for (int i = 0; i < settings.burstProjectileCount; i++)
        {
            Vector3 projectileDirection = Quaternion.AngleAxis((-settings.burstAttackSpreadAngle / 2) + angleOffset * i, Vector3.up) * burstFirePoint.forward;

            GameObject elementProjectile = Instantiate(settings.burstProjectilePrefab, burstFirePoint.position, Quaternion.identity);
            ElementProjectile projectile = elementProjectile.GetComponent<ElementProjectile>();
            projectile.moveDir = projectileDirection;
            projectile.ownerTag = gameObject.tag;
        }
    }

    public void SpawnLazerAim()
    {
        if (lazerAimGameObject != null)
            return;

        float angleB = Vector3.Angle(transform.forward, player.transform.position - transform.position);

        float horizontalOffset = Mathf.Sin(angleB * Mathf.Deg2Rad) * Vector3.Distance(transform.position, player.transform.position);

        lazerAimGameObject = Instantiate(settings.aimPrefab, transform.position + transform.right * horizontalOffset, Quaternion.identity);
        lazerAimGameObject.transform.right = transform.right;
    }

    public void SpawnLazerBeam()
    {
        if (lazerBeamGameObject != null)
            return;

        lazerBeamGameObject = Instantiate(settings.lazerPrefab, lazerAimGameObject.transform.position, lazerAimGameObject.transform.rotation);

        Destroy(lazerAimGameObject.gameObject);
        lazerAimGameObject = null;
    }

    public void EndLazerBeam()
    {
        Destroy(lazerBeamGameObject.gameObject);
        lazerBeamGameObject = null;
    }

    public void OnBarrierBreak()
    {

    }

    // TODO: Add eel death logic
    private void OnDeath()
    {
        animator.SetTrigger("eelDeath");
        // Some other logic here
    }
}

