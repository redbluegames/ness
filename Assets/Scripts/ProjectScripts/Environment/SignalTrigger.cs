using UnityEngine;
using System.Collections;

public class SignalTrigger : MonoBehaviour
{
	public Signal onPress;
	public bool switchActive { get; private set; }

	// Use this for initialization
	void Start ()
	{
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
			onPress.Invoke ();
		}
	}

}
