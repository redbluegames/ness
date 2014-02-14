using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class CapsuleRigidBodyMotor : MonoBehaviour
{
	public Vector3 MoveDirection;
	public Vector3 FaceDirection;
	public float TurnDamping = 25.0f;
	[Range(1,4)]
	[SerializeField]
	public float
		gravityMultiplier = 1;	// gravity modifier - often higher than natural gravity feels right for game characters
	[SerializeField]
	[Range(0.1f,3f)]
	public float
		MoveScale = 1;	    // how much the move speed of the character will be multiplied by
	[SerializeField]
	AdvancedSettings
		advancedSettings;                 // Container for the advanced settings class , this allows the advanced settings to be in a foldout in the inspector
	[System.Serializable]
	public class AdvancedSettings
	{
		public PhysicMaterial zeroFrictionMaterial;			// used when in motion to enable smooth movement
		public PhysicMaterial highFrictionMaterial;			// used when stationary to avoid sliding down slopes
		public float groundStickyEffect = 5f;				// power of 'stick to ground' effect - prevents bumping down slopes.
	}

	public bool IsOnGround { get; private set; }             // Is the character on the ground
	float originalHeight;                                   // Used for tracking the original height of the characters capsule collider
	Animator animator;                                      // The animator for the character
	CapsuleCollider capsule;                                // The collider for the character
	const float half = 0.5f;                                // whats it says, it's a constant for a half
	Vector3 velocity;

	// Use this for initialization
	void Start ()
	{
		animator = GetComponentInChildren<Animator> ();
		capsule = collider as CapsuleCollider;

		// as operator can return null so we need to make sure that its not before assigning to it
		if (capsule != null) {
			originalHeight = capsule.height;
			capsule.center = Vector3.up * originalHeight * half;
		} else {
			Debug.LogError (" collider cannot be cast to CapsuleCollider");
		}
		SetUpAnimator ();
	}
	
	void FixedUpdate ()
	{
		// grab current velocity, we will be changing it.
		velocity = rigidbody.velocity;

		GroundCheck (); // detect and stick to ground

		SetFriction (); // use low or high friction values depending on the current state

		// control and velocity handling is different when grounded and airborne:
		if (IsOnGround) {
			HandleGroundedVelocities ();
		} else {
			HandleAirborneVelocities ();
		}
	
		UpdateAnimator (); // send input and other state parameters to the animator

		// reassign velocity, since it will have been modified by the above functions.
		//rigidbody.velocity = velocity;
		Vector3 targetFaceDirection = FaceDirection;
		// Don't let them pitch up
		targetFaceDirection.y = 0.0f;
		rigidbody.rotation = Quaternion.Slerp (rigidbody.rotation,
		                                       Quaternion.LookRotation (targetFaceDirection), Time.deltaTime * TurnDamping);
	}

	void GroundCheck ()
	{
		// Cast for the ground
		float padding = .1f;
		Ray ray = new Ray (transform.position + Vector3.up * padding, -Vector3.up);
		float groundCastDistance = capsule.center.y + padding;
		RaycastHit[] hits = Physics.RaycastAll (ray, groundCastDistance);
		System.Array.Sort (hits, new RayHitComparer ());

		IsOnGround = false;
		rigidbody.useGravity = true;
		foreach (var hit in hits) {
			// check whether we hit a non-trigger collider (and not the character itself)
			if (!hit.collider.isTrigger) {
				// this counts as being on ground.

				// stick to surface - helps character stick to ground - specially when running down slopes
				if (velocity.y <= 0) {
					rigidbody.position = Vector3.MoveTowards (rigidbody.position, hit.point, Time.deltaTime * advancedSettings.groundStickyEffect);
				}

				IsOnGround = true;
				rigidbody.useGravity = false;
				break;
			}
		}

	}

	void SetFriction ()
	{
		if (IsOnGround) {
			// set friction to low or high, depending on if we're moving
			if (MoveDirection.magnitude == 0) {
				// when not moving this helps prevent sliding on slopes:
				collider.material = advancedSettings.highFrictionMaterial;
			} else {
				// but when moving, we want no friction:
				collider.material = advancedSettings.zeroFrictionMaterial;
			}
		} else {
			// while in air, we want no friction against surfaces (walls, ceilings, etc)
			collider.material = advancedSettings.zeroFrictionMaterial;
		}
	}

	void HandleGroundedVelocities ()
	{
		velocity.y = 0;

		if (MoveDirection.magnitude == 0) {
			// when not moving this prevents sliding on slopes:
			velocity.x = 0;
			velocity.z = 0;
		}
	}

	void HandleAirborneVelocities ()
	{
		rigidbody.useGravity = true;
		// apply extra gravity from multiplier:
		Vector3 extraGravityForce = (Physics.gravity * gravityMultiplier) - Physics.gravity;
		rigidbody.AddForce (extraGravityForce);
	}

	void UpdateAnimator ()
	{
		// Here we tell the animator what to do based on the current states and inputs.

		// only use root motion when on ground:
		animator.applyRootMotion = IsOnGround;
	}
	
	void SetUpAnimator ()
	{
		// this is a ref to the animator component on the root.
		animator = GetComponent<Animator> ();
		
		// we use avatar from a child animator component if present
		// (this is to enable easy swapping of the character model as a child node)
		foreach (var childAnimator in GetComponentsInChildren<Animator>()) {
			if (childAnimator != animator) {
				animator.avatar = childAnimator.avatar;
				Destroy (childAnimator);
				break;
			}
		}
	}
	
	public void OnAnimatorMove ()
	{
		// we implement this function to override the default root motion.
		// this allows us to modify the positional speed before it's applied.
		if (IsOnGround && Time.deltaTime > 0) {
			Vector3 v = MoveDirection;

			// Divide by delta time to get velocty in m/s
			v *= (animator.deltaPosition.magnitude * MoveScale) / Time.deltaTime;

			// we preserve the existing y part of the current velocity.
			v.y = rigidbody.velocity.y;
			rigidbody.velocity = v;
		}
	}

	//used for comparing distances
	class RayHitComparer: IComparer
	{
		public int Compare (object x, object y)
		{
			return ((RaycastHit)x).distance.CompareTo (((RaycastHit)y).distance);
		}	
	}
}