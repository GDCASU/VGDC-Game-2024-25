using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public enum DialogueOptions
    {
        PauseGameTime,
        TextBox,
        TriggerEvent
    }

    [Header("Dialogue Settings")]
    public DialogueOptions dialogueOptions;
    public bool hasCharacterPortrait;
    public bool isInteractable;

    [Header("External Objects")]
    public Action dialogueEvent;
    public TextAsset script;
    TMP_Text dialogueText;
    Interactions interactions;

    [Header("Preset Options")]
    public string[] characterNames;

    // Others
    float charactersPerSecond = 30;

    // Current Dialogue variables
    string currentLine;
    int currentLineNo;
    string[][] dialogue;


    void Start()
    {
        // Instantiates interactions script 
        interactions = GetComponent<Interactions>();
        if (interactions == null && isInteractable) { Debug.LogWarning("No Interactions script found on " + this.gameObject.name); }
        else if (isInteractable)
        {
            switch ((int)dialogueOptions)
            {
                case 0:
                    interactions.ChangeInteraction(PauseGameTime);
                    break;
                case 1: interactions.ChangeInteraction(TextBox); break;
                case 2: interactions.ChangeInteraction(TriggerEvent); break;
            }
        }

        // Instantiates dialogue text TMP component
        dialogueText = GetComponentInChildren<TMP_Text>();
        if (dialogueText == null) { Debug.LogWarning("No TMP_Text component found on the child of " + this.gameObject.name); }

        // Initializes current dialogue sequence
        dialogue = ReadFile();
        currentLine = dialogue[currentLineNo][0] + ": " + dialogue[currentLineNo][1];
        currentLineNo = 0;
    }

    void PauseGameTime()
    { 
        Time.timeScale = 0f;
        StartDialogue();
    }
    void TextBox()
    {
        StartDialogue();
    }
    void TriggerEvent() 
    {
        StartDialogue();
        //dialogueEvent.Invoke();
    }

    void StartDialogue()
    {
        // add change dialogue behavior to input system
        InputManager.OnChangeDialogue += ChangeDialogue;

        StartCoroutine(TypewriterText(currentLine));
    }
    public void ChangeDialogue()
    {
        if (currentLineNo == dialogue.Length) { ExitDialogue(); }
        else if (currentLine == dialogueText.text) // If the typewriter effect has finished
        {
            currentLineNo++;
            currentLine = dialogue[currentLineNo][0] + ": " + dialogue[currentLineNo][1];
        }
        else
        {
            // Skip to the end of the effect
            StopCoroutine(TypewriterText(currentLine));
            dialogueText.text = currentLine;
        }
    }
    void ExitDialogue()
    {
        Time.timeScale = 1f;

        // remove change dialogue behavior from input system
        InputManager.OnChangeDialogue -= ChangeDialogue;
    }

    IEnumerator TypewriterText(string line)
    {
        // NOTE: Do not use Time.deltaTime dependent functions
        // as they will not work with a frozen time scale

        float timer = 0;
        float interval = 1 / charactersPerSecond;
        string textBuffer = null;
        char[] chars = line.ToCharArray();
        int i = 0;

        while (i < chars.Length)
        {
            if (timer < 0.01f)
            {
                textBuffer += chars[i];
                dialogueText.text = textBuffer;
                timer += interval;
                i++;
            }
            else
            {
                timer -= 0.01f;
                yield return null;
            }
        }
    }

    /// <summary>  Takes information from text files and transfers into something the system can read </summary>
    public string[][] ReadFile()
    {
        // split script based on each line
        string scriptText = script.text;
        string[] lines = Regex.Split(scriptText, "\n|\r|\r\n"); 

        string[][] act = new string[1000][];
        int dialogueIndex = 0;
        string currentSpeaker = "";

        foreach (string line in lines)
        {
            if (line.StartsWith("//")) { return act; } // if is a comment

            else if (line == "END") // Checks if the file is done
            {
                act[dialogueIndex] = new string[2];
                act[dialogueIndex][0] = "END";
                act[dialogueIndex][1] = "END";
                return act;
            }

            else if (IsCharacterName(line)) { currentSpeaker = line; } // If is a name

            // If line isn't blank, store dialogue
            else if (!string.IsNullOrWhiteSpace(line))
            {
                act[dialogueIndex] = new string[2];
                act[dialogueIndex][0] = currentSpeaker;
                act[dialogueIndex][1] = line;
                dialogueIndex++;
                Debug.Log(currentSpeaker + ": " + line);
            }
        }
        
        return act;
    }

    private bool IsCharacterName(string text)
    {
        foreach (string name in characterNames)
        {
            if (text == name) { return true; }
        }
        return false;
    }
}
