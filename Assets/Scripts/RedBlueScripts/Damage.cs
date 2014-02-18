using UnityEngine;
using System.Collections;

public class Damage
{
	public RaycastHit HitLocation { get; set; }
	public Transform Attacker { get; set; }
	public float Amount { get; set; }
	public AttackData Attack;

	public Damage (float amount, AttackData attackData, RaycastHit hitLocation, Transform attackerTransform)
	{
		HitLocation = hitLocation;
		Amount = amount;
		Attacker = attackerTransform;
		Attack = attackData;
	}
}
