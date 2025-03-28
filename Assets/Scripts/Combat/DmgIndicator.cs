using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgIndicator : MonoBehaviour
{
    [Header("Configuration")]
    public float lifetime = 5.0f;
    public TMPro.TMP_Text textDisplay;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);

        rb.AddForce(Vector3.up * 2);
    }

    public void InitIndicator(Color color, int amt, TMPro.TMP_FontAsset font = null)
    {
        textDisplay.color = color;
        textDisplay.text = amt.ToString();
        
        if(font != null)
            textDisplay.font = font;
    }
}
