using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallHider : MonoBehaviour
{
	const float HIDESPEED = 0.3f;
	readonly Vector3 CASTOFFSET = new Vector3 (0, -0.2f, 0);
	List<GameObject> previouslyHidden = new List<GameObject> ();

	// Cached References
	Transform playerTransform;
	Transform cameraTransform;

	void Awake ()
	{
		GameObject player = GameObject.Find (ObjectNames.PLAYER);
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
		for (int i = 0; i < hits.Length; ++i) {
			TweenFadeInOut fader = hits [i].collider.gameObject.GetComponent<TweenFadeInOut> ();
			if (fader == null) {
				Debug.LogWarning ("Encountered a temporary wall without a fader script at " +
					"position " + hits [i].point);
				continue;
			}
			if (hits [i].point.z < playerTransform.position.z) {
				fader.FadeOut (HIDESPEED);
				previouslyHidden.Add (hits [i].collider.gameObject);
			}
		}

		for (int i = previouslyHidden.Count-1; i >= 0; --i) {
			bool containsWall = false;
			foreach (RaycastHit hit in hits) {
				if (previouslyHidden [i].Equals (hit.collider.gameObject)) {
					containsWall = true;
				}
			}
			if (!containsWall) {
				TweenFadeInOut fader = previouslyHidden [i].GetComponent<TweenFadeInOut> ();
				fader.FadeIn (HIDESPEED);
				previouslyHidden.Remove (previouslyHidden [i]);
			}
		}
	}

	IEnumerator FadeWallIn (TweenFadeInOut fader)
	{
		while (fader.isFadingIn) {
			yield return null;
		}
		fader.FadeIn (HIDESPEED);
	}
}
