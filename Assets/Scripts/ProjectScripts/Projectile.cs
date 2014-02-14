using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	public bool keepAlive;
	Damage damageOut;
	Team team;
	float startingTime;
	float lifeTime;
	AttackCast attackCast;
	Rigidbody projectileBody;

	void Awake ()
	{
		projectileBody = GetComponentInChildren<Rigidbody> ();
		attackCast = GetComponentInChildren<AttackCast> ();
		keepAlive = true;
	}

	public void Fire (float velocity, float secondsToDeath, Vector3 direction, Damage damage, Team shooterTeam)
	{
		team = shooterTeam;
		damageOut = damage;
		attackCast.OnHit += OnProjectileHit;
		attackCast.Begin ();
		keepAlive = false;
		startingTime = Time.time;
		lifeTime = secondsToDeath;
		projectileBody.AddForce (direction * velocity);
	}
	
	// Kills the projectile if enough time has passed.
	void Update ()
	{
		if (!keepAlive && Time.time - startingTime >= lifeTime) {
			DestroyProjectile ();
		}
	}

	void DestroyProjectile ()
	{
		attackCast.OnHit -= OnProjectileHit;
		attackCast.End ();
		Destroy (gameObject);
	}

	void OnProjectileHit (RaycastHit hit)
	{
		GameObject hitGameObject = hit.transform.gameObject;
		// TODO Refactor the way enemy detection works. Right now this only works for collisions
		// in which the hitobject or its children have enemy, but not the parents (giantguard)
		if (damageOut == null) {
			Debug.Break ();
		}
		damageOut.HitLocation = hit;
		Enemy enemy = hitGameObject.GetComponentInChildren<Enemy> ();
		Fighter fighter = hitGameObject.GetComponentInChildren<Fighter> ();
		if ((enemy != null && enemy.team != team) || (fighter != null && fighter.team != team)) {
			hitGameObject.SendMessage ("ApplyDamage", damageOut, SendMessageOptions.DontRequireReceiver);
			DestroyProjectile ();
		} else if (enemy == null) {
			// Something other than a player or an enemy was hit
			DestroyProjectile ();
		}
	}
}
