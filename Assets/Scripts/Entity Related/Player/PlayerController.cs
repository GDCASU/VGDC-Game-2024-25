using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By: William Peng, Jacob Kaufman-Warner
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Pupose:
 * Handle the movement and input of the player
 */// --------------------------------------------------------


/// <summary>
/// Controls the player
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float _speed;
    [SerializeField] private GameObject _projectileNeutralPrefab;
    [SerializeField] private GameObject _projectileFirePrefab;
    [SerializeField] private GameObject _projectileWaterPrefab;
    [SerializeField] private GameObject _projectileSparksPrefab;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private Animator moveController;
    [SerializeField] private SpriteRenderer playerRenderer;

    [Header("Projectile ItemData")]
    [SerializeField] private ItemData fireAmmo;
    [SerializeField] private ItemData waterAmmo;
    [SerializeField] private ItemData sparksAmmo;


    private GameObject _projectilePrefab;
    private int magicChange = 0;
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool _doDebugLog;

    // Ian HACK: We probably shouldnt use the character controller for
    // this, so script the movement system ourselves later for more granular control
    [SerializeField] private CharacterController _characterController;

	private void Start()
	{
		InputManager.OnAttack += AttackAction;
        //InputManager.ChangeElement += ChangeElementAction;
        _projectilePrefab = _projectileNeutralPrefab;
	}

	// Update is called once per frame
	void Update()
    {
        // Get Vector2 Input from Input Manager
        Vector3 input = InputManager.Instance.movementInput;

        // Since the player could be slightly rotated from the
        // world axis, we compute the rotation
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // Compute the actual world space direction of movement for the player
        Vector3 moveDirWorldSpace = (forward * input.y + right * input.x).normalized;

        // Move Player
        _characterController.Move(Time.deltaTime * _speed * moveDirWorldSpace);

        // Play walking animation if moving
        moveController.SetBool("IsMoving", Mathf.Abs(input.x) > 0 || Mathf.Abs(input.y) > 0);

        //Set sprite direction
        if (input.x < 0)
        {
            playerRenderer.flipX = true;
        } else if (input.x > 0)
        {
            playerRenderer.flipX = false;
        }
    }

    private void OnDestroy()
    {
        // Un-subscribe from events
        InputManager.OnAttack -= AttackAction;
        //InputManager.ChangeElement -= ChangeElementAction;
    }

    private void AttackAction()
	{
        if (Camera.main == null) Debug.Log("CAMERA WAS NULL!");
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 100f))
		{
            if(InventorySystem.Instance.CheckForAmmo()){
                switch(InventorySystem.Instance.GetSelectedAmmo()){
                    case AmmoType.Fire:
                        Debug.Log("Fire ammo");
                        _projectilePrefab = _projectileFirePrefab;
                        InventorySystem.Instance.Remove(fireAmmo);
                        break;
                    case AmmoType.Water:
                    Debug.Log("Water ammo");
                        _projectilePrefab = _projectileWaterPrefab;
                        InventorySystem.Instance.Remove(waterAmmo);
                        break;
                    case AmmoType.Sparks:
                        Debug.Log("Sparks ammo");
                        _projectilePrefab = _projectileSparksPrefab;
                        InventorySystem.Instance.Remove(sparksAmmo);
                        break;
                    case AmmoType.None:
                        _projectilePrefab = _projectileNeutralPrefab;
                        break;
                    default:
                        Debug.Log("Error with ammo");
                        break;
                }
                
            }else{
                Debug.Log("Neutral ammo");
                _projectilePrefab = _projectileNeutralPrefab;
            }
            // Spawn projectile prefab and set its target to the raycast hit location
            GameObject projectile = Instantiate(_projectilePrefab);
            projectile.transform.position = _projectileSpawnPoint.position;
            projectile.GetComponent<Projectile>().target = hit.point;
		}
	}

    private void ChangeElementAction(){
        //change between 4 elements of nothing(aka physical prolly), fire, water, sparks. More of a test feature as the game seems to not include switching. Prolly
        magicChange++;
        magicChange%=4;
        switch(magicChange){
            case 0: 
                _projectilePrefab = _projectileNeutralPrefab;
                break;
            case 1:
                _projectilePrefab = _projectileFirePrefab;
                break;
            case 2:
                _projectilePrefab = _projectileWaterPrefab;
                break;
            case 3:
                _projectilePrefab = _projectileSparksPrefab;
                break;
            default:
                Debug.Log("Something broke, it is over. This is the issue ---> " + magicChange);
                break;
        }
    }

}
