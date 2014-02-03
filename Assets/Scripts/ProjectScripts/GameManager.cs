using UnityEngine;
using System.Collections;

/*
 * Code pattern for this class borrowed from:
 * http://wiki.unity3d.com/index.php/Singleton
 *
 * This is our "god class" for handling managers and game-global variables.
 **/
public class GameManager : Singleton<GameManager>
{
	public TimeManager GameTime { get; private set; }
	public LayerMask LineOfSightMask;

	// guarantee this will be always a singleton only - can't use the constructor!
	protected GameManager ()
	{
	}

	void Awake ()
	{
		GameTime = gameObject.AddComponent ("TimeManager") as TimeManager;
	}
}
