using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
	public WeaponData weaponData;
	public GameObject attackNode;
	public AttackCast attackCast;
	public GameObject lightProjectile;
	public GameObject heavyProjectile;

	Damage damageOut;

	void Awake ()
	{
		attackCast = GetComponentInChildren<AttackCast> ();
	}

	public void BeginAttack (Damage damageToDeal)
	{
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
		hitGameObject.SendMessage ("ApplyDamage", damageOut, SendMessageOptions.DontRequireReceiver);
	}
}