using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelBurstProjectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 10f;
    [SerializeField] private int damage = 1;
    [SerializeField] private Elements element;

    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    // Local variables
    [HideInInspector] public string ownerTag = "";
    [HideInInspector] public Vector3 moveDir = Vector3.zero;

    private void Start()
    {
        // Destroy after lifetime passes
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move
        transform.Translate(moveDir * (speed * Time.deltaTime));
    }

    private void OnTriggerEnter(Collider other)
    {

        if (doDebugLog) Debug.Log(gameObject.name + " hit " + other.gameObject.name);

        // Dont collide if on the same tag
        if (other.CompareTag(ownerTag)) return; // Same tag, dont damage owner

        if (other.TryGetComponent(out InteractionManager manager)) return;

        // Try to damage the other object
        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable == null)
        {
            // Other Gameobject was not Damageable

            // Element Specific Behaviours
            switch (element)
            {
                // This is just here for possible future changes
                default:
                    break;
            }

            // Might move this into the switch later in possible future changes
            Destroy(gameObject);
            return;
        }
        // Otherwise it did, deal damage and see if we need to generate a reaction
        ReactionType reaction = damageable.TakeDamage(damage, element);

        // There is a reaction do perform
        // TODO: GENERATE REACTION IN THE WORLD
        switch (reaction)
        {
            case ReactionType.Fireworks:
                //
                break;

            case ReactionType.Undefined:
                break;
        }

        // Element Specific End Behaviours
        switch (element)
        {
            case Elements.Fire:
                break;

            default:
                Destroy(gameObject);
                break;
        }
    }
}
