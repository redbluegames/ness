using UnityEngine;
using System.Collections;

public class CharacterMotor : MonoBehaviour
{
	[HideInInspector]
	public Vector3
		MoveDirection;
	[HideInInspector]
	public Vector3
		FaceDirection;
	public float TurnDamping = 25.0f;
	public float Speed;
	public float MoveScale = 1.0f;
	public float Gravity = -20.0f;
	float verticalSpeed = 0.0f;
	CollisionFlags collisionFlags;

	void FixedUpdate ()
	{
		ApplyGravity ();

		// Get movement vector
		// Do not let them move up with a basic character controller
		MoveDirection.y = 0.0f;
		Vector3 movement = (MoveDirection.normalized * Speed * MoveScale) + new Vector3 (0.0f, verticalSpeed, 0.0f);
		movement *= Time.deltaTime;
		
		// Apply movement vector
		CharacterController biped = GetComponent<CharacterController> ();
		collisionFlags = biped.Move (movement);

		// Handle Rotation
		Vector3 lookDirection;
		Vector3 movementXZ = new Vector3 (movement.x, 0.0f, movement.z);
		if (FaceDirection != Vector3.zero) {
			lookDirection = FaceDirection;
		} else if (movementXZ != Vector3.zero) {
			// When no FaceDirection is set, default to looking in movement direction
			lookDirection = movementXZ;
		} else {
			// If no FaceDirection is set, and character is not moving, keep whatever facing you had
			lookDirection = Vector3.zero;
		}
		if (lookDirection != Vector3.zero) {
			transform.rotation = Quaternion.Slerp (transform.rotation,
				Quaternion.LookRotation (lookDirection), Time.deltaTime * TurnDamping);
		}

	}
	
	/*
	 * Sets vertical speed to the expected value based on whether or not the character is grounded.
	 */
	void ApplyGravity ()
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
