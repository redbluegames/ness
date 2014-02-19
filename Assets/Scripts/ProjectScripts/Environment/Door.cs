using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{
	public float moveTime = 2.0f;
	public bool startOpen;
	public bool rumbleOnOpen;
	public bool rumbleOnClose;
	Vector3 closedPosition;
	Vector3 openPosition;
	float height;

	const float PADDING = 0.01f;

	public bool isMoving { get; private set; }

	void Start ()
	{
		height = 2 * (gameObject.renderer.bounds.extents.y);
		closedPosition = transform.position;
		openPosition = closedPosition + new Vector3 (0, -height + PADDING, 0);

		if (startOpen) {
			Open ();
		}
	}

	/// <summary>
	/// Open the door. Do nothing if it's already moving up or down.
	/// </summary>
	public void Open ()
	{
		if (!isMoving) {
			if (rumbleOnOpen) {
				Rumble ();
			}
			StartCoroutine (MoveDoor (openPosition));
		}
	}

	/// <summary>
	/// Close the door. Do nothing if it's already moving up or down.
	/// </summary>
	public void Close ()
	{
		if (!isMoving) {
			if (rumbleOnClose) {
				Rumble ();
			}
			StartCoroutine (MoveDoor (closedPosition));
		}
	}

	/// <summary>
	/// Rumble to make the door more epic.
	/// </summary>
	void Rumble ()
	{
		CameraController camControl = Camera.main.GetComponent<CameraController> ();
		if (camControl != null) {
			camControl.Shake (100, moveTime + 0.25f, 0.07f);
		}
	}

	/// <summary>
	/// Moves the door to new position over time.
	/// </summary>
	/// <param name="newPosition">New position.</param>
	IEnumerator MoveDoor (Vector3 newPosition)
	{
		isMoving = true;
		Vector3 startingPosition = transform.position;
		float elapsed = 0;
		while (elapsed < moveTime) {
			elapsed += Time.deltaTime;
			transform.position = Vector3.Lerp (startingPosition, newPosition, elapsed / moveTime);
			yield return null;
		}
		isMoving = false;
	}
}
