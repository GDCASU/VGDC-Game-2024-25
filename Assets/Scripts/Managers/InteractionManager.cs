using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* -----------------------------------------------------------
 * Author:
 * Cami Lee
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Create an interactive canvas element when player is close
 * to the object
 */// --------------------------------------------------------


public class InteractionManager : MonoBehaviour
{
    [Header("HitScan")]
    bool canInteract = true; // false during dialogue, pause, etc.
    bool isRunning; // checks to see if coroutine is running
    float radius = 1.5f;

    [Header("Highlight")]
    private Color highlightColor = Color.grey;
    private List<Material> materials;
    private GameObject highlightedObject;

    [Header("Player")]
    [SerializeField] GameObject player;
    Interactions interactions;

    private void Update()
    {
        Detect();
    }
    
    /// <summary> Determines if player is close enough to a detectable object </summary>
    private void Detect()
    {
        GameObject tempObject = null;

        //-- Checks for Collisions --//
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, radius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.GetComponent<Interactions>() != null) { tempObject = collider.gameObject; }
        }

        //-- If there is a new collision --//
        if (tempObject != highlightedObject)
        {
            // Reset Old Highlight
            if (highlightedObject != null)
            {
                interactions = highlightedObject.GetComponent<Interactions>(); // get interaction type
                interactions.EndInteraction(); // prevent interactions

                DisableHighlight(highlightedObject);
            }

            // Set New Highlight
            highlightedObject = tempObject;
            interactions = null;

            if (highlightedObject != null)
            {
                interactions = highlightedObject.GetComponent<Interactions>(); // get interaction type
                interactions.StartInteraction(); // set up interaction variables

                ToggleHighlight(highlightedObject);
            }
        }
    }
    /// <summary> Highlights closest object </summary>
    private void ToggleHighlight(GameObject newObject)
    {
        // Fade in
        StartCoroutine(Fade(true));

        // Highlights the raycast object
        materials = newObject.GetComponent<Renderer>().materials.ToList();

        foreach (var material in materials)
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", highlightColor);
        }

        highlightedObject = newObject; // stores highlighted object.
    }
    /// <summary> Disables out of range highlights </summary>
    private void DisableHighlight(GameObject oldObject)
    {
        // Fade out
        StartCoroutine(Fade(false));

        // Removes highlight from the raycast object
        materials = oldObject.GetComponent<Renderer>().materials.ToList();

        foreach (var material in materials)
        {
            material.DisableKeyword("_EMISSION");
        }

        highlightedObject = oldObject; // updates highlighted object.
    }
    /// <summary> Fades menu and text out </summary>
    IEnumerator Fade(bool fadeIn)
    {
        isRunning = true;

        SpriteRenderer popupMenu = highlightedObject.GetComponentInChildren<SpriteRenderer>();
        int textNumber = highlightedObject.GetComponentsInChildren<TMP_Text>().Length;
        TMP_Text[] popupTexts = new TMP_Text[textNumber];

        for (int i = 0; i < textNumber; ++i)
        {
            popupTexts[i] = highlightedObject.GetComponentsInChildren<TMP_Text>()[i];
        }

        if (textNumber > 0 && popupMenu == null) { Debug.LogError("Missing Sprite Renderer component on highlighted object"); }
        
        float transition; // fade in vs fade out
        if (fadeIn) { transition = .1f; }
        else { transition = -.1f; }

        float max = 1f; float min = 0; // max and min transparency

        float transparencyMenu = popupMenu.color.a + transition; // starting value

        // Fades the menu and text out or in
        while (transparencyMenu >= min && transparencyMenu <= max)
        {
            transparencyMenu += transition;
            popupMenu.color = new Color(popupMenu.color.r, popupMenu.color.g, popupMenu.color.b, transparencyMenu);
            foreach (TMP_Text popupText in popupTexts)
            {
                popupText.color = new Color(popupText.color.r, popupText.color.g, popupText.color.b, transparencyMenu);
            }

            yield return new WaitForSeconds(0.01f);
        }

        isRunning = false;
    }
}