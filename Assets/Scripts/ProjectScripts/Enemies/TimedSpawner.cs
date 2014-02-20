using UnityEngine;
using System.Collections;

public class TimedSpawner : MonoBehaviour
{
	// Configuration
	public GameObject prefabToSpawn;
	public float spawnFrequency = 2.0f;
	public int maxActiveEnemies = 10;
	public bool activateOnStart;
	public bool spawnActive { get; private set; }

	int activeEnemiesSpawned;

	// Use this for initialization
	void Start ()
	{
		activeEnemiesSpawned = 0;
		if (activateOnStart) {
			ActivateSpawner ();
		} else {
			gameObject.SetActive (false);
		}
	}

	/// <summary>
	/// When timedSpawner gets activated, start spawner.
	/// </summary>
	void OnEnable ()
	{
		ActivateSpawner ();
	}

	/// <summary>
	/// Deactivates the spawner.
	/// </summary>
	public void DeactivateSpawner ()
	{
		spawnActive = false;
	}

	/// <summary>
	/// Activates the spawner.
	/// </summary>
	public void ActivateSpawner ()
	{
		spawnActive = true;
		StartCoroutine (PeriodicSpawn ());
	}

	int CountActive ()
	{
		return GetComponentsInChildren<Enemy> ().Length - 1;
	}

	IEnumerator PeriodicSpawn ()
	{
		while (spawnActive) {
			if (activeEnemiesSpawned < maxActiveEnemies) {
				GameObject newEnemy = (GameObject)Instantiate (prefabToSpawn, transform.position,
			                                               Quaternion.identity);
				newEnemy.transform.parent = transform;
				activeEnemiesSpawned++;
			} else {
				activeEnemiesSpawned = CountActive ();
			}
			yield return new WaitForSeconds (spawnFrequency);
		}
		yield break;
	}
	void OnDestroy ()
	{
		StopAllCoroutines ();
	}
	
}
