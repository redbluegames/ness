using UnityEngine;
using System.Collections;

public class AIRanged : MonoBehaviour
{

	public GameObject Target;
	public GameObject projectilePrefab;
	public GoogleFu.Attacks.rowIds attackId;
	public CharacterMotor motor;
	public Animation attackAnimation;
	public AttackCast attackCaster;
	CountDownTimer attackTime = new CountDownTimer ();
	CountDownTimer attackCooldown = new CountDownTimer ();
	bool isAttacking;
	float ATTACK_RANGE_SQUARED = 100.0f;
	float SIGHT_DISTANCE = 50.0f;

	// Attacks
	AttackData attack;

	// Cached objects
	Enemy enemy;

	void Start ()
	{
		attack = AttackManager.Instance.GetAttack (attackId);
		enemy = transform.parent.GetComponent<Enemy> ();
	}

	// Update is called once per frame
	void Update ()
	{
	
		FindTarget ();

		if (Target != null) {
			float sqrDistanceToTarget = (Target.transform.position - transform.position).sqrMagnitude;
			bool targetInRange = sqrDistanceToTarget <= ATTACK_RANGE_SQUARED;
			bool targetInSight = enemy.IsTargetVisible (Target, SIGHT_DISTANCE);
			if (!targetInSight) {
				motor.MoveDirection = enemy.lastSeenTargetPosition - transform.position;
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
		Target = GameObject.Find (SceneObjectNames.PLAYER);
	}

	/*
	 * Deal damage to the hit object based on the current attack.
	 */
	void OnAttackHit (RaycastHit hit)
	{
		Damage damageOut = new Damage (10.0f, attack, hit, transform);
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
		Damage damageOut = new Damage (attack.maxDamage, attack, new RaycastHit (), transform);
		GameObject newProjectile = (GameObject)Instantiate (
			projectilePrefab, transform.position, transform.rotation);
		newProjectile.GetComponent<Projectile> ().Fire (2000.0f, 5.0f, transform.forward, damageOut, 1.0f, Team.BadGuys);
	}
}
