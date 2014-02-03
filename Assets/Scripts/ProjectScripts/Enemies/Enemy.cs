using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(CharacterMotor))]
public class Enemy : MonoBehaviour
{
	// The Team the fighter is on
	public Team team;
	public AudioClip takeHitSound;
	public GameObject ragdollPrefab;
	public bool isAttacking {get; private set;}

	// "WantsTo" variables
	public Vector3 moveDirection;
	public Vector3 faceDirection;
	public bool wantsToAttack;

	CharacterMotor motor;
	Animator animator;
	Health health;
	AttackCast attackCaster;

	void Start ()
	{
		AssignReferences ();
		// This was moved to Start in order to make sure the player is ready to remember enemies.
		AddToPlayerEnemyList ();
	}

	/*
	 * Add Enemy to the list of enemies that the player uses for targetting.
	 */
	void AddToPlayerEnemyList ()
	{
		if (team == Team.BadGuys) {
			PlayerController player = GameObject.Find (ObjectNames.PLAYER).GetComponent<PlayerController> ();
			player.RememberEnemy (gameObject);
		}
	}

	/*
	 * Remove Enemy from the list of enemies that player uses for targetting.
	 */
	void RemoveFromPlayerEnemyList ()
	{
		if (team == Team.BadGuys) {
			// Take enemy off the player's list of enemies
			PlayerController player = GameObject.Find (ObjectNames.PLAYER).GetComponent<PlayerController> ();
			player.ForgetEnemy (gameObject);
		}
	}

	/* Assigns references to private fields that the character will use
	 */
	void AssignReferences ()
	{
		health = gameObject.GetComponent<Health> ();
		motor = gameObject.GetComponent<CharacterMotor> ();
		animator = gameObject.GetComponent<Animator> ();
		attackCaster = gameObject.GetComponentInChildren<AttackCast> ();
	}

	void Update ()
	{
		if(wantsToAttack)
		{
			isAttacking = true;
			wantsToAttack = false;
		}

		// Update animation parameters
		UpdateAnimator ();

		// Update face and move directions
		if(moveDirection != Vector3.zero) {
			// A zero moveDirection means they don't want to move. Use last heading.
			motor.MoveDirection = moveDirection;
		}
		motor.FaceDirection = faceDirection;
	}

	/// <summary>
	/// Handles keeping the animator up to date with the script state
	/// </summary>
	void UpdateAnimator ()
	{
		if(animator == null) {
			return;
		}
		float speed = 0.0f;
		if (moveDirection != Vector3.zero) {
			speed = 1.0f;
		} else {
			speed = 0.0f;
		}

		animator.SetBool("IsAttacking", isAttacking);
		animator.SetFloat ("Speed", speed);
	}

	/// <summary>
	/// Begin the sweep for damageable objects on the current attack
	/// </summary>
	public void StartAttackCast ()
	{
		attackCaster.OnHit += OnAttackHit;
		attackCaster.Begin ();
	}
	
	/// <summary>
	/// End the sweep for damageable objects on the current attack
	/// </summary>
	void EndAttackCast ()
	{
		attackCaster.End ();
	}

	/// <summary>
	/// Raises the animation complete event.
	/// </summary>
	public void OnAnimationComplete ()
	{
		isAttacking =false;
	}
	
	/*
	 * Deal damage to the hit object based on the current attack.
	 */
	void OnAttackHit (RaycastHit hit)
	{
		Damage damageOut = new Damage (10.0f, transform);
		GameObject hitGameObject = hit.transform.gameObject;
		hitGameObject.SendMessage ("ApplyDamage", damageOut, SendMessageOptions.DontRequireReceiver);
	}

	/*
	 * Resolve a hit and perform the appropriate reaction. This may mean
	 * taking damage or it may mean resolving a block.
	 */
	public void ApplyDamage (Damage damageFromHit)
	{
		SoundManager.PlayClipAtPoint (takeHitSound, transform.position);
		health.TakeDamage (damageFromHit);
	}

	void Die (Damage lethalDamage)
	{
		RemoveFromPlayerEnemyList ();
		Vector3 forceDirection = lethalDamage.HitLocation.point - lethalDamage.Attacker.transform.position;
		GameObject ragdoll = (GameObject)Instantiate (ragdollPrefab, transform.position, transform.rotation);
		Vector3 force = forceDirection * 50;
		ragdoll.rigidbody.isKinematic = false;
		ragdoll.rigidbody.AddForce (force);
		Destroy (gameObject);
		Destroy (ragdoll, 3.0f);
	}
}