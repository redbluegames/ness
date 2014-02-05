using UnityEngine;
using System.Collections;

public class FXParenter : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		StartCoroutine (ParentFXWhenLoaded ());
	}
	

	/// <summary>
	/// Wait until the FX have been loaded, thn iterate over all of our instantiated FX
	/// and move them into the FX POOL.
	/// </summary>
	IEnumerator ParentFXWhenLoaded ()
	{
		CFX_SpawnSystem fxSpawner = GetComponent<CFX_SpawnSystem> ();
		Transform container = GameObject.Find (ObjectNames.FX_POOL).transform;

		// Spin until the FX are loaded
		while (!CFX_SpawnSystem.AllObjectsLoaded) {
			yield return null;
		}

		// Parent all the FX objects
		for (int i= 0; i < fxSpawner.objectsToPreload.Length; ++i) {
			GameObject objectToMove = fxSpawner.objectsToPreload [i];
			GameObject instancedObj = CFX_SpawnSystem.GetNextObject (objectToMove, false);
			while (instancedObj.transform.parent != container) {
				instancedObj.transform.parent = container;
				instancedObj = CFX_SpawnSystem.GetNextObject (objectToMove, false);
			}
		}
	}
}
