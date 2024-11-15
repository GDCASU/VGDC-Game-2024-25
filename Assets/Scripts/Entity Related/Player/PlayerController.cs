using UnityEngine;
using FMOD.Studio;

/* -----------------------------------------------------------
 * Author:
 * Ian Fletcher
 * 
 * Modified By: William Peng, Sameer Reza (Audio)
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
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectileSpawnPoint;
    [SerializeField] private Animator moveController;
    [SerializeField] private SpriteRenderer playerRenderer;
    
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
		InputManager.OnAttack += AttackAction;
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
    }

    private void AttackAction()
	{
        if (Camera.main == null) Debug.Log("CAMERA WAS NULL!");
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit, 100f))
		{
			// Spawn projectile prefab and set its target to the raycast hit location
			GameObject projectile = Instantiate(_projectilePrefab);
			projectile.transform.position = _projectileSpawnPoint.position;
			projectile.GetComponent<Projectile>().target = hit.point;
            AudioManager.Instance.PlayEventNoDuplicate(_playerAttackSFX);
		} 
	}

}
