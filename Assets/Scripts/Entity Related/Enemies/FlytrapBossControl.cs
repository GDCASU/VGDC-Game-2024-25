using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class FlytrapBossControl : EntityScript
{
    [SerializeField] private GameObject _ratPrefab;

    private const float RAT_SPAWN_MIN_CD = 3f;
    private const float RAT_SPAWN_MAX_CD = 5f;
    private const float BURROW_MIN_CD = 7.5f;
    private const float BURROW_MAX_CD = 10f;
    private const float BURROW_DOWN_SPEED = 7f;
    private const float BURROW_UP_SPEED = 13f;
    private const float BURROW_TIME = 1f;
    private const float BURROW_ANTIC_TIME = 2f;

    private float _ratSpawnCooldown;
    private float _burrowCooldown;
    private bool _burrowing;

    private Animator _anim;
    private Collider _burrowHitbox;
    private Collider _bodyHitbox;
    private Transform _ratSpawnPoints;

	private new void Start()
	{
		_anim = transform.Find("Head").GetComponent<Animator>();
        _burrowHitbox = transform.Find("Hitbox").GetComponent<Collider>();
        _bodyHitbox = GetComponent<Collider>();
        _ratSpawnPoints = transform.Find("Rat Spawn Points");
		_ratSpawnCooldown = Random.Range(RAT_SPAWN_MIN_CD, RAT_SPAWN_MAX_CD);

        base.Start();
	}

	private void Update()
	{
        _ratSpawnCooldown -= Time.deltaTime;
        _burrowCooldown -= Time.deltaTime;

		if(!_burrowing)
        {
            // Spawn rat
            if(_ratSpawnCooldown < 0f)
            {
				GameObject rat = Instantiate(_ratPrefab, _ratSpawnPoints.GetChild(Random.Range(0, 4)).transform.position, Quaternion.identity);
                rat.SetActive(true);
                
				_ratSpawnCooldown = Random.Range(RAT_SPAWN_MIN_CD, RAT_SPAWN_MAX_CD);
			}

            // At below 50% hp, chase player by burrowing underneath them
            if((float)currentHealth / maxHealth <= 0.5f && _burrowCooldown < 0f)
            {
                StartCoroutine(Burrow());
            }
        }
	}

    private IEnumerator Burrow()
    {
        _burrowing = true;

		// Close and move underground, could use particles
		_anim.Play("close");

        float surfaceY = transform.position.y;
        float undergroundY = surfaceY - 7f;
        while(transform.position.y > undergroundY)
        {
            transform.position += BURROW_DOWN_SPEED * Vector3.down * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(BURROW_TIME);

        float targetX = PlayerObject.Instance.transform.position.x;
        float targetZ = PlayerObject.Instance.transform.position.z;
        transform.position = new Vector3(targetX, undergroundY, targetZ);

        // Show particles to telegraph?
        yield return new WaitForSeconds(BURROW_ANTIC_TIME);

        // Burrow up
        _anim.Play("close", -1, 0f);
        _burrowHitbox.enabled = true;
        _bodyHitbox.enabled = false; // Disable so the player doesn't get pushed up
		while(transform.position.y < surfaceY)
        {
			transform.position += BURROW_UP_SPEED * Vector3.up * Time.deltaTime;
			yield return null;
		}
        transform.position = new Vector3(targetX, surfaceY, targetZ);
        yield return _anim.PlayBlocking("open");

		_anim.Play("idle");
        _burrowHitbox.enabled = false;
        _bodyHitbox.enabled = true;
        _burrowCooldown = Random.Range(BURROW_MIN_CD, BURROW_MAX_CD);
		_burrowing = false;
		yield break;
    }
}
