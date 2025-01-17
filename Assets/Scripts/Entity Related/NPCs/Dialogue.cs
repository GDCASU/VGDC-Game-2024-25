using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    TMP_Text dialogueText;
    Interactions interactions;
    Action[] dialogueTypes = new Action[] { PauseGameTime, TextBox, Other};
    float charactersPerSecond = 90;

    public enum DialogueOptions
    {
        PauseGameTime,
        TextBox,
        Other
    }
    public DialogueOptions dialogueOptions;

    // Start is called before the first frame update
    void Start()
    {
        interactions = GetComponent<Interactions>();
        dialogueText = GetComponent<TMP_Text>();
        interactions.ChangeInteraction(dialogueTypes[(int)dialogueOptions]);
    }

    static void PauseGameTime()
    {

    }

    static void TextBox()
    {

    }
    static void Other() 
    { 

    }

    void ExitDialogue()
    {

    }

    IEnumerator TypewriterText(string line)
    {
        float timer = 0;
        float interval = 1 / charactersPerSecond;
        string textBuffer = null;
        char[] chars = line.ToCharArray();
        int i = 0;

        while (i < chars.Length)
        {
            if (timer < Time.deltaTime)
            {
                textBuffer += chars[i];
                dialogueText.text = textBuffer;
                timer += interval;
                i++;
            }
            else
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
