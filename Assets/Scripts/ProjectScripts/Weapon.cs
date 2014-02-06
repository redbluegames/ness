using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
	public WeaponData weaponData;
	public GameObject attackNode;
	public AttackCast attackCast;
	public GameObject lightProjectile;
	public GameObject heavyProjectile;
	Fighter weaponUser;
	public GameObject enemyHitFX;
	public GameObject wallHitFX;

	Damage damageOut;

	void Awake ()
	{
		attackCast = GetComponentInChildren<AttackCast> ();
	}

	public void BeginAttack (Damage damageToDeal, Fighter attacker)
	{
		weaponUser = attacker;
		attackCast.OnHit += OnWeaponHit;
		damageOut = damageToDeal;
		attackCast.Begin ();
	}

	public void EndAttack ()
	{
		attackCast.OnHit -= OnWeaponHit;
		attackCast.End ();
	}

	/*
	 * Deal damage to the hit object based on the current attack.
	 */
	void OnWeaponHit (RaycastHit hit)
	{
		GameObject hitGameObject = hit.transform.gameObject;
		GameObject fx = CFX_SpawnSystem.GetNextObject (wallHitFX);
		if (hitGameObject.CompareTag (Tags.ENEMY)) {
			fx = CFX_SpawnSystem.GetNextObject (enemyHitFX);
		}
		fx.transform.position = hit.point;
		fx.particleSystem.Simulate (0.05f);
		fx.particleSystem.Play ();

		hitGameObject.SendMessage ("ApplyDamage", damageOut, SendMessageOptions.DontRequireReceiver);
		weaponUser.SendMessage ("NotifyAttackHit", SendMessageOptions.DontRequireReceiver);
	}
}