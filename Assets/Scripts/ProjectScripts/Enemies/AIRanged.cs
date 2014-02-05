using UnityEngine;
using System.Collections;

public class AIRanged : MonoBehaviour
{

	public GameObject Target;
	public GameObject projectilePrefab;
	public GoogleFu.Attacks.rowIds attackId;
	AttackData attack;
	public CharacterMotor motor;
	public Animation attackAnimation;
	public AttackCast attackCaster;
	CountDownTimer attackTime = new CountDownTimer ();
	CountDownTimer attackCooldown = new CountDownTimer ();
	bool isAttacking;
	float ATTACK_RANGE_SQUARED = 100.0f;
	float SIGHT_DISTANCE = 50.0f;
	bool targetInRange;
	bool targetInSight;
	Vector3 lastSeenTargetPosition;

	void Start ()
	{
		attack = AttackManager.Instance.GetAttack (attackId);
	}

	// Update is called once per frame
	void Update ()
	{
	
		FindTarget ();

		if (Target != null) {
			float sqrDistanceToTarget = (Target.transform.position - transform.position).sqrMagnitude;
			targetInRange = sqrDistanceToTarget <= ATTACK_RANGE_SQUARED;
			targetInSight = IsTargetVisible ();
			if (!targetInSight) {
				motor.MoveDirection = lastSeenTargetPosition - transform.position;
			} else if (!targetInRange && !isAttacking) {
				// Approach target until in range
				motor.MoveDirection = Target.transform.position - transform.position;
			} else if ((targetInRange || isAttacking)) {
				// Attack
				// Stop moving
				motor.MoveDirection = Vector3.zero;

				// Begin or end a running attack
				if (!isAttacking && attackCooldown.IsTimeUp ()) {
					StartAttack ();
				} else if (isAttacking && attackTime.IsTimeUp ()) {
					EndAttack ();
				}
			}

			// Always face the target
			motor.FaceDirection = Target.transform.position - transform.position;
		}
	}

	/*
	 * Perform a raycast and return whether the closest collision is
	 * with the target.
	 */
	bool IsTargetVisible ()
	{
		Vector3 targetDirection = (Target.transform.position - transform.position);
		int allButEnemies = ~(1 << Layers.ENEMY); // 1s for all but Enemies layer
		RaycastHit[] hits = Physics.RaycastAll (transform.position, targetDirection, SIGHT_DISTANCE,
		                                        GameManager.Instance.LineOfSightMask & allButEnemies);
		if (hits.Length <= 0) {
			return false;
		}
		// Look through all hits and store off the closest
		RaycastHit closestHit = hits [0];
		for (int i = 0; i < hits.Length; ++i) {
			if (hits [i].distance < closestHit.distance) {
				closestHit = hits [i];
			}
		}
		bool targetVisible = closestHit.collider.gameObject == Target;
		if (targetVisible) {
			lastSeenTargetPosition = closestHit.collider.transform.position;
		}
		return targetVisible;
	}

	void StartAttack ()
	{
		attackAnimation.Play ();
		FireProjectileWeapon ();
		attackTime.StartTimer (attackAnimation.clip.length);
		isAttacking = true;
	}

	void EndAttack ()
	{
		attackCooldown.StartTimer (0.0f);
//		attackCaster.End ();
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
	
	/*
	 * Spawn a projectile and fire it away from the enemy.
	 */
	void FireProjectileWeapon ()
	{
		// Play firing animation
//		animation.Play (attack.swingAnimation.name, PlayMode.StopAll);
		
		// Spawn and fire projectile
		Damage damageOut = new Damage (attack.maxDamage, attack.reactionType, transform);
		GameObject newProjectile = (GameObject)Instantiate (
			projectilePrefab, transform.position, transform.rotation);
		newProjectile.GetComponent<Projectile> ().Fire (2000.0f, 5.0f, transform.forward, damageOut, Team.BadGuys);
	}
}
