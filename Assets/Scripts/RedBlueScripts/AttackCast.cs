﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * Handles casting between this object and it's position in the previous frame. It then
 * sends a callback for each object hit.
 */
public class AttackCast : MonoBehaviour
{
	bool needsBegin = true;

	// GameObjects that show the two cast positions - start and end
	GameObject debugCurrentPositionObject;
	GameObject debugLastPositionObject;

	// Track last position of this game object
	Vector3 lastFramePosition;

	// A list of hit game objects so that we only report one OnHit per object
	List<GameObject> ignoreObjects = new List<GameObject> ();

	// The radius for this attack sphere
	public float radius;

	// The bits this attack should hit
	public LayerMask hitLayer;

	// Delegate template that specifies action to take on hit.
	public delegate void OnHitEvent (RaycastHit hit);
	public event OnHitEvent OnHit;

	// Flags for showing debug information
	public bool debugShowCasts;
	public bool debugShowHits;

	/*
	 * Initialize the AttackCast when enabled
	 */
	void OnEnable ()
	{
		// Immediately disable the script if it was not started through Begin.
		if (needsBegin) {
			enabled = false;
			return;
		}
		// TODO: Could support cast to source position, like I did in Ben10

		// Initialize previous position to the current attack position
		lastFramePosition = transform.position;

		// Clear previously hit objects so that we can re-hit them
		ignoreObjects.Clear ();
		ignoreObjects.Add (transform.root.gameObject);
	}

	/*
	 * Cleanup the AttackCast when disabled
	 */
	void OnDisable ()
	{
		DestroyDebugObjects ();
	}

	/*
	 * Begin the attack sweep by setting cast object to active.
	 */
	public void Begin ()
	{
		needsBegin = false;
		enabled = true;
		gameObject.SetActive (true);
	}

	/*
	 * End the attack sweep.
	 */
	public void End ()
	{
		OnHit = null;
		gameObject.SetActive (false);
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		Vector3 direction = (transform.position - lastFramePosition).normalized;
		float distance = Vector3.Distance (lastFramePosition, transform.position);
		RaycastHit[] hits;
		hits = Physics.SphereCastAll (lastFramePosition, radius, direction, distance, hitLayer);
		ReportHits (hits);
		RenderDebugCasts ();

		// Remember this position as the last
		lastFramePosition = transform.position;
	}

	void RenderDebugCasts ()
	{
		//TODO: Could wrap a precompile flag to never show debugs in release
		if (!debugShowCasts) {
			return;
		}

		// Spawn debug objects if they haven't been spawned
		if (debugCurrentPositionObject == null) {
			SpawnDebugObjects ();
		}

		// Update debug line. Wish this was a cylinder.
		Debug.DrawLine (transform.position, lastFramePosition);

		// Update gizmo, if we have one
		Gizmo gizmo = (Gizmo)GetComponent<Gizmo> ();
		if (gizmo != null) {
			gizmo.gizmoSize = radius;
			gizmo.enabled = debugShowCasts;
		}

		// Make sure spheres represent current cast radius as a diameter
		debugLastPositionObject.transform.localScale = (Vector3.one * (radius * 2));
		debugCurrentPositionObject.transform.localScale = (Vector3.one * (radius * 2));

		// Hide spheres if debugs are off
		debugLastPositionObject.SetActive (debugShowCasts);
		debugCurrentPositionObject.SetActive (debugShowCasts);

		// Set the new positions of the spheres
		debugLastPositionObject.transform.position = lastFramePosition;
		debugCurrentPositionObject.transform.position = transform.position;
	}

	/*
	 * Spawn debug objects used to see where the casts are located
	 */
	void SpawnDebugObjects ()
	{
		Material debugMaterial = new Material (Shader.Find ("Transparent/Diffuse"));
		debugCurrentPositionObject = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		debugCurrentPositionObject.transform.position = transform.position;
		debugCurrentPositionObject.renderer.material = debugMaterial;
		debugCurrentPositionObject.renderer.material.color = new Color (0, 1.0f, 0, .3f);
		debugCurrentPositionObject.collider.enabled = false;
		debugCurrentPositionObject.transform.parent = transform;
		debugCurrentPositionObject.SetActive (debugShowCasts);

		debugLastPositionObject = GameObject.CreatePrimitive (PrimitiveType.Sphere);
		debugLastPositionObject.transform.position = lastFramePosition;
		debugLastPositionObject.renderer.material = debugMaterial;
		debugLastPositionObject.renderer.material.color = new Color (1.0f, 1.0f, 0, .3f);
		debugLastPositionObject.collider.enabled = false;
		debugLastPositionObject.transform.parent = transform;
		debugLastPositionObject.SetActive (debugShowCasts);
	}

	/*
	 * Destroy the objects spawned to show debug casts
	 */
	void DestroyDebugObjects ()
	{
		if (debugCurrentPositionObject != null) {
			Destroy (debugCurrentPositionObject);
			Destroy (debugLastPositionObject);
		}
	}

	/*
	 * Send Hits to hit objects, from a Raycast hit array.
	 */
	void ReportHits (RaycastHit[] hits)
	{
		foreach (RaycastHit hit in hits) {
			ReportHit (hit);
		}
	}

	/*
	 * Apply the OnHit function to a specified Raycast hit. This should be made project-agnostic
	 * at some point, so that AttackCasts can be a tool.
	 */
	void ReportHit (RaycastHit hit)
	{
		// Throw out iIgnored objects
		if (ignoreObjects.Contains (hit.collider.transform.root.gameObject)) {
			return;
		}

		if (debugShowHits) {
			Debug.DrawRay (hit.point, hit.normal, Color.red, 0.5f);
		}

		// Trigger OnHit event if anyone is subscribed
		if (OnHit != null) {
			OnHit (hit);
		}

		GameObject hitGameObject = hit.collider.gameObject;
		ignoreObjects.Add (hitGameObject);
	}
}
