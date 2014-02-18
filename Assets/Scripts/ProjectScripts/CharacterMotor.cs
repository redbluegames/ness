using UnityEngine;
using System.Collections;

public class CharacterMotor : MonoBehaviour
{
	public Vector3 MoveDirection;
	public Vector3 FaceDirection;
	public float TurnDamping = 25.0f;
	public float Speed = 5.0f;
	public float MoveScale = 1.0f;
	public float Gravity = -20.0f;
	public float Mass = 1.0f;
	public LayerMask PushLayer = 1 << Layers.RAGDOLL;
	float verticalSpeed = 0.0f;
	CollisionFlags collisionFlags;
	Animator animator;
	Vector3 lastMovement;
	bool isPositionScriptDriven;
	
	void Start ()
	{
		AssignReferences ();
		isPositionScriptDriven = animator == null;
	}
	
	void AssignReferences ()
	{
		animator = GetComponentInChildren<Animator> ();
	}

	void FixedUpdate ()
	{
		// When character root motion does not come from an animation, it must be updated on update
		if (isPositionScriptDriven) {
			MoveCharacter ();
			UpdateFacing ();
		}
	}

	// Push rigid bodies we come in contact with
	void OnControllerColliderHit(ControllerColliderHit hit) {

		// Only push the ragdoll layer for now
		if(!Layers.IsObjectOnLayerMask(hit.gameObject, PushLayer)) {
			return;
		}

		Rigidbody body = hit.collider.attachedRigidbody;

		// Only push rigidbodies
		if (body == null || body.isKinematic)
			return;
		// Do not push things that are below us
		if (hit.moveDirection.y < -0.4f)
			return;

		// Only push in XZ direction
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
		// Multiply push vector by the character's speed to give believeable push amount
		Vector3 pushForce = pushDir * hit.controller.velocity.magnitude * Mass;
		
		// Add Force instead of modifying velocity directly. This takes the rigid body's mass
		// into account, and prevents stomping of velocity.
		body.AddForceAtPosition (pushForce, hit.point);

	}

	void OnAnimatorMove ()
	{
		// Get speed from animation
		Speed = animator.deltaPosition.magnitude;
		MoveCharacter ();
		UpdateFacing ();
	}
	
	void MoveCharacter ()
	{
		
		// Get movement vector
		// Do not let them move up with a basic character controller
		MoveDirection.y = 0.0f;
		if (MoveDirection == Vector3.zero) {
			Debug.LogError ("CharacterMotor receiving zero moveDirection for object: " + gameObject.name);
		}
		Vector3 movement = (MoveDirection.normalized * Speed * MoveScale);
		// When Moving comes from updates (script driven vs. animation driven) account for delta time
		if (isPositionScriptDriven) {
			movement *= Time.deltaTime;
		}

		// Add in gravity
		AdjustVerticalSpeedForGravity ();
		movement += new Vector3 (0.0f, verticalSpeed, 0.0f);
		
		// Apply movement vector
		CharacterController biped = GetComponent<CharacterController> ();
		collisionFlags = biped.Move (movement);
		lastMovement = movement;
	}

	void UpdateFacing ()
	{
		Vector3 targetFaceDirection;
		Vector3 movementXZ = new Vector3 (lastMovement.x, 0.0f, lastMovement.z);
		if (FaceDirection != Vector3.zero) {
			// Take the specified face direction
			targetFaceDirection = FaceDirection;
		} else if (movementXZ != Vector3.zero) {
			// When no FaceDirection is set, default to looking in movement direction
			targetFaceDirection = movementXZ;
		} else {
			// If no FaceDirection is set, and the character is not moving, keep whatever facing you had
			targetFaceDirection = Vector3.zero;
		}
		if (targetFaceDirection != Vector3.zero) {
			transform.rotation = Quaternion.Slerp (transform.rotation,
			                                       Quaternion.LookRotation (targetFaceDirection), Time.deltaTime * TurnDamping);
		}
	}
	
	/*
	 * Sets vertical speed to the expected value based on whether or not the character is grounded.
	 */
	void AdjustVerticalSpeedForGravity ()
	{
		if (IsGrounded ()) {
			verticalSpeed = 0.0f;
		} else {
			verticalSpeed += Gravity * Time.deltaTime;
		}
	}
	
	/*
	 * Checks to see if the character is grounded by checking collision flags.
	 */
	bool IsGrounded ()
	{
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}
}
