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
	public AnimationClip swingAnimation;
	public string projectilePrefab;
	
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
}
