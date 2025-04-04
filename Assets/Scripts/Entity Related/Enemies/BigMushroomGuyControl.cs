using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author: William
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose: Control script for big mushroom guy enemy
 * 
 */// --------------------------------------------------------


/// <summary>
/// Control script for big mushroom guy
/// </summary>
public class BigMushroomGuyControl : MonoBehaviour
{
    [Header("Control")]
    [SerializeField] private float _meleeAttackDistance = 2f;
    [SerializeField] private float _rangedAttackCooldown = 3f;
    [SerializeField] private float _rangedAttackRadius = 2f;

    [Header("Attack Prefabs")]
    [SerializeField] private GameObject _meleeAttackPrefab;
	[SerializeField] private GameObject _rangedAttackPrefab;

	private AIDestinationSetter _aiDestinationSetter;
    private AIPath _aiPath;
    private Transform _projectileSpawnPoint;
    private bool _attacking;

	void Start()
    {
        _aiDestinationSetter = GetComponent<AIDestinationSetter>();
		_aiPath = GetComponent<AIPath>();
        _projectileSpawnPoint = transform.Find("Projectile Spawn Point");

		StartCoroutine(RangedAttack());
    }

    void Update()
    {
        if(!_attacking)
        {
            float distToPlayer = Vector3.Distance(PlayerObject.Instance.transform.position, transform.position);
            if(distToPlayer < _meleeAttackDistance)
            {
                StartCoroutine(MeleeAttack());
            }
        }
    }

    private IEnumerator RangedAttack()
    {
        while(gameObject)
        {
            // Wait to detect player
            yield return new WaitUntil(() => !_attacking && _aiDestinationSetter.target != null);

            float angle = Random.Range(0f, 2f * Mathf.PI);
            float offset = Random.Range(0f, _rangedAttackRadius);
            Vector3 offsetFromPlayer = new Vector3(offset * Mathf.Cos(angle), 0f, offset * Mathf.Sin(angle));
            Vector3 target = PlayerObject.Instance.transform.position + offsetFromPlayer;

			GameObject rangedAttack = Instantiate(_rangedAttackPrefab, _projectileSpawnPoint.position, Quaternion.identity);
            rangedAttack.GetComponent<MushroomProjectile>().target = target;
			rangedAttack.SetActive(true);

            yield return new WaitForSeconds(_rangedAttackCooldown);
		}
    }

    private IEnumerator MeleeAttack()
    {
        _attacking = true;
        _aiPath.canMove = false;

        // Play melee attack telegraph and animation
        yield return new WaitForSeconds(1f);

        //GameObject meleeAttack = Instantiate(_meleeAttackPrefab, transform.position, Quaternion.identity);
        //meleeAttack.SetActive(true);

		_attacking = false;
        _aiPath.canMove = true;
    }
}
