using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FadeToBlack : MonoBehaviour
{

    public float duration = 1.0f;
    public Image image;

    public UnityEvent OnFadeIn, OnFadeOut;

    public void Activate()
    {
        image.DOFade(1, duration).OnComplete(() =>
        {
            OnFadeIn.Invoke();
        });
    }

    public void Reverse()
    {
        image.DOFade(0, duration).OnComplete(() =>
        {
            OnFadeOut.Invoke();
        }); 
    }
}
