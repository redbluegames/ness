using UnityEngine;
using GoogleFu;
using System;
using System.Collections;

public class Fighter : MonoBehaviour
{
	IController controller;
	public float runSpeed;
	public GameObject target;
	Health health;
	bool isHuman = false;

	public Team team;
	
	// Timers
	CountUpTimer chargeUpTimer;
	public float currentChargeUpTime = 0;
	float superBlockWindow = 1.0f;
	CountUpTimer shieldTime = new CountUpTimer ();

	// Weapons and Attacks
	public GameObject weaponHand;
	public GameObject[] carriedWeapons;
	public int equippedWeaponIndex;
	public AttackData[] attacks;
	public AttackData currentAttack;
	public bool feetPlanted;

	// State Variables
	public bool isAttacking { get; private set; }
	public bool isLockedOn { get; private set; }
	public bool isBlocking { get; private set; }

	// Animations, Sounds, and FX
	public TweenFlashColor shieldFlash;
	public AnimationClip idle;
	public AnimationClip blockIdle;
	public TrailRenderer swingTrail;
	public AudioClip blockSound;
	public AudioClip shieldUpSound;
	public AudioClip takeHitSound;
	public AudioSource attackAndBlockChannel;
	public GameObject chargeFX; 

	// Dodge
	float dodgeSpeed;
	float startDodgeSpeed = 50.0f;
	float endDodgeSpeed = 0.0f;
	float dodgeTime = 0.6f;
	Vector3 currentDodgeDirection;
	
	// Knockback
	float currentMoveReactionDuration = 0.6f;
	Vector3 currentMoveReactionDirection;
	// How much of the knockback is the target moving?
	const float KNOCKBACK_MOVE_PORTION = 0.35f; 

	// Store variables for attack tracking
	float forcedAttackMoveSpeed;

	// Character control members
	Vector3 moveDirection;
	float gravity = -20.0f;
	float verticalSpeed = 0.0f;
	float attackDamping = 5.0f;
	const float defaultDamping = 25.0f;
	float damping = 25.0f;
	CollisionFlags collisionFlags;

	// Timers
	float lastHitTime = Mathf.NegativeInfinity;
	float lastDodgeTime = Mathf.NegativeInfinity;
	float lastKnockbackTime = Mathf.NegativeInfinity;
	
	// Color management members
	Color hitColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
	public Color nativeColor;

	// Cached objects
	Transform myTransform;
	CameraController playerCamera;
	
	// Character state
	public enum CharacterState
	{
		Idle = 0,
		Moving,
		Dodging,
		Blocked,
		Blocking,
		Knockedback,
		KnockedbackByBlock
	}
	public CharacterState characterState;

	public enum AttackType
	{
		Weak = 0,
		Strong = 1
	}

	void Awake ()
	{
		myTransform = transform;
		playerCamera = GameObject.FindWithTag (Tags.MAIN_CAMERA).GetComponent<CameraController> ();
		carriedWeapons = new GameObject[2];
		// TODO make this check for controller == null, otherwise
		// it always overrides the one chosen in the editor.
		controller = GetComponent<IController> ();
		health = GetComponent<Health> ();
		damping = defaultDamping;
		dodgeSpeed = startDodgeSpeed;
		if (swingTrail == null) {
			swingTrail = GetComponentInChildren<TrailRenderer> ();
			if (swingTrail != null) {
				swingTrail.renderer.enabled = false;
			}
		}
	}

	void Start ()
	{
		// Initialize attacks
		chargeUpTimer = new CountUpTimer ();
		if (isHuman) {
			SetupHumanFighter ();
		} else {
			SetupEnemyFighter ();
		}
	}

