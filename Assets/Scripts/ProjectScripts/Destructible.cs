using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Health))]
public class Destructible : MonoBehaviour {

	public GameObject deathFXPrefab;
	Health health;

	void Start ()
	{
		health = gameObject.GetComponent<Health> ();
	}

	/*
	 * Resolve a hit and perform the appropriate reaction. This may mean
	 * taking damage or it may mean resolving a block.
	 */
	public void ApplyDamage (Damage damageFromHit)
	{
		health.TakeDamage (damageFromHit);
	}
	
	void Die (Damage lethalDamage)
	{
		// Remove the current collider so that the pieces don't unembed.
		collider.enabled = false;

		GameObject deathFX = (GameObject)Instantiate (deathFXPrefab, transform.position, transform.rotation);

		// ToDo: Apply force from damage onto any rigid bodies

		Destroy (deathFX, 3.0f);
		Destroy (gameObject);
	}
}
