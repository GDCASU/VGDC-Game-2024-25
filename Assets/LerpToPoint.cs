using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LerpToPoint : MonoBehaviour
{
    public float duration = 3f;
    public Ease ease;
    public Transform target;
    
    public void Activate()
    {
        transform.DOMove(target.position, duration).SetEase(ease);
    }
}
