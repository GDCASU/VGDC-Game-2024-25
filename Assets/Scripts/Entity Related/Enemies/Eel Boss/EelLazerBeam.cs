using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelLazerBeam : MonoBehaviour
{
    public EelBossSettings settings;

    private List<IDamageable> damageTickImmunity = new();

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out IDamageable damageable))
        {
            if (other.gameObject.CompareTag(TagDefinitions.EnemyTag) && !settings.beamFriendlyFire)
                return;


            if (!damageTickImmunity.Contains(damageable))
            {
                damageTickImmunity.Add(damageable);
                damageable.TakeDamage(settings.beamDamagePerTick, Elements.Neutral, transform.position);
                StartCoroutine(DamageTickImmunityCoro(damageable));
            }
        }
           
    }

    private IEnumerator DamageTickImmunityCoro(IDamageable damageable)
    {
        yield return new WaitForSeconds(settings.beamTickInterval);
        damageTickImmunity.Remove(damageable);
    }

}
