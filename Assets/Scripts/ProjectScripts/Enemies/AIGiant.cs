using UnityEngine;
using System.Collections;

public class AIGiant : MonoBehaviour
{
	public GameObject Target;
	public Animation attackAnimation;
	TrailRenderer trailRenderer;
	public AttackCast attackCaster;
	CountDownTimer attackTime = new CountDownTimer ();
	CountDownTimer attackCooldown = new CountDownTimer ();
	bool isAttacking;
	float SIGHT_DISTANCE = 50.0f;

	// Attacks
	AttackData intendedAttack; // Attack chosen for execution
	AttackData lightAttack;
	AttackData heavyAttack;

	// Cached references
	Enemy enemy;

	void Start ()
	{
		AssignParentEnemy ();
		trailRenderer = transform.parent.GetComponentInChildren<TrailRenderer> ();
		lightAttack = AttackManager.Instance.GetAttack (GoogleFu.Attacks.rowIds.GIANT_SWING);
		heavyAttack = AttackManager.Instance.GetAttack (GoogleFu.Attacks.rowIds.GIANT_BIGSWING);
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
			if (intendedAttack == null) {
				intendedAttack = ChooseAttack ();
			}
			float sqrDistanceToTarget = (Target.transform.position - transform.position).sqrMagnitude;
			bool targetInRange = sqrDistanceToTarget < intendedAttack.range;
			bool targetInSight = enemy.IsTargetVisible (Target, SIGHT_DISTANCE);
			if (!targetInRange && !isAttacking) {
				// Approach target until in range
				enemy.MoveDirection = Target.transform.position - transform.position;
			} else {
				// Stop moving
				enemy.MoveDirection = Vector3.zero;

				// Begin or end a running attack
				if (!isAttacking && attackCooldown.IsTimeUp ()) {
					StartAttack (intendedAttack);
					intendedAttack = null;
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

	AttackData ChooseAttack ()
	{
		AttackData ret;
		// We can have smarter choices than 50 / 50 but for now this works.
		if (RBRandom.PercentageChance (50)) {
			ret = lightAttack;
		} else {
			ret = heavyAttack;
		}
		return ret;
	}

	void StartAttack (AttackData attackToStart)
	{
		// TODO This is bad practice. When we refactor this, let's properly enapsulate
		// the current attack in one of these classes.
		enemy.currentAttack = attackToStart;
		attackAnimation.Play (attackToStart.swingAnimation.name);
		attackCaster.OnHit += OnAttackHit;
		attackCaster.Begin ();
		trailRenderer.enabled = true;
		attackTime.StartTimer (attackToStart.swingAnimation.length);
		isAttacking = true;
	}

	void EndAttack ()
	{
		attackCooldown.StartTimer (0.5f);
		attackCaster.End ();
		trailRenderer.enabled = false;
		isAttacking = false;
		// TODO This is bad practice. When we refactor this, let's properly enapsulate
		// the current attack in one of these classes.
		enemy.currentAttack = null;
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
		Damage damageOut = new Damage (20.0f, enemy.currentAttack, hit, transform.parent.transform);
		GameObject hitGameObject = hit.transform.gameObject;
		if (hitGameObject.CompareTag (Tags.PLAYER)) {
			hitGameObject.SendMessage ("ApplyDamage", damageOut, SendMessageOptions.DontRequireReceiver);
		}
	}
}
