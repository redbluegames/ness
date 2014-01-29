using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FighterAI : IController
{
	Fighter fighter;
	
	void Awake ()
	{
		fighter = gameObject.GetComponent<Fighter> ();
	}
	
	/*
	 * Return whether or not the assigned target is now in range of an attack.
	 */
	bool IsTargetInRange (AttackData attack)
	{
		GameObject target = fighter.target;
		// TODO: Check if target is in range for a specified attack
		return Vector3.Distance (transform.position, target.transform.position) <= attack.range;
	}
	
	/*
	 * Decide what the character should do.
	 */
	public override void Think ()
	{
		FindTarget ();
		GameObject fighterTarget = fighter.target;
		// Fall back to patrol if we have no target.
		if (fighterTarget == null) {
			return;
		}

		if (fighter.isAttacking) {
			return;
		}

		// TODO I've officially mucked this up and don't care cause we're redoing the AI very soon
		// Use the attack, or else walk up to the target if not in range.
		if (IsTargetInRange (AttackManager.Instance.GetAttack (GoogleFu.Attacks.rowIds.ENEMY_WEAK))) {
			fighter.AttackWithWeapon (Fighter.AttackType.Weak);
		} else {
			Vector3 moveDirection = fighterTarget.transform.position - transform.position;
			fighter.Run (moveDirection);
		}
	}

	/*
	 * For now, finding a target is as simple as setting it to the only player
	 * in the game.
	 */
	void FindTarget ()
	{
		if (fighter.target == null) {
			GameObject player = GameObject.FindGameObjectWithTag (Tags.PLAYER);
			if (player != null) {
				fighter.LockOnTarget (player);
			}
		}
	}
}
