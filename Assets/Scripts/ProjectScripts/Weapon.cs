using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
	public WeaponData weaponData;
	public GameObject attackNode;
	public AttackCast attackCast;
	public GameObject lightProjectile;
	public GameObject heavyProjectile;
	public GameObject enemyHitFX;
	public GameObject wallHitFX;
	
	protected Fighter weaponUser;
	protected Damage damageOut;

	void Awake ()
	{
		attackCast = GetComponentInChildren<AttackCast> ();
	}

	virtual public void BeginAttack (Damage damageToDeal, Fighter attacker)
	{
		weaponUser = attacker;
		attackCast.OnHit += OnWeaponHit;
		damageOut = damageToDeal;
		attackCast.Begin ();
	}

	virtual public void EndAttack ()
	{
		attackCast.OnHit -= OnWeaponHit;
		attackCast.End ();
	}

	/*
	 * Deal damage to the hit object based on the current attack.
	 */
	virtual protected void OnWeaponHit (RaycastHit hit)
	{
		GameObject hitGameObject = hit.transform.gameObject;
		GameObject fx = CFX_SpawnSystem.GetNextObject (wallHitFX);
		if (hitGameObject.CompareTag (Tags.ENEMY)) {
			fx = CFX_SpawnSystem.GetNextObject (enemyHitFX);
		}
		fx.transform.position = hit.point;
		fx.particleSystem.Simulate (0.05f);
		fx.particleSystem.Play ();

		// Assign the hit location to the damage
		damageOut.HitLocation = hit;
		hitGameObject.SendMessage ("ApplyDamage", damageOut, SendMessageOptions.DontRequireReceiver);
		weaponUser.SendMessage ("NotifyAttackHit", SendMessageOptions.DontRequireReceiver);
	}
}