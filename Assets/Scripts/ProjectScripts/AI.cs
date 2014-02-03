using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour
{

	public GameObject Target {get; private set;}
	public Enemy enemy;
	CountDownTimer attackCooldown = new CountDownTimer ();
	bool isAttacking;
	
	// Update is called once per frame
	void Update ()
	{
		FindTarget ();

		if (Target != null) {
			isAttacking = enemy.isAttacking;
			float sqrDistanceToTarget = (Target.transform.position - transform.position).sqrMagnitude;
			float ATTACK_RANGE_SQUARED = 16.0f;
			bool isInAttackRange = sqrDistanceToTarget <= ATTACK_RANGE_SQUARED;
			if (isInAttackRange && attackCooldown.IsTimeUp()) {
				enemy.wantsToAttack = true;
				attackCooldown.StartTimer (2.0f);
			} else {
				if(isInAttackRange && !isAttacking) {
					// Stop moving
					enemy.moveDirection = Vector3.zero;
				} else {
					// Approach target until in range. Also approach during attack
					enemy.moveDirection = (Target.transform.position - transform.position);
				}
			}

			// Always face the target
			enemy.faceDirection =  ( Target.transform.position - transform.position);
		}
	}
	
	void FindTarget ()
	{
		Target = GameObject.Find (ObjectNames.PLAYER);
	}
}
