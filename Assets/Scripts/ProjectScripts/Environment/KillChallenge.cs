using UnityEngine;
using System.Collections;

public class KillChallenge : Challenge
{
	public GameObject[] enemiesToKill;

	void Start ()
	{
		if (enemiesToKill.Length == 0) {
			Debug.LogError ("KillChallenge attached to " + gameObject.name + " without wiring up" +
				" any enemies to kill.");
		}
	}

	#region implemented abstract members of Challenge
	public override bool TestForChallengeCompletion ()
	{
		foreach (GameObject enemy in enemiesToKill) {
			if (enemy == null) {
				continue;
			} else if (enemy.activeInHierarchy) {
				complete = false;
				return complete;
			}
		}
		complete = true;
		return complete;
	}
	#endregion
}
