using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	public bool keepAlive;
	Damage damageOut;
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

	public void Fire (float velocity, float secondsToDeath, Vector3 direction, Damage damage)
	{
		attackCast.OnHit += OnProjectileHit;
		attackCast.Begin ();
		damageOut = damage;
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
		hitGameObject.SendMessage ("ApplyDamage", damageOut, SendMessageOptions.DontRequireReceiver);
		DestroyProjectile ();
	}
}
