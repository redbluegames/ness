using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour
{

	public GameObject Target { get; private set; }
	GameObject player;
	Enemy enemy;
	CountDownTimer attackCooldown = new CountDownTimer ();
	const float WAIT_TIME = 3.0f;
	const float SIGHT_DISTANCE = 15.0f;

	void Start ()
	{
		AssignParentEnemy ();
		player = GameObject.Find (SceneObjectNames.PLAYER);
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
		if (Target == null) {
			FindTarget ();
		}

		if (Target != null) {
			Vector3 directionToTarget = (Target.transform.position - transform.position);
			float sqrDistanceToTarget = directionToTarget.sqrMagnitude;
			directionToTarget.Normalize ();
			// Check if we should start an attack
			float ATTACK_RANGE_SQUARED = 16.0f;
			bool isInAttackRange = sqrDistanceToTarget <= ATTACK_RANGE_SQUARED;
			if (isInAttackRange && attackCooldown.IsTimeUp ()) {
				enemy.WantsToAttack = true;
				attackCooldown.StartTimer (WAIT_TIME);
			}

			if (isInAttackRange && !enemy.isAttacking) {
				// Stop moving
				enemy.MoveDirection = Vector3.zero;
			} else {
				// Approach target until in range. Also approach during attack
				enemy.MoveDirection = directionToTarget;
			}
			
			// Always face and move towards the target
			enemy.FaceDirection = directionToTarget;
		}
	}
	
	/// <summary>
	/// Perform a visibility check on the player and return whether
	/// it was seen or not.
	/// </summary>
	bool FindTarget ()
	{
		if (enemy.IsTargetVisible (player, SIGHT_DISTANCE)) {
			Target = player;
			return true;
		}
		return false;
	}
}
