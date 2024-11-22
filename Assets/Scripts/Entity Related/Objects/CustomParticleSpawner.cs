using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* -----------------------------------------------------------
 * Author: William Peng
 * 
 * 
 * Modified By:
 * 
 */// --------------------------------------------------------

/* -----------------------------------------------------------
 * Purpose: Provide an interface for objects to easily spawn particles on certain events
 * 
 */// --------------------------------------------------------


/// <summary>
/// Spawns particles on certain events
/// </summary>
public class CustomParticleSpawner : MonoBehaviour
{
    // Use this bool to gate all your Debug.Log Statements please
    [Header("Debugging")]
    [SerializeField] private bool doDebugLog;

    [SerializeField] private CustomParticleSpawnerData[] awakeParticles;
    [SerializeField] private CustomParticleSpawnerData[] onEnableParticles;
    [SerializeField] private CustomParticleSpawnerData[] onDestroyParticles;

	private void Awake()
	{
		foreach(CustomParticleSpawnerData data in awakeParticles)
		{
			Destroy(Instantiate(data.particle, transform.position, Quaternion.identity), data.destroyAfter);
		}
	}

	private void OnEnable()
	{
		foreach(CustomParticleSpawnerData data in onEnableParticles)
		{
			Destroy(Instantiate(data.particle, transform.position, Quaternion.identity), data.destroyAfter);
		}
	}

	private void OnDestroy()
	{
		foreach(CustomParticleSpawnerData data in onDestroyParticles)
		{
			Destroy(Instantiate(data.particle, transform.position, Quaternion.identity), data.destroyAfter);
		}
	}
}

[Serializable]
public struct CustomParticleSpawnerData
{
	// The particle to spawn
	public GameObject particle;
	// Destroy the particle after this amount of time
	public float destroyAfter;
}
