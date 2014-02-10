using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : IController
{
	public int PlayerIndex { get; private set; }

	public InputDevice playerDevice { get; private set; }
	
	public Fighter fighter;
	public List<GameObject> enemies;

	bool isPlayerBound;
	bool rightStickAvailable = true;
	bool rightStickHorizontalAvailable = true;

	// Targetting variables
	GameObject pointer;
	GameObject targetReticle;
	float reticleY;
	Vector3 previousReticleScale;

	void Awake ()
	{
		AssignReferences ();
		isPlayerBound = false;
		ShowReticle (false);

		BindPlayer (0, InputDevices.GetAllInputDevices () [(int)InputDevices.ControllerTypes.Keyboard]);
	}

	/*
	 * Wire up all objects for class.
	 */
	void AssignReferences ()
	{
		enemies = new List<GameObject> ();
		pointer = GameObject.Find (ObjectNames.POINTER);
		fighter = gameObject.GetComponent<Fighter> ();
		targetReticle = GameObject.Find (ObjectNames.TARGET_RETICLE);
		reticleY = targetReticle.transform.position.y;
	}
	
	void Update ()
	{
		ShowReticle (fighter.target != null);
		RepositionReticle ();
	}

	public override void Think ()
	{
		if (!isPlayerBound) {
			return;
		}

		TryPause ();
		if (!TimeManager.Instance.IsPaused) {
			TryMove ();
			TryDodge ();
			TryTargetting ();
			TrySwitchWeapon ();
			TryAttack ();
			TryBlock ();
			TryDebugs ();
		}
	}

	void TryPause ()
	{
		if (Input.GetKeyDown (KeyCode.Return)) {
			if (TimeManager.Instance.IsPaused) {
				TimeManager.Instance.RequestUnpause ();
			} else {
				TimeManager.Instance.RequestPause ();
			}
		}
	}

	/*
	 * Apply movement in the Player's desired directions according to the various speed
	 * and movement variables.
	 */
	void TryMove ()
	{
		// Get input values
		float horizontal = 0.0f, vertical = 0.0f;
		horizontal = RBInput.GetAxisRawForPlayer (InputStrings.HORIZONTAL, PlayerIndex);
		vertical = RBInput.GetAxisRawForPlayer (InputStrings.VERTICAL, PlayerIndex);

		// Convert to camera world space
		Vector3 direction = new Vector3 (horizontal, 0.0f, vertical);
		direction = Camera.main.transform.TransformDirection (direction);
		direction = new Vector3 (direction.x, 0, direction.z);

		if (direction != Vector3.zero) {
			fighter.Run (direction);
		}
	}
	
	/*
	 * Handle toggling of targetting as well as switching between targets. While holding down
	 * target button, player can cycle through targets.
	 */
	void TryTargetting ()
	{
		float deadStickThreshold = 0.5f;
		InputDevice xbox = InputDevices.GetAllInputDevices () [(int)InputDevices.ControllerTypes.XBox];
		float rightStickPressedAxis = Input.GetAxisRaw (RBInput.ConcatPlayerIndex (InputStrings.TARGET, PlayerIndex, xbox));
		bool rightStickPressed = rightStickPressedAxis >= 0.99 && rightStickAvailable;
		// Consolidate bool for PC and XBox
		bool isTargetPressed = RBInput.GetButtonDownForPlayer (InputStrings.TARGET, PlayerIndex) ||
			rightStickPressed;

		// Toggle Targeting on and off
		if (isTargetPressed) {
			rightStickAvailable = false;
			if (!fighter.isLockedOn) {
				TargetNearest ();
			} else {
				fighter.LoseTarget ();
			}
		} 

		// Switch between targets
		if (fighter.isLockedOn) {
			// PC and Controller controls likely should diverge here due to right stick use
			InputDevice pc = InputDevices.GetAllInputDevices () [(int)InputDevices.ControllerTypes.Keyboard];
			float aimAxis = Input.GetAxisRaw (RBInput.ConcatPlayerIndex (InputStrings.TARGETHORIZONTAL, PlayerIndex, xbox));

			// Move target left
			if (Input.GetButtonDown (RBInput.ConcatPlayerIndex (InputStrings.TARGETLEFT, PlayerIndex, pc)) || 
				(aimAxis < -deadStickThreshold && rightStickHorizontalAvailable)) {
				rightStickHorizontalAvailable = false;
				TargetNext (true);
			}
			
			// Move target right
			if (Input.GetButtonDown (RBInput.ConcatPlayerIndex (InputStrings.TARGETRIGHT, PlayerIndex, pc)) 
				|| (aimAxis > deadStickThreshold && rightStickHorizontalAvailable)) {
				rightStickHorizontalAvailable = false;
				TargetNext (false);
			}
			// Enforce stick behavior like a button
			rightStickHorizontalAvailable = IsAxisDead (aimAxis, deadStickThreshold);
		}
		rightStickAvailable = IsAxisDead (rightStickPressedAxis, deadStickThreshold);
	}

	bool IsAxisDead (float axisValue, float deadStickThreshold)
	{
		return (axisValue < deadStickThreshold && axisValue > -deadStickThreshold);
	}

	void TryAttack ()
	{
		bool isLightAttack = RBInput.GetButtonDownForPlayer (InputStrings.FIRE, PlayerIndex);
		if (isLightAttack) {
			fighter.AttackWithWeapon (Fighter.AttackType.Weak);
		}
		bool isLightReleased = RBInput.GetButtonUpForPlayer (InputStrings.FIRE, PlayerIndex);
		if (isLightReleased) {
			fighter.ReleaseWeapon ();
		}
		bool isHeavyAttack = RBInput.GetButtonDownForPlayer (InputStrings.FIRE2, PlayerIndex);
		if (isHeavyAttack) {
			fighter.AttackWithWeapon (Fighter.AttackType.Strong);
		}
		bool isHeavyReleased = RBInput.GetButtonUpForPlayer (InputStrings.FIRE2, PlayerIndex);
		if (isHeavyReleased) {
			fighter.ReleaseWeapon ();
		}
	}

	/*
	 * Read input for Switch Weapon button and ask Fighter to switch between Sword and Spear.
	 */
	void TrySwitchWeapon ()
	{
		bool isSwitchWeapon = RBInput.GetButtonDownForPlayer (InputStrings.SWITCHWEAPON, PlayerIndex);
		if (isSwitchWeapon) {
			fighter.SwitchWeapon ();
		}
	}

	void TryDodge ()
	{
		// Get input values
		float horizontal = 0.0f, vertical = 0.0f;
		horizontal = RBInput.GetAxisRawForPlayer (InputStrings.HORIZONTAL, PlayerIndex);
		vertical = RBInput.GetAxisRawForPlayer (InputStrings.VERTICAL, PlayerIndex);

		// Convert to camera world space
		Vector3 direction = new Vector3 (horizontal, 0.0f, vertical);
		direction = Camera.main.transform.TransformDirection (direction);
		direction = new Vector3 (direction.x, 0, direction.z);

		// If player isn't standing still and hits dodge button, let's dodge!
		if (RBInput.GetButtonDownForPlayer (InputStrings.DODGE, PlayerIndex) &&
			direction != Vector3.zero) {
			fighter.Dodge (direction);
		}
	}
	
	/*
	 * Set fighter to blocking or unblocking depending on button up or down.
	 */
	void TryBlock ()
	{
		if (RBInput.GetAxisForPlayer (InputStrings.BLOCK, PlayerIndex) == 1 ||
			RBInput.GetButtonForPlayer (InputStrings.BLOCK, PlayerIndex)) {
			fighter.Block ();
		} else if (RBInput.GetAxisForPlayer (InputStrings.BLOCK, PlayerIndex) < 1 ||
			RBInput.GetButtonUpForPlayer (InputStrings.BLOCK, PlayerIndex)) {
			if (fighter.isBlocking) {
				fighter.UnBlock ();
			}
		}
	}

	/*
	 * Reads input and handles action for all debug functions
	 */
	void TryDebugs ()
	{
	}

	public void BindPlayer (int index, InputDevice device)
	{
		isPlayerBound = true;

		PlayerIndex = index;
		playerDevice = device;
		fighter.SetHuman (true);
	}

	/*
	 * Debug method that highlights the Arrow or not. Pass in True to highlight,
	 * False to not highlight it.
	 */
	void ShowReticle (bool showReticle)
	{
		Renderer[] renderers = pointer.GetComponentsInChildren<Renderer> ();
		foreach (Renderer reticlePieceRenderer in renderers) {
			if (showReticle) {
				reticlePieceRenderer.material.color = Color.red;
				targetReticle.SetActive (true);
			} else {
				reticlePieceRenderer.material.color = Color.blue;
				targetReticle.SetActive (false);
			}
		}
	}

	/// <summary>
	/// Repositions the reticle under the target enemy and scales it if necessary.
	/// </summary>
	void RepositionReticle ()
	{
		if (fighter.target != null) {
			// Move our reticle under the target
			Transform targetTransform = fighter.target.transform;
			targetReticle.transform.position = new Vector3 (targetTransform.position.x, reticleY,
			                                                targetTransform.position.z);

			// Scale if necessary
			float scaleMultiple = 1.5f;
			Vector3 newScale = new Vector3 (fighter.target.collider.bounds.size.x * scaleMultiple, 1, 
			                                fighter.target.collider.bounds.size.z * scaleMultiple);
			if (newScale != previousReticleScale) {
				previousReticleScale = newScale;
				targetReticle.transform.localScale = newScale;
			}
		}
	}

	/*
	 * Lock the players onto the closest known enemy. Does not check for sight.
	 */
	void TargetNearest ()
	{
		// Can't target nearest if there are no enemies
		if (enemies.Count == 0) {
			fighter.LoseTarget ();
			return;
		}
		GameObject newTarget = null;
		float closestDistance = Vector3.SqrMagnitude (enemies [0].transform.position - transform.position);
		foreach (GameObject enemy in enemies) {
			float enemyDistance = Vector3.SqrMagnitude (enemy.transform.position - transform.position);
			if (enemyDistance <= closestDistance) {
				closestDistance = enemyDistance;
				newTarget = enemy;
			}
		}

		// Select the next target
		if (newTarget != null) {
			fighter.LockOnTarget (newTarget);
			RepositionReticle ();
		}
	}

	/*
	 * Move the target left or right relative to the current target. Takes the vector between
	 * the target and each other enemy and stores the one with the smallest distance to be the
	 * new target. Takes a parameter @moveTargetLeft which specifies whether to move the target
	 * to the left of the current target or to the right.
	 */
	void TargetNext (bool moveTargetLeft)
	{
		float minDistance = float.MaxValue;
		GameObject newTarget = null;
		Vector3 directionToTarget = fighter.target.transform.position - transform.position;
		foreach (GameObject enemy in enemies) {
			Vector3 directionToEnemy = enemy.transform.position - transform.position;
			bool isLeft = (directionToTarget - directionToEnemy).x > 0;
			float distanceX = (directionToTarget - directionToEnemy).magnitude;
			if (isLeft == moveTargetLeft && distanceX <= minDistance && enemy != fighter.target) {
				newTarget = enemy;
				minDistance = distanceX;
			}
		}
		if (newTarget != null) {
			fighter.LockOnTarget (newTarget);
			RepositionReticle ();
		}
	}

	/*
	 * Add the provided enemy object to player's list of enemies to track.
	 */
	public void RememberEnemy (GameObject enemy)
	{
		enemies.Add (enemy);
	}

	/*
	 * Remove the provided enemy object from the player's list of enemies to track.
	 */
	public void ForgetEnemy (GameObject enemy)
	{
		if (enemies.Contains (enemy)) {
			bool sameAsTarget = enemy.Equals (fighter.target);
			enemies.Remove (enemy);
			if (sameAsTarget) {
				TargetNearest ();
			}
		} else {
			Debug.LogWarning ("Tried to remove enemy from list in which it doesn't exist.");
		}
	}
}