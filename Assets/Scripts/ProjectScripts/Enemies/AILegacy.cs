using UnityEngine;
using System.Collections;

public class AILegacy : MonoBehaviour
{
	public GameObject Target { get; private set; }
	GameObject player;
	Enemy enemy;
	Animation attackAnimation;
	TrailRenderer trailRenderer;
	public AttackCast attackCaster;
	GameObject projectilePrefab;
	public Transform projectileSpawnPoint;
	CountDownTimer attackTime = new CountDownTimer ();
	CountDownTimer attackCooldown = new CountDownTimer ();
	bool isAttacking;

	// Attacks
	AttackData intendedAttack; // Attack chosen for execution
	AttackData[] attacks;

	// Configurable Propertiees (we can set up an abstract class to avoid using the Unity editor)
	public float sightDistance = 50.0f;
	public GoogleFu.Attacks.rowIds[] attackIds;
	public float cooldownTime = 0.5f;
	
	void Start ()
	{
		SetupReferences ();
	}
	 
	/// <summary>
	/// Finds and assigns required references.
	/// </summary>
	void SetupReferences ()
	{
		Transform enemyRootObj = transform.parent;
		player = GameObject.Find (SceneObjectNames.PLAYER);
		enemy = enemyRootObj.GetComponent<Enemy> ();
		attackAnimation = enemyRootObj.GetComponentInChildren<Animation> ();
		trailRenderer = enemyRootObj.GetComponentInChildren<TrailRenderer> ();
		projectilePrefab = (GameObject)Resources.Load ("proj_fireball");
		attacks = new AttackData[attackIds.Length];
		for (int i = 0; i < attacks.Length; ++i) {
			attacks [i] = AttackManager.Instance.GetAttack (attackIds [i]);
		}
		if (attacks.Length == 0) {
			Debug.LogError (string.Format ("Attacks for {0}.{1} not wired up correctly.", 
			                               enemyRootObj.name, name));
		}
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
			float sqrRange = intendedAttack.range * intendedAttack.range;
			bool targetInRange = sqrDistanceToTarget < sqrRange;
			bool targetInSight = enemy.IsTargetVisible (Target, sightDistance);
			if (!targetInSight) {
				enemy.MoveDirection = enemy.lastSeenTargetPosition - transform.position;
			} else if (!targetInRange && !isAttacking) {
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
					if (enemy.currentAttack.IsRanged ()) {
						FireProjectileWeapon (enemy.currentAttack);
					}
					EndAttack ();
				}

				// Wait for cooldown
			}

			// Always face the target, ignoring y coordinates
			if (!isAttacking || (isAttacking && enemy.currentAttack.IsRanged ())) {
				enemy.FaceDirection = Target.transform.position - transform.position;
				enemy.FaceDirection.y = 0;
			}
		}
	}

	/// <summary>
	/// Determine the appropriate attack given the enemy's situation. For now, let's
	/// just choose at random from the available attacks but in the future this is
	/// an area for smarter AI.
	/// </summary>
	/// <returns>The chosen attack</returns>
	AttackData ChooseAttack ()
	{
		// Randomly pick an attack. Do this differently for smarter AI.
		return attacks [Random.Range (0, attacks.Length)];
	}

	/// <summary>
	/// Starts the attack, activating the cast if needed, playing the animation, and starting
	/// timer for the attack.
	/// </summary>
	/// <param name="attackToStart">Attack to start.</param>
	void StartAttack (AttackData attackToStart)
	{
		// TODO This is bad practice. When we refactor this, let's properly enapsulate
		// the current attack in one of these classes.
		enemy.currentAttack = attackToStart;
		attackAnimation.Play (attackToStart.swingAnimation.name);
		if (!attackToStart.IsRanged ()) {
			attackCaster.OnHit += OnAttackHit;
			attackCaster.Begin ();
			trailRenderer.enabled = true;
		}
		attackTime.StartTimer (attackToStart.swingAnimation.length);
		isAttacking = true;
	}

	/// <summary>
	/// Ends the attack, turning off the trails and caster and beginning the cooldown timer.
	/// </summary>
	void EndAttack ()
	{
		attackCooldown.StartTimer (cooldownTime);
		if (!enemy.currentAttack.IsRanged ()) {
			attackCaster.End ();
			trailRenderer.enabled = false;
		}
		isAttacking = false;
		// TODO This is bad practice. When we refactor this, let's properly enapsulate
		// the current attack in one of these classes.
		enemy.currentAttack = null;
	}

	/// <summary>
	/// Perform a visibility check on the player and return whether
	/// it was seen or not.
	/// </summary>
	bool FindTarget ()
	{
		if (enemy.IsTargetVisible (player, sightDistance)) {
			Target = player;
			return true;
		}
		return false;
	}

	/// <summary>
	/// Deal damage to the hit object based on the current attack.
	/// </summary>
	/// <param name="hit">Hit.</param>
	void OnAttackHit (RaycastHit hit)
	{
		Damage damageOut = new Damage (20.0f, enemy.currentAttack, hit, transform.parent.transform);
		GameObject hitGameObject = hit.transform.gameObject;
		if (hitGameObject.CompareTag (Tags.PLAYER)) {
			hitGameObject.SendMessage ("ApplyDamage", damageOut, SendMessageOptions.DontRequireReceiver);
		}
	}

	/// <summary>
	///  Spawn a projectile and fire it away from the enemy.
	/// </summary>
	/// <param name="projectileAttack">Projectile attack.</param>
	void FireProjectileWeapon (AttackData projectileAttack)
	{
		// Play firing animation
		//		animation.Play (attack.swingAnimation.name, PlayMode.StopAll);
		// Spawn and fire projectile
		Damage damageOut = new Damage (projectileAttack.maxDamage, projectileAttack, new RaycastHit (), transform);
		GameObject newProjectile = (GameObject)Instantiate (
				projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
		Vector3 projectileToPlayer = Target.transform.position - projectileSpawnPoint.position;
		projectileToPlayer.y = 0;
		newProjectile.GetComponent<Projectile> ().Fire (150.0f, 5.0f, projectileToPlayer, damageOut, 1.0f, Team.BadGuys);
	}
}
