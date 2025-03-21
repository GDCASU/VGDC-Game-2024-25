using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.CompilerServices;

public class Water_Shock : MonoBehaviour
{ 

    // Start is called before the first frame update
    private void OnTriggerStay(Collider collision)
    {
        Debug.Log("IN");
        bool obtained = collision.gameObject.TryGetComponent<IDamageable>(out IDamageable IDG);
        if (obtained)
        {
            IDG.TakeDamage(0, Elements.Water);
        }
        if (collision.gameObject.tag == "Projectile")
        {
            Debug.Log("PROJEF");
        }


    }
   


}
