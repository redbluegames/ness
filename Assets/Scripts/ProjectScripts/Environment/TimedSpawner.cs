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
			// Trigger OnEnable
			ActivateSpawner ();
		}
	}

	/// <summary>
	/// Deactivates the spawner.
	/// </summary>
	[Signal]
	void DeactivateSpawner ()
	{
		spawnActive = false;
		StopAllCoroutines ();
	}

	/// <summary>
	/// Activates the spawner.
	/// </summary>
	[Signal]
	void ActivateSpawner ()
	{
		spawnActive = true;
		StartCoroutine (PeriodicSpawn ());
	}

	/// <summary>
	/// Counts how many objects have been spawned.
	/// </summary>
	/// <returns>The number of children objects aka spawned objects.</returns>
	int CountActive ()
	{
		return transform.childCount;
	}

	IEnumerator PeriodicSpawn ()
	{
		while (spawnActive) {
			Debug.Log ("PeriodicSpawn running....");
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
}
