using UnityEngine;
using System.Collections;

public class ShootingTrap : MonoBehaviour
{
	public GameObject projectilePrefab;        // Prefab to shoot
	public float offset;                       // Used for staggering traps
	public float delay = 1.5f;                 // Time between shots in seconds

	[Signal]
	void StartShooting ()
	{
		StartCoroutine (ShootPeriodically ());
	}

	[Signal]
	void StopShooting ()
	{
		StopAllCoroutines ();
	}

	IEnumerator ShootPeriodically ()
	{
		yield return new WaitForSeconds (offset);
		while (enabled) {
			AttackData attack = AttackManager.Instance.GetAttack (GoogleFu.Attacks.rowIds.ENEMY_FIREBALL);
			Damage damageOut = new Damage (attack.maxDamage, attack, new RaycastHit (), transform);
			GameObject projectile = (GameObject)Instantiate (projectilePrefab, transform.position, 
			                                                 projectilePrefab.transform.localRotation);
			projectile.GetComponent<Projectile> ().Fire (500.0f, 5.0f, transform.forward,
			                                            damageOut, 1.0f, Team.BadGuys);
			yield return new WaitForSeconds (delay);
		}
	}
}
