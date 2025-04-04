using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Buffers.Text;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;

/// <summary> Script used to display damage numbers on the game </summary>
public class HitpointsRenderer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject HitPointPrefab;
    [SerializeField] private GameObject HitPointsParent;

    [Header("Hitpoints Settings")]
    [SerializeField] private float acceleration = 0.01f;
    [SerializeField] private float startingSpeed = 5;
    [SerializeField] private float hitpointStayTime = 1f;

    [Header("Other Tools")]
    [SerializeField] private bool DisableHitpoints;
    [SerializeField] private bool TestAnimation;

    /// <summary> HitpointsRenderer's Singleton </summary>
    [HideInInspector] public static HitpointsRenderer Instance;

    //Local Variables
    private RectTransform hitpointContainerRect;
    


    void Awake()
    {
        // Set the Singleton
        if (Instance != null && Instance != this)
        {
            // Already set, destroy this object
            Destroy(gameObject);
            return;
        }
        // Not set yet
        Instance = this;
    }
    
    void OnDestroy()
    {
        // Null singleton
        if (Instance == this) Instance = null;
    }

    //Testing
    void Update()
    {
        if (TestAnimation)
        {
            PrintDamage(Vector3.zero, 1, Color.red);
            TestAnimation = false;
        }
    }

    /// <summary>
    /// Spawns a floating damage text in world space using a world-space TextMesh Pro object.
    /// </summary>
    /// <param name="entityPos"> Where to spawn the hitpoint text in the world. </param>
    /// <param name="damage"> Damage value to show. </param>
    /// <param name="colorIn"> Color of the damage text. </param>
    public void PrintDamage(Vector3 entityPos, int damage, Color colorIn)
    {
        if (DisableHitpoints || damage < 1) return;

        // Instantiate the 3D text directly in world space
        GameObject hitpoint = Instantiate(
            HitPointPrefab, 
            entityPos, 
            Quaternion.identity,
            HitPointsParent.transform
        );

        // Get the text component and assign damage text & color
        TMP_Text hitpointMesh = hitpoint.GetComponent<TMP_Text>();
        hitpointMesh.text = damage.ToString();
        hitpointMesh.color = colorIn;

        // Start the animation coroutine
        if (gameObject.activeInHierarchy)
            StartCoroutine(AnimateHitpoint(hitpoint));
    }

    /// <summary>
    /// Simple upward floating + fade-out or destroy logic for the spawned text.
    /// </summary>
    private IEnumerator AnimateHitpoint(GameObject hitpoint)
    {
        float timeElapsed = 0f;
        float speed = startingSpeed;

        // Get a random angle between 60° and 120°
        float angleDeg = Random.Range(60f, 120f);
        float angleRad = angleDeg * Mathf.Deg2Rad;

        // Convert that angle to a direction in the X–Y plane, ensuring it points up
        Vector2 direction2D = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        Vector3 direction = new Vector3(direction2D.x, direction2D.y, 0f);

        while (timeElapsed < hitpointStayTime)
        {
            float newSpeed = Mathf.Lerp(speed, 0f, acceleration);
            hitpoint.transform.Translate(direction * (speed * Time.deltaTime), Space.World);
            speed = newSpeed;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Destroy the text object
        Destroy(hitpoint);
    }
}
