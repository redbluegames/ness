using UnityEngine;
using System.Collections;

public class Damage
{
	public RaycastHit HitLocation { get; set; }
	public Transform Attacker { get; set; }
	public float Amount { get; set; }
	public AttackData.ReactionType HitReaction { get; set; }

	public Damage (float amount, AttackData.ReactionType hitReaction, RaycastHit hitLocation, Transform attacker)
	{
		HitLocation = hitLocation;
		Amount = amount;
		Attacker = attacker;
		HitReaction = hitReaction;
	}

	public Damage (float amount, Transform attacker, RaycastHit hitLocation)
	{
		Amount = amount;
		HitLocation = hitLocation;
		Attacker = attacker;
		HitReaction = AttackData.ReactionType.None;
	}
	
	public Damage (float amount, AttackData.ReactionType hitReaction, Transform attacker)
	{
		Amount = amount;
		HitLocation = new RaycastHit ();
		Attacker = attacker;
		HitReaction = hitReaction;
	}
}
