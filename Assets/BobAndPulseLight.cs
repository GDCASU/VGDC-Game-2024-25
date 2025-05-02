using UnityEngine;

[RequireComponent(typeof(Light))]
public class BobAndDynamicPulseLight : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobAmplitude = 0.5f;
    public float bobFrequency = 1f;

    [Header("Light Pulse Settings")]
    public float lightMinIntensity = 0.5f;
    public float lightMaxIntensity = 2f;
    public float pulseFrequencyMin = 0.3f;
    public float pulseFrequencyMax = 1.2f;
    public float pulseChangeInterval = 3f;
    public float pulseTransitionSpeed = 0.5f; // How fast to approach target

    private Vector3 startPos;
    private Light myLight;

    private float currentPulseFrequency;
    private float targetPulseFrequency;
    private float pulseChangeTimer;

    void Start()
    {
        startPos = transform.position;
        myLight = GetComponent<Light>();

        currentPulseFrequency = Random.Range(pulseFrequencyMin, pulseFrequencyMax);
        targetPulseFrequency = currentPulseFrequency;
    }

    void Update()
    {
        // Bobbing motion
        float yOffset = Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(startPos.x, startPos.y + yOffset, startPos.z);

        // Handle dynamic pulse frequency
        pulseChangeTimer += Time.deltaTime;
        if (pulseChangeTimer >= pulseChangeInterval)
        {
            pulseChangeTimer = 0f;
            targetPulseFrequency = Random.Range(pulseFrequencyMin, pulseFrequencyMax);
        }

        // Smooth transition to new pulse frequency
        currentPulseFrequency = Mathf.MoveTowards(currentPulseFrequency, targetPulseFrequency, Time.deltaTime * pulseTransitionSpeed);

        // Light pulsing
        float pulse = Mathf.Sin(Time.time * currentPulseFrequency) * 0.5f + 0.5f;
        myLight.intensity = Mathf.Lerp(lightMinIntensity, lightMaxIntensity, pulse);
    }
}
