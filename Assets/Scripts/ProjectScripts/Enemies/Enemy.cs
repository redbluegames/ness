using UnityEngine;
using System;
using System.Collections;

public class Enemy : MonoBehaviour
{
	// The Team the fighter is on
	public Team team;

	public AudioClip takeHitSound;
	public GameObject ragdollPrefab;

	Health health;

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