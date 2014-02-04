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
	public float throttle;
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
			animator.SetTrigger ("Attack");
			wantsToAttack = false;
		}

		// Update animation parameters
		UpdateAnimator ();

		// Update face and move directions
		motor.MoveDirection = moveDirection;
		motor.FaceDirection = faceDirection;
	}

	void LateUpdate ()
	{
		// Poll the current state instead of getting an animation complete event.
		isAttacking = animator.GetCurrentAnimatorStateInfo(0).IsName("Attack");
	}

	/// <summary>
	/// Handles keeping the animator up to date with the script state
	/// </summary>
	void UpdateAnimator ()
	{
		if(animator == null) {
			return;
		}
		float speed = throttle > 0.0f ? 1.0f : 0.0f;
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

		// Remove the current collider so that the ragdoll doesn't unembed.
		collider.enabled = false;
		Vector3 forceDirection = transform.position - lethalDamage.Attacker.transform.position;
		GameObject ragdoll = (GameObject)Instantiate (ragdollPrefab, transform.position, transform.rotation);
		Vector3 force = forceDirection * 100;
		ragdoll.rigidbody.AddForce (force);

		Destroy (gameObject);
		Destroy (ragdoll, 3.0f);
	}
}