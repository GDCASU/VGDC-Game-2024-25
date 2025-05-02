using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaxiDialogueController : MonoBehaviour
{
    public TMP_Text TaxiDriver, Aubrey;
    public float typeSpeed = 1f;

    public string[] AubreyTalking = { ".*.*.", ".*.*.", "Every story has a sliver of truth, or so they say." };
    public string[] TaxiTalking = { "Pretty late to be heading out this way, right?", "Why are you taking me out this far?", "Ya know...* I’ve heard about this forest before.* Just stories though.* Haha.*" };


    public void Converse(int index)
    {

        StartCoroutine(TypeText(TaxiTalking[index], TaxiDriver, () => {
            StartCoroutine(TypeText(AubreyTalking[index], Aubrey));
        }));
    }


    public IEnumerator TypeText(string line, TMP_Text textField, Action onComplete = null)
    {
        textField.text = "";
        float delay = typeSpeed / Mathf.Max(line.Replace("*", "").Length, 1); // Only count visible characters

        foreach (char letter in line)
        {
            if (letter == '*')
            {
                yield return new WaitForSeconds(0.25f); // Custom pause
            }
            else
            {
                textField.text += letter;
                yield return new WaitForSeconds(delay);
            }
        }

        onComplete?.Invoke(); // Trigger the callback if provided
    }



}
