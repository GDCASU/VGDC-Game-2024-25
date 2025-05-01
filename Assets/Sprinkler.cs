using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinkler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject waterParticleSystem;
    [SerializeField] private GameObject[] sprinklersVisuals;

    [Header("Timers and Cooldowns")]
    [SerializeField] private float sprinklingTime = 5.0f;
    [SerializeField] private float sprinklerCooldown = 3.0f;
    [SerializeField] private float sprinklerRotationSpeed = -2.0f;

    private bool activated = false;
    private bool onCooldown = false;

    private void Update()
    {
        if (activated)
        {
            foreach (GameObject sprinker in sprinklersVisuals)
            {
                sprinker.transform.Rotate(0, 0, sprinklerRotationSpeed * Time.deltaTime);
            }
        }
    }

    public void OnGamFireworkReaction()
    {
        if (onCooldown)
            return;

        if (activated)
            return;

        activated = true;

        // apply water effect to barriers
        
        foreach(GameObject sprinker in sprinklersVisuals)
        {
            GameObject particleSystem = Instantiate(waterParticleSystem, sprinker.transform.position, sprinker.transform.rotation);
            Destroy(particleSystem, sprinklingTime);

            StartCoroutine(DelayedFunction(sprinklingTime, () =>
            {
                activated = false;
                onCooldown = true;
                sprinker.transform.rotation = Quaternion.identity;

                StartCoroutine(DelayedFunction(sprinklerCooldown, () => onCooldown = false));
            }));
        }
    }

    public IEnumerator DelayedFunction(float delay, System.Action func)
    {
        yield return new WaitForSeconds(delay);
        func.Invoke();
    }
}
