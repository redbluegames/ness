using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackManager : Singleton<AttackManager>
{
	public List<AttackData> attackList = new List<AttackData> ();
	public GoogleFu.Attacks attacksDb;

	void Awake ()
	{
		attacksDb = GetComponent<GoogleFu.Attacks> ();
		if (attacksDb == null) {
			Debug.LogError ("Attack DB not found. Make sure you have run GoogleFU import.");
		}
		ImportAttacks ();
	}

	public int Count ()
	{
		return attackList.Count;
	}
	
	/*
	 * Search through our list of attacks and return the requested Attack.
	 * Must supply the GoogleFU rowId for the desired attack.
	 */
	public AttackData GetAttack (GoogleFu.Attacks.rowIds attackId)
	{
		AttackData ret = GetAttack (attackId.ToString ());
		if (ret == null) {
			Debug.LogError ("Could not find attack for attackId: " + attackId.ToString ());
		}
		return ret;
	}
	
	/*
	 * Search through our list of attacks and returns the requested Attack.
	 * Must supply the GoogleFU rowID as a string value.
	 */
	public AttackData GetAttack (string attackId)
	{
		AttackData ret = null;
		foreach (AttackData attack in attackList) {
			GoogleFu.AttacksRow requestedAttack = attacksDb.GetRow (attackId);
			if (requestedAttack == null) {
				Debug.LogError ("Could not find attack for attackId: " + attackId);
			}
			if (string.Equals (attack.attackName, requestedAttack._NAME)) {
				ret = attack;
			}
		}
		return ret;
	}
	
	public void AddAttack (AttackData attack)
	{
		attackList.Add (attack);
	}
	
	public void ClearAttacks ()
	{
		attackList.Clear ();
	}

	
	/*
	 * Read our AttackDB object (after doing a GoogleFU import) and convert the data
	 * to instances of the Attack class.
	 */
	void ImportAttacks ()
	{
		ClearAttacks ();
		
		foreach (GoogleFu.AttacksRow row in attacksDb.Rows) {
			AttackData newAttack = (AttackData)ScriptableObject.CreateInstance<AttackData> ();
			newAttack.name = row._NAME;
			newAttack.attackName = row._NAME;
			newAttack.minDamage = row._MINDAMAGE;
			newAttack.maxDamage = row._MAXDAMAGE;
			newAttack.strength = ConvertStrengthToEnum (row._STRENGTH);
			newAttack.range = row._RANGE;
			newAttack.controlType = ConvertControlTypeToEnum (row._CONTROLTYPE);
			newAttack.reactionType = ConvertReactionToEnum (row._REACTIONTYPE);
			newAttack.minKnockback = row._MINKNOCKBACK;
			newAttack.maxKnockback = row._MAXKNOCKBACK;
			newAttack.chargeAnimation = LoadClipFromString (row._CHARGEANIMATION);
			newAttack.chargeMovescale = row._CHARGEMOVESCALE;
			newAttack.swingAnimation = LoadClipFromString (row._SWINGANIMATION);
			newAttack.projectilePrefab = row._PROJECTILEPREFAB;
			newAttack.cameraShakeIntensity = row._CAMERASHAKEINTENSITY;
			AddAttack (newAttack);
		}
	}
	
	/*
	 * Maps each ReactionType string to its corresponding enum value.
	 */
	AttackData.ReactionType ConvertReactionToEnum (string typeString)
	{
		if (string.Equals (typeString, "Knockback")) {
			return AttackData.ReactionType.Knockback;
		} else {
			return AttackData.ReactionType.None;
		}
	}

	AttackData.ControlType ConvertControlTypeToEnum (string typeString)
	{
		if (string.Equals (typeString, "Swing")) {
			return AttackData.ControlType.Swing;
		} else if (string.Equals (typeString, "Hold")) {
			return AttackData.ControlType.Hold;
		} else {
			return AttackData.ControlType.Swing;
		}
	}

	AttackData.Strength ConvertStrengthToEnum (string strength)
	{
		if (string.Equals (strength, "Weak")) {
			return AttackData.Strength.Weak;
		} else {
			return AttackData.Strength.Strong;
		}
	}

	/*
	 * Helper method that loads an animation clip using a provided clip name.
	 * This may need refactoring later as it may be better to just store a string
	 * until the animation is needed.
	 */
	AnimationClip LoadClipFromString (string clipname)
	{
		// Note this is not the most efficient time to load new animations as 
		// EVERY single attack animation will be loaded when we run the game.
		return (AnimationClip)Resources.Load (clipname, typeof(AnimationClip));
	}
	
	AudioClip LoadAudioFromString (string clipname)
	{
		// Note this is not the most efficient time to load new sounds as 
		// EVERY single attack sound will be loaded when we run the game.
		return (AudioClip)Resources.Load (clipname, typeof(AudioClip));
	}
}
