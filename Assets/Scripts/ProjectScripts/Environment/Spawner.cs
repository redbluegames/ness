using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
	public GameObject[] enemies;
	public Transform[] spawnLocations;
	public bool testSpawn;
	GameObject prefabContainer;
	bool spawnReady;

	void Start ()
	{
		spawnReady = true;
		prefabContainer = GameObject.Find (SceneObjectNames.DYNAMIC_OBJECTS);
	}

	// Update is called once per frame
	void Update ()
	{
		if (testSpawn) {
			testSpawn = false;
			SpawnWave ();
		}
	}

	/// <summary>
	/// Triggers a spawn on enter.
	/// </summary>
	void OnTriggerEnter (Collider collider)
	{
		if (collider.CompareTag (Tags.PLAYER)) {
			if (spawnReady) {
				SpawnWave ();
			}
		}
	}

	/// <summary>
	/// Allows for another spawn to occur if player leaves trigger.
	/// </summary>
	void OnTriggerExit ()
	{
		if (collider.CompareTag (Tags.PLAYER)) {
			spawnReady = true;
		}
	}

	/// <summary>
	/// Instantiate every enemy in our array of enemies and parent them to
	/// the dynamic objects container. Spawn them at a provided location or
	/// if none is provided, at the spawner.
	/// </summary>
	void SpawnWave ()
	{
		Vector3 spawnAt;

		Vector3 randSpacing;
		// Note that the Standard Assets has a different solution that involves object pooling
		// multiple caches of enemies. Consider looking at that when we need a more
		// robust system.
		int nextSpawnLocation = 0;
		for (int i = 0; i < enemies.Length; i++) {
			// Cycle through our spawn locations
			if (spawnLocations [nextSpawnLocation] != null) {
				spawnAt = spawnLocations [nextSpawnLocation].position;
			} else {
				spawnAt = transform.position;
			}
			nextSpawnLocation = (nextSpawnLocation + 1) % spawnLocations.Length;

			// Spawn an enemy at the location with some randomness.
			GameObject newEnemy = (GameObject)Instantiate (enemies [0]);
			newEnemy.transform.parent = prefabContainer.transform;
			// Arrange our enemies randomly
			randSpacing = new Vector3 (Random.Range (0, 2), 0, Random.Range (0, 2));
			newEnemy.transform.position = spawnAt + randSpacing;
		}
	}
}