	void Update ()
	{
		Debug.DrawLine (Vector3.zero, myTransform.forward);
		ApplyGravity ();
		PerformActionInProgress ();
		UpdateLockOn ();

		controller.Think ();
		TryDebugs ();

		// Animation sector
		if (!isAttacking && (IsIdle () || IsMoving ()) && !chargeUpTimer.IsRunning ()) {
			if (isBlocking) {
				animation.CrossFade (blockIdle.name, 0.1f);
			} else {
				animation.Play (idle.name, PlayMode.StopAll);
			}
		} else if (IsDodging ()) {
			animation.Play (idle.name, PlayMode.StopAll);
		} else if (IsInMoveReaction ()) {
			// Interrupt or stop attack animation
			animation.Play (idle.name, PlayMode.StopAll);
		}

		// Color and effects
		RenderColor ();
		if (chargeUpTimer.IsRunning ()) {
			float chargeUpRatio = chargeUpTimer.GetTimerRuntime () / currentAttack.chargeAnimation.length;
			DisplayChargeEffects (chargeUpRatio);
		} else {
			HideChargeEffects ();
		}
	}
	
	/*
	 * Any pending actions that need to finish up go here. For example, swinging
	 * a sword starts but later ends in this method, restoring the character state to ready
	 * to swing.
	 */
	void PerformActionInProgress ()
	{
		if (isAttacking) {
			UpdateAttackState ();
		} else if (IsDodging ()) {
			UpdateDodgeState ();
		} else if (IsInMoveReaction ()) {
			UpdateMoveReactionState ();
		}
	}
	
	/*
	 * Perform object wiring specific for Human Fighters. This includes starting weapons
	 * and attacks.
	 */
	void SetupHumanFighter ()
	{
		equippedWeaponIndex = 0;
		attacks = new AttackData[Enum.GetNames (typeof(AttackType)).Length];
		carriedWeapons [equippedWeaponIndex] = WeaponManager.Instance.SpawnWeapon (Weapons.rowIds.SPEAR, weaponHand);
		carriedWeapons [equippedWeaponIndex + 1] = WeaponManager.Instance.SpawnWeapon (Weapons.rowIds.BOW, weaponHand);
		EquipWeapon (equippedWeaponIndex);
	}

	/*
	 * Perform object wiring specific to Enemy Fighters. This includes starting weapons and attacks.
	 */
	void SetupEnemyFighter ()
	{
		AttackManager attackManager = (AttackManager)GameObject.Find (ObjectNames.MANAGERS).GetComponent <AttackManager> ();
		attacks = new AttackData[Enum.GetNames (typeof(AttackType)).Length];
		attacks [(int)AttackType.Weak] = attackManager.GetAttack (GoogleFu.Attacks.rowIds.ENEMY_WEAK);
		attacks [(int)AttackType.Strong] = attackManager.GetAttack (GoogleFu.Attacks.rowIds.ENEMY_STRONG);
	}

	#region Exposed Fighter Actions
	
	/*
	 * Walk the fighter in a given direction.
	 */
	public void Run (Vector3 direction)
	{
		// Workaround for bad character / attack states
		if (!feetPlanted && (IsIdle () || IsMoving ())) {
			characterState = CharacterState.Moving;
			float movescale = 1.0f;
			bool isCharging = chargeUpTimer.IsRunning ();
			if (isBlocking || isCharging) {
				movescale = 0.5f;
			}
			Move (direction, runSpeed * movescale);
		}
	}
	
