using UnityEngine;
using System.Collections;

public class Damage
{
	public RaycastHit HitLocation { get; set; }
	public Transform Attacker { get; set; }
	public float Amount { get; set; }
	public AttackData Attack;

	public Damage (float amount, AttackData attackData, RaycastHit hitLocation, Transform attacker)
	{
		HitLocation = hitLocation;
		Amount = amount;
		Attacker = attacker;
		Attack = attackData;
	}

	public Damage (float amount, Transform attacker, RaycastHit hitLocation)
	{
		Amount = amount;
		HitLocation = hitLocation;
		Attacker = attacker;
		Attack = null;
	}
	
	public Damage (float amount, AttackData attackData, Transform attacker)
	{
		Amount = amount;
		HitLocation = new RaycastHit ();
		Attacker = attacker;
		Attack = attackData;
	}
}
