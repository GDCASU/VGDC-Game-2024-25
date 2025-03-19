using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose:
 * Manage the player's health bar
 */// --------------------------------------------------------


/// <summary>
/// Class that manages the players health bar
/// </summary>
public class PlayerHealthBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image heart;
    [SerializeField] private List<Image> beatGraphs; // Should be in order in the inspector
    [SerializeField] private List<Sprite> hearts; // Should be set in order on the inspector, first should be empty

    [Header("Settings")] 
    [SerializeField] private Color greenGraphColor;
    [SerializeField] private Color yellowGraphColor;
    [SerializeField] private Color redGraphColor;
    [SerializeField] private bool moveGraphsRight = false;
    [SerializeField] private int redGraphUpperBound;
    [SerializeField] private int yellowGraphUpperBound;
    [SerializeField, Range(1f, 50f)] private float graphSpeedMultiplier;
    [SerializeField, Range(0.1f,5f)] private float greenGraphSpeed;
    [SerializeField, Range(0.1f,5f)] private float yellowGraphSpeed;
    [SerializeField, Range(0.1f,5f)] private float redGraphSpeed;
    
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;
    
    // Local Variables
    // For some reason unity's rectTransform.rect.width does not return the width value shown
    // on the inspector, so im setting it manually
    private const float graphWidth = 256f;
    private float currentSpeed = 0f;
    
    void Start()
    {
        // Subscribe to events
        PlayerObject.Instance.OnHealthChange += OnPlayerHealthChanged;
        
        // Set variables
        heart.sprite = hearts[PlayerObject.Instance.currentHealth];
        SetGraphStatus();
    }

    private void Update()
    {
        // Move the beat graphs
        foreach (Image graph in beatGraphs)
        {
            Vector2 newPosition = graph.rectTransform.anchoredPosition;
            newPosition.x -= currentSpeed * Time.deltaTime;
            graph.rectTransform.anchoredPosition = newPosition;
        }
        
        // Check if the first beat graph went off view
        if (beatGraphs[0].rectTransform.anchoredPosition.x <= -graphWidth)
        {
            // It did, add it to the end
            Image poppedGraph = beatGraphs[0];
            beatGraphs.RemoveAt(0);
            beatGraphs.Add(poppedGraph);
            Vector2 newPosition = poppedGraph.rectTransform.anchoredPosition;
            newPosition.x = graphWidth;
            poppedGraph.rectTransform.anchoredPosition = newPosition;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe to events
        PlayerObject.Instance.OnHealthChange -= OnPlayerHealthChanged;
    }

    /// <summary>
    /// Function that will be called whenever the player's health changes
    /// </summary>
    /// <param name="health"></param>
    private void OnPlayerHealthChanged(int health)
    {
        // Check if we need to change the graph speed
        SetGraphStatus();
        
        // Set the heart UI, its okay not to shift the index due to the hearts starting at 0
        heart.sprite = hearts[health];
    }
    
    /// <summary>
    /// Function to call to check if we need to change the speed of the graphs and color
    /// </summary>
    private void SetGraphStatus()
    {
        if (PlayerObject.Instance.currentHealth <= redGraphUpperBound)
        {
            // Player fell on red
            currentSpeed = redGraphSpeed * graphSpeedMultiplier;
            ChangeGraphColor(redGraphColor);
        }
        else if (PlayerObject.Instance.currentHealth <= yellowGraphUpperBound)
        {
            // Player fell on yellow
            currentSpeed = yellowGraphSpeed * graphSpeedMultiplier;
            ChangeGraphColor(yellowGraphColor);
        }
        else
        {
            currentSpeed = greenGraphSpeed * graphSpeedMultiplier;
            ChangeGraphColor(greenGraphColor);
        }
    }

    /// <summary>
    /// Helper func to change the color of all the graphs
    /// </summary>
    private void ChangeGraphColor(Color targetColor)
    {
        foreach (Image graph in beatGraphs)
        {
            graph.color = targetColor;
        }
    }
}
