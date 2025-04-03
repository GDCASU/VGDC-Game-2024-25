using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ElementStatusHandler))]
public class EelBoss : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private ElementStatusHandler elementStatusHandler;
    [SerializeField] private EelBossSettings settings;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform burstFirePoint;


    [Header("Readouts")]
    [InspectorReadOnly] private int currentHealth;
    [InspectorReadOnly] private int aggressionLevel = 1;

    private PlayerController player;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerController>();
    }

    public ReactionType TakeDamage(int damage, Elements element)
    {
        // Compute damage through multiplier
        int newDamage = settings.damageMultiplier.ComputeDamage(damage, element);
        // Ignore if zero/immune
        if (newDamage <= 0) return ReactionType.Undefined;
        // Damage health
        int previousHealth = currentHealth;
        currentHealth -= newDamage;
        if (currentHealth <= 0)
        {
            // Render damage
            HitpointsRenderer.Instance.PrintDamage(transform.position, currentHealth, Color.red);
        }
        else
        {
            HitpointsRenderer.Instance.PrintDamage(transform.position, newDamage, Color.red);
        }
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

    public void OnBarrierBreak()
    {

    }

    private void OnDeath()
    {

    }
}

