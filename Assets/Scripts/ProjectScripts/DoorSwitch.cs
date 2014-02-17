using UnityEngine;
using System.Collections;

public class DoorSwitch : MonoBehaviour
{
	public Door door;
	bool playerOnSwitch;
	bool pressed;

	void Awake ()
	{
		if (door == null) {
			Debug.LogError ("DoorSwitch (" + name + ") has no associated door.");
		}
	}

	void OnTriggerEnter (Collider collider)
	{
		if (!playerOnSwitch && collider.CompareTag (Tags.PLAYER)) {
			playerOnSwitch = true;
			if (!pressed) {
				door.Open ();
				transform.position = transform.position - new Vector3 (0, 0.6f, 0);
			} else {
				transform.position = transform.position + new Vector3 (0, 0.6f, 0);
				door.Close ();
			}
			pressed = !pressed;
		}
	}

	void OnTriggerExit (Collider collider)
	{
		if (playerOnSwitch && collider.CompareTag (Tags.PLAYER)) {
			playerOnSwitch = false;
		}
	}
}
