using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DamageIndicatorSettings")]
public class DamageIndicatorSettings : ScriptableObject
{
    [Header("Defaults")]
    public ElementDamageStats defaultIndicatorConfig;
    public GameObject damageIndicatorPrefab;
    [Header("Element Configs")]
    public ElementDamageStats fireIndicatorConfig;
    public ElementDamageStats waterIndicatorConfig;
    public ElementDamageStats sparkIndicatorConfig;
    public ElementDamageStats fungalIndicatorConfig;
    public ElementDamageStats neutralIndicatorConfig;

    [Serializable]
    public struct ElementDamageStats
    {
        public Color textColor;
        [Tooltip("Overrides the default font used for indicator")] public TMPro.TMP_FontAsset damageFontOverride;
    }

    public ElementDamageStats GetConfig(Elements element)
    {
        switch (element)
        {
            case Elements.Fire:
                return fireIndicatorConfig;
            case Elements.Fungal:
                return fungalIndicatorConfig;
            case Elements.Sparks:
                return sparkIndicatorConfig;
            case Elements.Undefined:
                Debug.LogWarning("Undefined Type Damage was selected for Inidcator??? This was supposed to happen right?");
                break;
            case Elements.Water:
                return waterIndicatorConfig;
        }

        return defaultIndicatorConfig;
    }
}

