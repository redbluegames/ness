using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour
{

	public GameObject Target { get; private set; }

	Enemy enemy;
	CountDownTimer attackCooldown = new CountDownTimer ();

	void Start ()
	{
		AssignParentEnemy ();
	}

	/// <summary>
	/// Finds and assigns the parent entity this AI will control
	/// </summary>
	void AssignParentEnemy ()
	{
		enemy = transform.parent.GetComponent<Enemy> ();
	}

	// Update is called once per frame
	void Update ()
	{
		FindTarget ();

		if (Target != null) {
			Vector3 directionToTarget = (Target.transform.position - transform.position);
			float sqrDistanceToTarget = (Target.transform.position - transform.position).sqrMagnitude;
			// Check if we should start an attack
			float ATTACK_RANGE_SQUARED = 16.0f;
			bool isInAttackRange = sqrDistanceToTarget <= ATTACK_RANGE_SQUARED;
			if (isInAttackRange && attackCooldown.IsTimeUp ()) {
				enemy.wantsToAttack = true;
				attackCooldown.StartTimer (3.0f);
			}

			if (isInAttackRange && !enemy.isAttacking) {
				// Stop moving
				enemy.throttle = 0.0f;
			} else {
				// Approach target until in range. Also approach during attack
				enemy.throttle = 1.0f;
			}
			
			// Always face and move towards the target
			enemy.moveDirection = directionToTarget;
			enemy.faceDirection = directionToTarget;
		}
	}

	/// <summary>
	/// Performs a search for a target and assigns it to remember it.
	/// </summary>
	void FindTarget ()
	{
		Target = GameObject.Find (ObjectNames.PLAYER);
	}
}
