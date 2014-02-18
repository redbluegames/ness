using UnityEngine;
using System.Collections;

public class Spear : Weapon
{
	public CountUpTimer attackTime;

	/*
	 * Deal damage to the hit object based on the current attack.
	 */
	override protected void OnWeaponHit (RaycastHit hit)
	{
		Damage actualDamage = new Damage(damageOut);
		actualDamage.Amount = Mathf.Ceil(actualDamage.Amount * weaponUser.ChargePercentage);

		GameObject hitGameObject = hit.transform.gameObject;
		GameObject fx = CFX_SpawnSystem.GetNextObject (wallHitFX);
		if (hitGameObject.CompareTag (Tags.ENEMY)) {
			fx = CFX_SpawnSystem.GetNextObject (enemyHitFX);
		}
		fx.transform.position = hit.point;
		fx.particleSystem.Simulate (0.05f);
		fx.particleSystem.Play ();

		// Assign the hit location to the damage
		actualDamage.HitLocation = hit;
		hitGameObject.SendMessage ("ApplyDamage", actualDamage, SendMessageOptions.DontRequireReceiver);
		weaponUser.SendMessage ("NotifyAttackHit", SendMessageOptions.DontRequireReceiver);
	}
}