using UnityEngine;
using System.Collections;

public class ChallengeRoom : MonoBehaviour
{
	public Door[] doors;
	Challenge challenge;
	bool closed;

	// Use this for initialization
	void Start ()
	{
		if (doors == null) {
			Debug.LogError ("ChallengeRoom (" + name + ") has no associated doors.");
		}
		challenge = GetComponent <Challenge> ();
		if (challenge == null) {
			Debug.LogError ("ChallengeRoom (" + name + ") has no associated challenge.");
		}
	}

	void Update ()
	{
		if (challenge.complete) {
			foreach (Door door in doors) {
				door.Open ();
			}
			gameObject.SetActive (false);
		}
	}
	
	void OnTriggerEnter (Collider collider)
	{
		if (!closed && collider.CompareTag (Tags.PLAYER)) {
			closed = true;
			foreach (Door door in doors) {
				door.Close ();
			}
			StartCoroutine (PollForComplete ());
		}
	}

	/// <summary>
	/// Checks on the challenge's criteria to see if it's been completed.
	/// Does not do this every frame.
	/// </summary>
	/// <returns>The for complete.</returns>
	IEnumerator PollForComplete ()
	{
		while (!challenge.complete) {
			challenge.TestForChallengeCompletion ();
			yield return new WaitForSeconds (2);
		}
	}
}
