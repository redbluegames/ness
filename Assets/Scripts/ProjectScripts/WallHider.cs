using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallHider : MonoBehaviour
{
	const float HIDESPEED = 0.25f;
	readonly Vector3 CASTOFFSET = new Vector3 (0, -0.2f, 0);
	List<GameObject> previouslyHiddenWalls = new List<GameObject> ();

	// Cached References
	Transform playerTransform;
	Transform cameraTransform;

	void Awake ()
	{
		GameObject player = GameObject.Find (SceneObjectNames.PLAYER);
		playerTransform = player.transform;
		cameraTransform = Camera.main.transform;
	}

	// Update is called once per frame
	void LateUpdate ()
	{
		Vector3 toPlayer = playerTransform.position - cameraTransform.position + 
			cameraTransform.TransformDirection (CASTOFFSET);
		Ray camToPlayer = new Ray (cameraTransform.position, toPlayer);
		//Debug.DrawRay (cameraTransform.position, toPlayer, Color.red);
		int onlyTempWalls = 1 << Layers.TEMP_WALL;
		RaycastHit[] hits = Physics.RaycastAll (camToPlayer, 25.0f, onlyTempWalls);
		FadeObstructingWallsOut (hits);
		FadeUnobstructingWallsIn (hits);
	}

	/// <summary>
	/// Fades all walls out that were hit in a provided raycast.
	/// </summary>
	/// <param name="obstructingWalls">Walls hit by a raycast from the camera towards the player.</param>
	void FadeObstructingWallsOut (RaycastHit[] obstructingWalls)
	{
		for (int i = 0; i < obstructingWalls.Length; ++i) {
			TweenFadeInOut fader = obstructingWalls [i].collider.gameObject.GetComponent<TweenFadeInOut> ();
			if (fader != null) {
				fader.FadeOut (HIDESPEED);
				previouslyHiddenWalls.Add (obstructingWalls [i].collider.gameObject);
			} else {
				Debug.LogWarning ("Encountered a temporary wall without a fader script at " +
					"position " + obstructingWalls [i].point);
			}
		}
	}

	/// <summary>
	/// Fades the walls in if they are no longer in the list of walls hit by
	/// our camera's raycast.
	/// </summary>
	/// <param name="obstructingWalls">Walls hit by a raycast from the camera towards the player.</param>
	void FadeUnobstructingWallsIn (RaycastHit[] obstructingWalls)
	{
		for (int i = previouslyHiddenWalls.Count-1; i >= 0; --i) {
			bool wallStillObstructing = false;
			for (int h = 0; h < obstructingWalls.Length; ++h) {
				if (previouslyHiddenWalls [i].Equals (obstructingWalls [h].collider.gameObject)) {
					wallStillObstructing = true;
				}
			}
			if (!wallStillObstructing) {
				TweenFadeInOut fader = previouslyHiddenWalls [i].GetComponent<TweenFadeInOut> ();
				fader.FadeIn (HIDESPEED);
				previouslyHiddenWalls.Remove (previouslyHiddenWalls [i]);
			}
		}
	}
}
