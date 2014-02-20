using UnityEngine;
using System.Collections;

public class ActivateObjectSwitch : MonoBehaviour
{
	public GameObject[] objectsToActivate;
	public bool switchActive { get; private set; }

	// Use this for initialization
	void Start ()
	{
		if (objectsToActivate.Length == 0) {
			Debug.Log ("ActivateObjectSwitch not wired up with any objects");
		}
		switchActive = true;
	}

	/// <summary>
	/// When player enters the trigger, perform a one time object activation.
	/// </summary>
	/// <param name="collider">Collider.</param>
	void OnTriggerEnter (Collider collider)
	{
		if (switchActive && collider.CompareTag (Tags.PLAYER)) {
			switchActive = false;
			foreach (GameObject obj in objectsToActivate) {
				obj.SetActive (true);
			}
		}
	}

}
