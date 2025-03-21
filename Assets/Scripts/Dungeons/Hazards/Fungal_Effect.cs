using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class Funga_Effect : MonoBehaviour
{

    // Start is called before the first frame update
    private void OnTriggerStay(Collider collision)
    {
        Debug.Log("Enter");
        bool obtained = collision.gameObject.TryGetComponent<IDamageable>(out IDamageable IDG);
        if (obtained)
        {
            IDG.TakeDamage(0, Elements.Fungal);
        }
        if (collision.gameObject.tag == "Projectile")
        {
            Debug.Log("PROJEF");
        }



    }



}