	/*
	 * Try to make the character swing its weapon. If it's in the process
	 * of swinging or swing is on cooldown, it won't do anything. Determine whether
	 * character should enter charge state or fire its weapon.
	 */
	public void AttackWithWeapon (AttackType attackType)
	{
		if (isAttacking || (!IsMoving () && !IsIdle ())) {
			// Ignore any attacks while player isn't moving or idle.
			// Ignore any inputs while the player is already attacking or isn't
			return;
		}
		if (isBlocking) {
			// TODO Get Edward to help make the animation unblock while attacking animation
			// is playing. Is this worth it without using mechanim?
			UnBlock ();
		}
		currentAttack = attacks [(int)attackType];
		if (currentAttack.controlType == AttackData.ControlType.Hold) {
			StartChargingWeapon ();
		} else {
			FireWeapon ();
		}
	}

	
	/*
	 * Switch through our carried weapons and equip the next one in the array. This includes
	 * setting any class variables that cache info about the currently equipped weapon.
	 */
	public void SwitchWeapon ()
	{
		// Swith to the next weapon in the list
		if (!isAttacking && !chargeUpTimer.IsRunning ()) {
			int nextWeaponBeingCarried = (equippedWeaponIndex + 1) % carriedWeapons.Length;
			EquipWeapon (nextWeaponBeingCarried);
		}
	}
	
	/*
	 * After charging a weapon, when the fighter lets go of the charge, it should
	 * perform ReleaseWeapon. This method stamps off the charged up time and begins
	 * the Swing animation.
	 */
	public void ReleaseWeapon ()
	{
		bool isCharging = chargeUpTimer.IsRunning ();
		if (isCharging) {
			currentChargeUpTime = chargeUpTimer.GetTimerRuntime ();
			chargeUpTimer.StopTimer ();
			FireWeapon ();
		}
	}
	
	/*
	 * Cause fighter to dodge in a given direction.
	 */
	public void Dodge (Vector3 direction)
	{
		if (IsMoving () || IsIdle ()) {
			if (isBlocking) {
				UnBlock ();
			}
			currentDodgeDirection = direction;
			dodgeSpeed = startDodgeSpeed;
			characterState = CharacterState.Dodging;
			CancelAttack ();
			lastDodgeTime = Time.time;
		}
	}

	/*
	 * Set the fighter in Blocking state. Play animations and sounds.
	 */
	public void Block ()
	{
		if (isBlocking || isAttacking || chargeUpTimer.IsRunning ()) {
			return;
		}
		
		if (IsIdle () || IsMoving ()) {
			PlaySound (shieldUpSound);
			shieldTime.StartTimer ();
			isBlocking = true;
		}
	}

	/*
	 * Set the fighter back to non-blocking state.
	 */
	public void UnBlock ()
	{
		PlaySound (shieldUpSound);
		shieldTime.StopTimer ();
		isBlocking = false;
	}

	/*
	 * Set the Fighter target to the provided Transform and start staring it down.
	 */
	public void LockOnTarget (GameObject newTarget)
	{
		target = newTarget;
		isLockedOn = true;
		if (newTarget == null) {
			Debug.LogWarning ("Attempted to lock on to null target.");
		}
		// Look at XZ coordinate of target only
		Vector3 lookPosition = target.transform.position;
		lookPosition.y = myTransform.position.y;
		if (!feetPlanted) {
			myTransform.LookAt (lookPosition);
		}
	}

	/*
	 * Set the target transform to null, effectively losing it.
	 */
	public void LoseTarget ()
	{
		target = null;
		isLockedOn = false;
	}

	#endregion

	/*
	 * Plays the specified sound clip as a one shot sound
	 */
	void PlaySound (AudioClip clip)
	{
		attackAndBlockChannel.PlayOneShot (clip);
	}

	/*
	 * Ensure locked on characters always face their targets, even when the other
	 * entity moves (lock on is enforced during move as well).
	 */
	void UpdateLockOn ()
	{
		if (target != null) {
			LockOnTarget (target);
		}
	}
	
	/*
	 * Sets vertical speed to the expected value based on whether or not the character is grounded.
	 */
	void ApplyGravity ()
	{
		if (IsGrounded ()) {
			verticalSpeed = 0.0f;
		} else {
			verticalSpeed += gravity * Time.deltaTime;
		}
	}

