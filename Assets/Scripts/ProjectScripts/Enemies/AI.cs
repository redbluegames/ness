using UnityEngine;
using System.Collections;

public class AI : MonoBehaviour
{

	public GameObject Target;
	public CharacterMotor motor;
	public Animation attackAnimation;
	public AttackCast attackCaster;
	CountDownTimer attackTime = new CountDownTimer ();
	CountDownTimer attackCooldown = new CountDownTimer ();
	bool isAttacking;
	
	// Update is called once per frame
	void Update ()
	{
	
		FindTarget ();

		if (Target != null) {
			float sqrDistanceToTarget = (Target.transform.position - transform.position).sqrMagnitude;
			float ATTACK_RANGE_SQUARED = 6.0f;
			bool isInAttackRange = sqrDistanceToTarget > ATTACK_RANGE_SQUARED;
			if (isInAttackRange && !isAttacking) {
				// Approach target until in range
				motor.MoveDirection = Target.transform.position - transform.position;
			} else {
				// Attack
				// Stop moving
				motor.MoveDirection = Vector3.zero;

				// Begin or end a running attack
				if (!isAttacking && attackCooldown.IsTimeUp ()) {
					StartAttack ();
				} else if (isAttacking && attackTime.IsTimeUp ()) {
					EndAttack ();
				}

				// Wait for cooldown
			}

			// Always face the target
			motor.FaceDirection = Target.transform.position - transform.position;
		}
	}

	void StartAttack ()
	{
		attackAnimation.Play ();
		attackCaster.OnHit += OnAttackHit;
		attackCaster.Begin ();
		attackTime.StartTimer (attackAnimation.clip.length);
		isAttacking = true;
	}

	void EndAttack ()
	{
		attackCooldown.StartTimer (1.0f);
		attackCaster.End ();
		isAttacking = false;
	}
	
	void FindTarget ()
	{
		Target = GameObject.Find (ObjectNames.PLAYER);
	}

	/*
	 * Deal damage to the hit object based on the current attack.
	 */
	void OnAttackHit (RaycastHit hit)
	{
		Damage damageOut = new Damage (10.0f, transform);
		GameObject hitGameObject = hit.transform.gameObject;
		hitGameObject.SendMessage ("ApplyDamage", damageOut, SendMessageOptions.DontRequireReceiver);
	}
}
