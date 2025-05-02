using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DamageDetector : MonoBehaviour, IDamageable
{
    public UnityEvent<int, Elements> onDamageTaken = new();
    public ReactionType TakeDamage(int damage, Elements element, Vector3 direction)
    {
        onDamageTaken?.Invoke(damage, element);

        return ReactionType.Undefined;
    }

    ReactionType IDamageable.TakeDamage(int damage, Elements element)
    {
        onDamageTaken?.Invoke(damage, element);

        return ReactionType.Undefined;
    }
}
