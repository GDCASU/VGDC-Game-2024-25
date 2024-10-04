using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
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
 * Create an interaction canvas element when player is close
 * to the object
 */// --------------------------------------------------------


/// <summary>
/// Class that will protect all objects that are meant to be present on all scenes
/// </summary>
/// 
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

    bool canChange = true;

    private void Update()
    {
        Detect();
        if (canInteract && highlightedObject != null) { Interact(); }
    }

    private void Interact()
    {
        // Implement Interactable key feature

        interactions.StartInteraction();
    }

    void Detect()
    {
        GameObject tempObject = null;

        // Checks for Collisions
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, radius);
        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.tag == "Interactable" && collider.gameObject.GetComponent<Interactions>() != null)
            {
                Debug.Log($"{collider.gameObject.name} is nearby");

                tempObject = collider.gameObject;
            }
        }

        if (tempObject != highlightedObject)
        {
            // Reset Old Highlight
            if (highlightedObject != null)
            {
                DisableHighlight(highlightedObject);
                StartCoroutine(Fade(-.1f));
            }

            // Set New Highlight
            highlightedObject = tempObject;
            interactions = null;

            if (highlightedObject != null)
            {
                ToggleHighlight(highlightedObject);
                interactions = highlightedObject.GetComponent<Interactions>();
                highlightedObject.GetComponentInChildren<TMP_Text>().text = $"<b>{highlightedObject.name}</b> \n {interactions.typeName}"; // change text based on the object pls

                StartCoroutine(Fade(.1f));
            }
        }
    }
    public void ToggleHighlight(GameObject newObject)
    {
        // Highlights the raycast object
        materials = newObject.GetComponent<Renderer>().materials.ToList();

        foreach (var material in materials)
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", highlightColor);
        }

        highlightedObject = newObject; // stores highlighted object.
    }

    public void DisableHighlight(GameObject oldObject)
    {
        // Removes highlight from the raycast object
        materials = oldObject.GetComponent<Renderer>().materials.ToList();

        foreach (var material in materials)
        {
            material.DisableKeyword("_EMISSION");
        }

        highlightedObject = oldObject; // updates highlighted object.
    }

    IEnumerator Fade(float transition) // change from transition to something else
    {
        isRunning = true;
        SpriteRenderer popupMenu = highlightedObject.GetComponentInChildren<SpriteRenderer>();
        TMP_Text popupText = highlightedObject.GetComponentInChildren<TMP_Text>();

        if (popupMenu == null || popupText == null) { Debug.LogError("Missing component on highlighted object (Sprite Renderer or TMP_Text)"); }

        float max = 1f; float min = 0; // max and min transparency

        float transparencyMenu = popupMenu.color.a + transition;

        // Fades the menu and text out or in
        while (transparencyMenu >= min && transparencyMenu <= max)
        {
            transparencyMenu += transition;
            popupMenu.color = new Color(popupMenu.color.r, popupMenu.color.g, popupMenu.color.b, transparencyMenu);
            popupText.color = new Color(popupText.color.r, popupText.color.g, popupText.color.b, transparencyMenu);

            yield return new WaitForSeconds(0.01f);
        }
        isRunning = false;
    }
}
