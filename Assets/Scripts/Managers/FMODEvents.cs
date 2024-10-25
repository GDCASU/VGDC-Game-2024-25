using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
public class FMODEvents : MonoBehaviour
{
    #region SFX

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference playerFootstepSFX { get; private set; }

    [field: Header("Collectable SFX")]
    [field: SerializeField] public EventReference coinCollect { get; private set; }
    [field: SerializeField] public EventReference coinIdle { get; private set; }


    [field: Header("Test One Shot SFX")]
    [field: SerializeField] public EventReference testSFX { get; private set; }

    [field: Header("Test BGM")]
    [field: SerializeField] public EventReference testBGM { get; private set; }

    #endregion

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log("FMODEvents already exists, destroying duplicate");
            Destroy(gameObject);
        }
    }
}

