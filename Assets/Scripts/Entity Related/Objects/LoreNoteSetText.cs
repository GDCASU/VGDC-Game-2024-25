using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/* -----------------------------------------------------------
* Author:
* Cami Lee
* 
* Modified By:
*/// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Change text based on text file
*/// --------------------------------------------------------
public class LoreNoteSetText : MonoBehaviour
{
    [SerializeField] TextAsset textAsset;
    TMP_Text textUI;

    // Start is called before the first frame update
    void Start()
    {
        textUI = GetComponent<TMP_Text>();
        textUI.text = textAsset.text;
    }

}