	/*
	 * Apply movement in the Player's desired directions according to the various speed
	 * and movement variables.
	 */
	void Move (Vector3 direction, float speed)
	{
		// Get movement vector
		Vector3 movement = (direction.normalized * speed) + new Vector3 (0.0f, verticalSpeed, 0.0f);
		movement *= Time.deltaTime;

		// Rotate to face the direction of XZ movement immediately, if lockFacing isn't set
		Vector3 movementXZ = new Vector3 (movement.x, 0.0f, movement.z);
		if (target != null) {
			LockOnTarget (target);
		} else if (movementXZ != Vector3.zero) {
			myTransform.rotation = Quaternion.Slerp (myTransform.rotation,
					Quaternion.LookRotation (movementXZ), Time.deltaTime * damping);
		}

		// Apply movement vector
		CharacterController biped = GetComponent<CharacterController> ();
		collisionFlags = biped.Move (movement);
	}

	/*
	 * Assigns movement to the character, which is applied in the attack state update
	 */
	void SetAttackMovement (float speed)
	{
		PlantFeet ();
		forcedAttackMoveSpeed = speed;
	}

	/*
	 * Assigns movement to the character at a ratio related to the amount of time the player
	 * has charged up.
	 */
	void SetAttackMovementCharged (float speed)
	{
		if (currentAttack == null) {
			Debug.LogError ("CurrentAttack was null...how?");
		}
		// TODO we can have a more robust algorithm for determining charge forward speed.
		// Probably should be exponential.
		float multiplier = 0.5f + Mathf.Min (currentChargeUpTime / currentAttack.chargeAnimation.length, 1.0f);
		SetAttackMovement (speed * multiplier);
	}

	/*
	 * Prevent movement during certain parts of an attack.
	 */
	void PlantFeet ()
	{
		feetPlanted = true;
	}

	/*
	 * Mark feet unplanted, restoring movement to a character.
	 */
	void UnplantFeet ()
	{
		feetPlanted = false;
	}

	/*
	 * Check that enough time has passed after character swung to call the
	 * swing "complete". Once it is, restore the character state to normal.
	 */
	void UpdateAttackState ()
	{
		if (forcedAttackMoveSpeed > 0) {
			Move (transform.TransformDirection (Vector3.forward), forcedAttackMoveSpeed);
		}
		float dampingToGo = attackDamping - damping;
		damping = damping + dampingToGo * 0.1f;
		//damping = Mathf.Lerp (damping, attackDamping, dampingPercentage);
	}

	/*
	 * Turn on the attack trail renderer as well as the attack caster.
	 */
	void StartAttackCast ()
	{
		Weapon activeWeapon = carriedWeapons [equippedWeaponIndex].GetComponent<Weapon> ();
		//TODO Create a Damage tab in the google fu spreadsheet that assists with calculating desired damage.
		Damage damageOut = new Damage (currentAttack.maxDamage, myTransform, new RaycastHit ());
		activeWeapon.BeginAttack (damageOut, this);
	}

	/*
	 * Turn off the attack trail renderer as well as the attack caster.
	 */
	void EndAttackCast ()
	{
		Weapon activeWeapon = carriedWeapons [equippedWeaponIndex].GetComponent<Weapon> ();
		activeWeapon.EndAttack ();
	}

	/*
	 * Cancels and cleans up after an active attack.
	 */
	void CancelAttack ()
	{
		// Stop the existing attack sound
		attackAndBlockChannel.Stop ();
		// Clear any unfinished forced attack move speed
		forcedAttackMoveSpeed = 0;
		UnplantFeet ();
		damping = defaultDamping;
		// Cancel Attack can be called even if the player isn't attacking
		if (currentAttack != null) {
			// Only deactivate non-projectile attacks
			if (currentAttack.IsRanged ()) {
				EndAttackCast ();
			}
		}
		// Kill any attack trail
		SetAttackTrailActive (false);
		// Cancel charge ups
		chargeUpTimer.StopTimer ();
		currentChargeUpTime = 0;
		isAttacking = false;
	}

