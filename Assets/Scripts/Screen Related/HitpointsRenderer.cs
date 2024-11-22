using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Buffers.Text;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using Unity.VisualScripting;

/// <summary> Script used to display damage numbers on the game </summary>
public class HitpointsRenderer : MonoBehaviour
{
    [Header("Hitpoints Container")]
    [SerializeField] private GameObject HitPointPrefab;

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
        // Get the rect transform of the hitpoint container
        hitpointContainerRect = GetComponent<RectTransform>();

        // Handle Singleton
        if (Instance != null) { Destroy(gameObject); }
        Instance = this;
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

    /// <summary> Display the damage number on the screen canvas </summary>
    public void PrintDamage(Vector3 entityPos, int damage, Color colorIn)
    {
        //Dont do anything if disabled or damage is less than 1
        if (DisableHitpoints || (damage < 1) ) { return; }

        // Convert enemy position from world space to screen space
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(entityPos);

        // Use ScreenPointToWorldPointInRectangle to convert screen space point to canvas space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(hitpointContainerRect, screenPoint, Camera.main, out Vector2 canvasHitPoint);

        // Create the Hitpoint
        GameObject hitpoint = Instantiate(HitPointPrefab, canvasHitPoint, Quaternion.identity, this.transform);
        
        // Since the Instantiate function uses world space, adjust to local space in canvas
        hitpoint.GetComponent<RectTransform>().localPosition = canvasHitPoint;

        // Get the TextMesh Component
        TMP_Text hitpointMesh = hitpoint.GetComponent<TMP_Text>();

        // Set the data
        hitpointMesh.text = damage.ToString();
        hitpointMesh.color = colorIn;

        // Animation coroutine
        if(gameObject.activeInHierarchy) StartCoroutine(AnimateHitpoint(hitpoint));
    }

    //Animation Coroutine
    private IEnumerator AnimateHitpoint(GameObject hitpoint)
    {
        float timeElapsed = 0f;
        float speed = startingSpeed;
        float newSpeed;

        // Randomize a Direction Vector and make it always have a pos Y so they go up
        Vector2 direction = Random.insideUnitCircle.normalized;
        direction.y = Mathf.Abs(direction.y);

        //Animates the hitpoint going up using gravity
        while (timeElapsed < hitpointStayTime)
        {
            //Animation
            newSpeed = Mathf.Lerp(speed, 0, acceleration);
            hitpoint.transform.Translate(speed * Time.deltaTime * direction);
            speed = newSpeed;

            timeElapsed += Time.deltaTime;

            //Wait 1 frame
            yield return null;
        }

        //Destroy the hitpoint object
        Destroy(hitpoint);
    }
}
