using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EelBossSettings", menuName = "Eel Boss Settings")]
public class EelBossSettings : ScriptableObject
{
    public float maxHealth;
    public DamageMultiplier damageMultiplier;

    [Header("Aggression Settings")]
    public float aggressionIncreasePercent = 1.5f;
    public float gamSpawnIncreasePerBarrier = 0.25f;

    [Header("Attack Settings")]
    public float minAttackTime = 0.5f;
    public float maxAttackTime = 2f;
    public float burstAttackChance = .7f;
    public float lazerAttackChance = .3f;
    public float frequencyOffset = 0.05f;

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

    [Header("Spawning Settings")]
    public GameObject gamPrefab;
    public float gamSpawnRadius = 3f;
    public float minSpawnTime = 3;
    public float maxSpawnTime = 6;
}
