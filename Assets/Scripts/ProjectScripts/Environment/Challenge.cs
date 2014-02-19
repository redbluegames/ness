using UnityEngine;
using System.Collections;

public abstract class Challenge : MonoBehaviour
{
	public bool complete { get; protected set; }

	abstract public bool TestForChallengeCompletion ();
}
