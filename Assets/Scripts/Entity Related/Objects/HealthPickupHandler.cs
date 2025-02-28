using UnityEngine;

/* -----------------------------------------------------------
 * Author:
 * Sam Cahill
 *
 * Modified By:
 *
 */ // --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Handle health pickup timeout destruction and blinking
 */ // --------------------------------------------------------

public class HealthPickupHandler : MonoBehaviour
{
    [Header("Cooldown")]
    public float cooldown = 5f;

    [Range(0, 10)]
    public float blinkSpeed = 3;

    [Range(0, 10)]
    public float blinkTime = 2;

    private float timeRemaining;
    private float timeSpawned;

    // Start is called before the first frame update
    void Start()
    {
        timeSpawned = Time.time;
        timeRemaining = cooldown;
    }

    // Update is called once per frame
    void Update()
    {
        timeRemaining -= Time.deltaTime;

        //Destroy
        if (timeRemaining <= 0 )
        {
            Destroy( gameObject );
        }
        //Flashing
        else if ( timeRemaining < blinkTime )
        {
            Renderer rend = this.gameObject.GetComponent<Renderer>();

            float lerpValue = Mathf.PingPong((Time.time - timeSpawned) * blinkSpeed * (timeRemaining / blinkTime), 1);
            Color color = Color.Lerp(Color.red, Color.white, lerpValue);

            rend.material.color = color;
        }
    }
}