	/*
	 * Sets the fighter as a human or AI
	 */
	public void SetHuman (bool human)
	{
		isHuman = human;
	}

	/*
	 * Make the specified weapon index the active weapon, disabling others and wiring
	 * up attack variables to the new weapon.
	 */
	void EquipWeapon (int weaponIndex)
	{
		// Disable all other weapons
		for (int i = 0; i < carriedWeapons.Length; i++) {
			if (i != weaponIndex) {
				carriedWeapons [i].SetActive (false);
			}
		}
		// Set the specified weapon to active and load in the associated attacks
		equippedWeaponIndex = weaponIndex;
		carriedWeapons [equippedWeaponIndex].SetActive (true);

		currentChargeUpTime = 0;
		attacks [(int)AttackType.Weak] = carriedWeapons [equippedWeaponIndex].GetComponent<Weapon> ().weaponData.lightAttack;
		attacks [(int)AttackType.Strong] = carriedWeapons [equippedWeaponIndex].GetComponent<Weapon> ().weaponData.heavyAttack;
	}

	/*
	 * Fires the current attack for the currently equipped weapon. Determines whether
	 * to attack with a projectile or melee attack based on the attack data.
	 */
	void FireWeapon ()
	{
		isAttacking = true;
		// Fire weapon, attack isn't chargable
		bool isMeleeAttack = !currentAttack.IsRanged ();
		if (isMeleeAttack) {
			FireMeleeWeapon ();
		} else {
			FireProjectileWeapon ();
		}
	}

	/*
	 * Swing the weapon associated with the passed in attack. Performs all logic
	 * surrounding a swing and starts off it's animation.
	 */
	void FireMeleeWeapon ()
	{
		SetAttackTrailActive (true);
		animation.Play (currentAttack.swingAnimation.name, PlayMode.StopAll);
	}

	/*
	 * Spawn a projectile based on the currently equipped weapon and fire it away from
	 * the player.
	 */
	void FireProjectileWeapon ()
	{
		// Play firing animation
		animation.Play (currentAttack.swingAnimation.name, PlayMode.StopAll);
		
		// Spawn and fire projectile
		Weapon firingWeapon = carriedWeapons [equippedWeaponIndex].GetComponent<Weapon> ();
		GameObject projectilePrefab = null;
		Damage damageOut = new Damage (currentAttack.maxDamage, currentAttack, myTransform);
		if (currentAttack.strength == AttackData.Strength.Weak) {
			projectilePrefab = firingWeapon.lightProjectile;
		} else if (currentAttack.strength == AttackData.Strength.Strong) {
			projectilePrefab = firingWeapon.heavyProjectile;
		}
		GameObject newProjectile = (GameObject)Instantiate (
			projectilePrefab, transform.position, transform.rotation);
		newProjectile.GetComponent<Projectile> ().Fire (2000.0f, 5.0f, transform.forward, damageOut, Team.GoodGuys);
	}

	void SetAttackTrailActive (bool isActive)
	{
		
		if (swingTrail != null) {
			swingTrail.renderer.enabled = isActive;
		}
	}

	/*
	 * Begins the fighter charging up an attack. Performs logic around charging,
	 * sets fighter state, and kicks off the charge animation.
	 */
	void StartChargingWeapon ()
	{
		chargeUpTimer.StartTimer ();
		currentChargeUpTime = 0;
		animation.CrossFade (currentAttack.chargeAnimation.name, 0.1f);
	}

	/*
	 * Resolve the dodge roll once it's complete.
	 */
	void UpdateDodgeState ()
	{
		if (Time.time - lastDodgeTime >= dodgeTime) {
			characterState = CharacterState.Idle;
		} else {
			dodgeSpeed += (endDodgeSpeed - dodgeSpeed) * 0.15f;
			Move (currentDodgeDirection, dodgeSpeed);
		}
	}

