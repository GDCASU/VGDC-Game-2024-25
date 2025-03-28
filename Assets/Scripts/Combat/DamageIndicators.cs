using System;
using UnityEngine;

public class DamageIndicators : MonoBehaviour
{
    public DamageIndicatorSettings indicatorSettings;

    public void SpawnIndicator(int damage, Elements element)
    {
        DmgIndicator indicator = Instantiate(indicatorSettings.damageIndicatorPrefab, transform.position, Quaternion.identity).GetComponent<DmgIndicator>();

        DamageIndicatorSettings.ElementDamageStats settings = indicatorSettings.GetConfig(element);
        indicator.InitIndicator(settings.textColor, damage, settings.damageFontOverride);
    }
}

