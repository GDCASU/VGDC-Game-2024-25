using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EelBossSettings", menuName = "Eel Boss Settings")]
public class EelBossSettings : ScriptableObject
{
    public float maxHealth;
    public DamageMultiplier damageMultiplier;

    [Header("Burst Settings")]
    public GameObject burstProjectilePrefab;
    public int burstProjectileCount;
    public float burstAttackSpreadAngle;

    [Header("Lazer Settings")]
    public int damagePerSecond = 1;
}
