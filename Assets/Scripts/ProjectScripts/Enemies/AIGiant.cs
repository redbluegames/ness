using UnityEngine;
using System.Collections;

public class AIGiant : MonoBehaviour
{
	public GameObject Target;
	Enemy enemy;
	public AnimationClip swingAttackAnimation;
	public AnimationClip bigSwingAttackAnimation;
	public Animation attackAnimation;
	TrailRenderer trailRenderer;
	public AttackCast attackCaster;
	CountDownTimer attackTime = new CountDownTimer ();
	CountDownTimer attackCooldown = new CountDownTimer ();
	bool isAttacking;
	
	void Start ()
	{
		AssignParentEnemy ();
		trailRenderer = transform.parent.GetComponentInChildren<TrailRenderer> ();
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
			float sqrDistanceToTarget = (Target.transform.position - transform.position).sqrMagnitude;
			float ATTACK_RANGE_SQUARED = 14.0f;
			bool isInAttackRange = sqrDistanceToTarget > ATTACK_RANGE_SQUARED;
			if (isInAttackRange && !isAttacking) {
				// Approach target until in range
				enemy.MoveDirection = Target.transform.position - transform.position;
			} else {
				// Attack
				// Stop moving
				enemy.MoveDirection = Vector3.zero;

				// Begin or end a running attack
				if (!isAttacking && attackCooldown.IsTimeUp ()) {
					bool bigAttack = RBRandom.PercentageChance (50);
					if (!bigAttack) {
						StartAttack (swingAttackAnimation);
					} else {
						StartAttack (bigSwingAttackAnimation);
					}
				} else if (isAttacking && attackTime.IsTimeUp ()) {
					EndAttack ();
				}

				// Wait for cooldown
			}

			// Always face the target, ignoring y coordinates
			if (!isAttacking) {
				float yFaceDirection = transform.position.y;
				enemy.FaceDirection = Target.transform.position - transform.position;
				enemy.FaceDirection.y = yFaceDirection;
			}
		}
	}

	void StartAttack (AnimationClip attackClip)
	{
		attackAnimation.Play (attackClip.name);
		attackCaster.OnHit += OnAttackHit;
		attackCaster.Begin ();
		trailRenderer.enabled = true;
		attackTime.StartTimer (attackClip.length);
		isAttacking = true;
	}

	void EndAttack ()
	{
		attackCooldown.StartTimer (0.5f);
		attackCaster.End ();
		trailRenderer.enabled = false;
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
		Damage damageOut = new Damage (20.0f, transform.parent.transform, hit);
		GameObject hitGameObject = hit.transform.gameObject;
		if (hitGameObject.CompareTag (Tags.PLAYER)) {
			hitGameObject.SendMessage ("ApplyDamage", damageOut, SendMessageOptions.DontRequireReceiver);
		}
	}
}