	/*
	 * Knockback consists of a pushback stage and stun stage. This method uses
	 * timers for both to determine which it is in. When both timers are up,
	 * return the fighter to Idle, allowing movement again.
	 */
	void UpdateMoveReactionState ()
	{
		float timeInKnockbackState = Time.time - lastKnockbackTime;
		if (timeInKnockbackState >= currentMoveReactionDuration) {
			characterState = CharacterState.Idle;
		} else if (timeInKnockbackState < (KNOCKBACK_MOVE_PORTION * currentMoveReactionDuration)) {
			Move (currentMoveReactionDirection, startDodgeSpeed);
		}
	}

	/*
	 * Interrupt fighter, push them back, and stun them for a second. This
	 * method only puts the fighter in the knockedback state and clears any
	 * other conflicting states it might have set.
	 */
	void ReceiveKnockback (Vector3 direction, float duration)
	{
		if (isAttacking) {
			CancelAttack ();
		}
		characterState = CharacterState.Knockedback;
		lastKnockbackTime = Time.time;
		currentMoveReactionDirection = direction;
		currentMoveReactionDuration = duration;
	}

	/*
	 * Set the fighter state to begin a knockback after getting blocked.
	 */
	void ReceiveKnockbackByBlock (Vector3 direction, float duration)
	{
		if (isAttacking) {
			CancelAttack ();
		}
		characterState = CharacterState.KnockedbackByBlock;
		lastKnockbackTime = Time.time;
		currentMoveReactionDirection = direction;
		currentMoveReactionDuration = duration;
	}

	/*
	 * Return a reference to the fighter's attacks
	 */
	public AttackData[] GetAttacks ()
	{
		return attacks;
	}

	/*
	 * Reads input and handles action for all debug functions
	 */
	void TryDebugs ()
	{
	}

	public void SnapToPoint (Transform point)
	{
		myTransform.position = point.transform.position;
	}
	
	/*
	 * Resolve a hit and perform the appropriate reaction. This may mean
	 * taking damage or it may mean resolving a block.
	 */
	public void ApplyDamage (Damage incomingDamage)
	{
		//TODO derive this from the attack, or damage info
		playerCamera.Shake (3.0f, 0.2f, 0.1f);
		// Handle blocked hits first
		Vector3 toHit = incomingDamage.HitLocation.point - myTransform.position;
		bool hitFromFront = Vector3.Dot (myTransform.forward, toHit.normalized) < 0;
		if (isBlocking && !hitFromFront) {
			// Uncomment these to help debug this, but it should be correct.
			//Debug.DrawLine (Vector3.zero, myTransform.forward);
			//Debug.DrawLine (Vector3.zero, toHit.normalized, Color.red);
			//Debug.DrawLine (myTransform.forward, toHit.normalized, Color.green);
			shieldFlash.Flash ();
			PlaySound (blockSound);
			// Cause attacker to get knocked back
			// TODO We need a check for if the damage is ranged
			if (shieldTime.GetTimerRuntime () < superBlockWindow) {
				// Only knockback enemies if non-projectile attacks
				if (!incomingDamage.Attack.IsRanged ()) {
					Enemy attackingEnemy = incomingDamage.Attacker.GetComponent<Enemy> ();
					attackingEnemy.ReceiveKnockback (toHit.normalized * 15.0f, 0.5f);
				}
			}
		} else {
			// Play a new hit sound at the location. Must make minDistance the same as the
			// attack channel so that it plays at the same volume. This is kind of weird...
			AudioSource source = SoundManager.PlayClipAtPoint (takeHitSound, incomingDamage.HitLocation.point);
			source.minDistance = attackAndBlockChannel.minDistance;

			lastHitTime = Time.time;
			health.TakeDamage (incomingDamage);
			// Handle reaction type of successful hits
			if (incomingDamage.Attack.reactionType == AttackData.ReactionType.Knockback) {
//				float knockBackDuration = 0.2f;
				// Knock back in the opposite direction of the attacker.
				//ReceiveKnockback ((myTransform.position - incomingDamage.Attacker.position).normalized, knockBackDuration);
			}
		}
	}

