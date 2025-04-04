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
    public GameObject aimPrefab;
    public GameObject lazerPrefab;
    public int beamDamagePerTick = 1;
    public float beamTickInterval = 1;
    public bool beamFriendlyFire = false;
    public float aimTime = 2.0f;
    public float lazerTime = 10.0f;
}
