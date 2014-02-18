using UnityEngine;
using System.Collections;

[System.Serializable]
public class AttackData : ScriptableObject
{
	public string attackName;
	public int minDamage;
	public int maxDamage;
	public float range;
	public Strength strength;
	public ControlType controlType;
	public ReactionType reactionType;
	public float minKnockback;
	public float maxKnockback;
	public AnimationClip chargeAnimation;
	public float chargeMovescale;
	public AnimationClip swingAnimation;
	public string projectilePrefab;
	public float cameraShakeIntensity;
	
	public enum ReactionType
	{
		None,
		Knockback
	}
	
	public enum ControlType
	{
		Swing,
		Hold
	}

	public enum Strength
	{
		Weak,
		Strong
	}

	/// <summary>
	/// Determines whether this attack is ranged.
	/// </summary>
	/// <returns><c>true</c> if this attack is ranged; otherwise, <c>false</c>.</returns>
	public bool IsRanged ()
	{
		return !string.Equals (projectilePrefab, string.Empty);
	}
}