	/*
	 * This is called by the weapon to let the fighter know an attack hit its target.
	 */
	public void NotifyAttackHit ()
	{
		//GameManager.Instance.FreezeGame (0.067f);
		playerCamera.Shake (3.0f, 0.3f, currentAttack.cameraShakeIntensity);
	}

	void Die ()
	{
		Debug.Log ("Player died. Play animation here.");
		gameObject.SetActive (false);
	}

	#region Effects and Rendering
	/*
	 * Render the color of the fighter, according to the state it is in.
	 */
	void RenderColor ()
	{
		Color colorToShow;
		bool isCharging = chargeUpTimer.IsRunning ();
		const float timeToShowHit = 0.1f;
		
		if (isCharging) {
			float chargeRatio = chargeUpTimer.GetTimerRuntime () / currentAttack.chargeAnimation.length;
			if (chargeRatio >= 1) {
				colorToShow = Color.Lerp (nativeColor, Color.magenta, chargeRatio);
			} else {
				colorToShow = nativeColor;
			}
		} else if (Time.time >= timeToShowHit + lastHitTime) {
			switch (characterState) {
			case CharacterState.Idle:
				colorToShow = nativeColor;
				break;
			case CharacterState.Dodging:
				colorToShow = Color.green;
				break;
			case CharacterState.Knockedback:
				colorToShow = Color.blue;
				break;
			case CharacterState.KnockedbackByBlock:
				colorToShow = Color.grey;
				break;
			case CharacterState.Moving:
				colorToShow = nativeColor;
				break;
			default:
				colorToShow = nativeColor;
				break;
			}
		} else {
			colorToShow = hitColor;
		}
		
		renderer.material.color = colorToShow;
	}

	/*
	 * Show the FX that indicate a player is charging their weapon. Takes a chargeratio
	 * which the percent charged up the player is. At full charge, hides effect.
	 */
	void DisplayChargeEffects (float chargeRatio)
	{
		Vector3 startingSize = new Vector3 (2.75f, 2.75f, 1f);
		if (!chargeFX.activeSelf) {
			chargeFX.SetActive (true);
			// Only zoom in for heavy attacks
			if (currentAttack.strength == AttackData.Strength.Strong) {
				playerCamera.Zoom (1.1f, currentAttack.chargeAnimation.length);
			}
		}
		chargeFX.transform.localScale = Vector3.Lerp (startingSize, Vector3.one, chargeRatio);
	}

	/*
	 * Hide the charge effect.
	 */
	void HideChargeEffects ()
	{
		if (chargeFX.activeSelf) {
			chargeFX.SetActive (false);
			playerCamera.ResetZoom ();
		}
	}

	#endregion

	#region Character State Checks
	/*
	 * Checks to see if the character is grounded by checking collision flags.
	 */
	bool IsGrounded ()
	{
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}

	/*
	 * Return true if the character is in any of the idle states.
	 */
	bool IsIdle ()
	{
		return characterState == CharacterState.Idle;
	}
	
	/*
	 * Return true if the character is in dodging state.
	 */
	bool IsDodging ()
	{
		return characterState == CharacterState.Dodging;
	}
	
	/*
	 * Return true if the character is moving.
	 */
	bool IsMoving ()
	{
		return characterState == CharacterState.Moving;
	}
	
	/*
	 * Return true if the character is in the middle of a Move Reaction (ex. knockback).
	 */
	bool IsInMoveReaction ()
	{
		return characterState == CharacterState.Knockedback || characterState == CharacterState.KnockedbackByBlock;
	}
	
	/*
	 * Yet another state check bool...
	 */
	bool IsKnockedBackByBlock ()
	{
		return characterState == CharacterState.KnockedbackByBlock;
	}
	#endregion
}