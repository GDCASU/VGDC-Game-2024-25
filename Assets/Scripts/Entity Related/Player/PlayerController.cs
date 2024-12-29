using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;
using FMOD.Studio;


/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By: William Peng, Jacob Kaufman-Warner, Sameer Reza (Audio)
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
    // Time since movement in the current direction started
    private float _elapsedMovementTime = 0;

    private Vector3 _previousInputVector = Vector3.zero;

    [Header("Values")]
    [SerializeField] private float _speedBeforeAcceleration;
    [SerializeField] private float _speedAfterAcceleration;
    [SerializeField] private float _delayBeforeAcceleration;
    [SerializeField] private float _durationOfAcceleration;
    [SerializeField] private GameObject _projectileNeutralPrefab;
    [SerializeField] private GameObject _projectileFirePrefab;
    [SerializeField] private GameObject _projectileWaterPrefab;
    [SerializeField] private GameObject _projectileSparksPrefab;
    [SerializeField] private GameObject _projectileSporePrefab;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private Animator moveController;
    [SerializeField] private SpriteRenderer playerRenderer;

    [Header("Projectile ItemData")]
    [SerializeField] private ItemData fireAmmo;
    [SerializeField] private ItemData waterAmmo;
    [SerializeField] private ItemData sparksAmmo;
    [SerializeField] private ItemData sporeAmmo;


    private GameObject _projectilePrefab;
    private int magicChange = 0;
    
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool _doDebugLog;

    // Ian HACK: We probably shouldnt use the character controller for
    // this, so script the movement system ourselves later for more granular control
    [SerializeField] private CharacterController _characterController;

    #region Audio References
    private EventInstance _playerFootstepSFX;
    private EventInstance _playerAttackSFX;
    #endregion

	private void Start()
	{
        PlayerData.Instance.PlayerController = this;

		InputManager.OnAttack += AttackAction;
        InputManager.ChangeElement += ChangeElementAction;
        _projectilePrefab = _projectileNeutralPrefab;
        _playerFootstepSFX = AudioManager.Instance.CreateEventInstance(FMODEvents.instance.playerFootstepSFX);
        _playerAttackSFX = AudioManager.Instance.CreateEventInstance(FMODEvents.instance.playerAttackSFX);
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

        // Set movement duration
        if (input == _previousInputVector) {
            _elapsedMovementTime += Time.deltaTime;
        } else {
            _elapsedMovementTime = 0;
            _previousInputVector = input;
        }

        // Move Player
        float finalRelativeSpeedIncrease = (_speedAfterAcceleration / _speedBeforeAcceleration) - 1;
        float relativeSpeedIncrease = finalRelativeSpeedIncrease * Mathf.Clamp01((_elapsedMovementTime - _delayBeforeAcceleration) / _durationOfAcceleration);
        float speedMultiplier = 1 + relativeSpeedIncrease;
        _characterController.Move(Time.deltaTime * _speedBeforeAcceleration * speedMultiplier * moveDirWorldSpace);

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

        UpdateSound();
    }

    private void UpdateSound()
    {
        //if moving, play the footstep sound
        if (Mathf.Abs(InputManager.Instance.movementInput.x) > 0)
        {
            AudioManager.Instance.PlayEventNoDuplicate(_playerFootstepSFX);
        }
        else
        {
            _playerFootstepSFX.stop(STOP_MODE.ALLOWFADEOUT);
        }
    }

    private void OnDestroy()
    {
        // Un-subscribe from events
        InputManager.OnAttack -= AttackAction;
        InputManager.ChangeElement -= ChangeElementAction;

		PlayerData.Instance.PlayerController = null;
	}

    private void AttackAction()
	{
        if (Camera.main == null) Debug.Log("CAMERA WAS NULL!");

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
        // Every scene must have a ground object to raycast onto
        if(Physics.Raycast(ray, out hit, 100f, LayerMask.GetMask(new string[] { "Ground" })))
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
                    case AmmoType.Spore:
                        Debug.Log("Spore ammo");
                        _projectilePrefab = _projectileSporePrefab;
                        InventorySystem.Instance.Remove(sporeAmmo);
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
            // Spawn projectile prefab
            GameObject projectile = Instantiate(_projectilePrefab);
            projectile.SetActive(false);
			projectile.transform.position = _projectileSpawnPoint.position;
			
            // Set target to the point on the ray that matches the y position of the spawn point
			float hitY = hit.point.y;
			float targetY = _projectileSpawnPoint.position.y;
            Vector3 rayVector = ray.direction.normalized;
			Vector3 target = hit.point + rayVector / rayVector.y * Mathf.Abs(targetY - hitY);

            //Debug.DrawRay(ray.origin, 1000f * ray.direction, Color.red, 2.5f);
            //Debug.DrawRay(hit.point, rayVector / rayVector.y * Mathf.Abs(targetY - hitY), Color.green, 5f);
            projectile.GetComponent<Projectile>().target = target;
			projectile.SetActive(true);
            AudioManager.Instance.PlayEventNoDuplicate(_playerAttackSFX);

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
