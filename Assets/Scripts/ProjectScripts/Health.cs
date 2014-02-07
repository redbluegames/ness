using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
	public int maxHealth = 100;
	public float curHealth = 100.0f;
	const int NO_HEALTH = 0;
	
	void KillOwner (Damage lethalDamage)
	{
		// play death animation
		gameObject.SendMessage ("Die", lethalDamage, SendMessageOptions.RequireReceiver);
	}
	
	public void TakeDamage (Damage incomingDamage)
	{
		AdjustHealth (incomingDamage);
	}
	
	public void Heal (float healthGain)
	{
		//TODO Code review needed here to determine if we need Damage to handle heals.
		Damage healingDamage = new Damage (-healthGain, transform, new RaycastHit ());
		AdjustHealth (healingDamage);
	}

	/*
	 * Adjust object health by a provided amount.
	 */
	protected void AdjustHealth (Damage incomingDamage)
	{
		curHealth -= incomingDamage.Amount;
		Debug.Log ("Health: Subtracting = " + incomingDamage.Amount);
		if (curHealth <= NO_HEALTH) {
			curHealth = NO_HEALTH;
			KillOwner (incomingDamage);
		} else if (curHealth > maxHealth) {
			Debug.LogWarning ("HP set above max. Let's avoid this if user already was full hp.");
			curHealth = maxHealth;
		}
	}

	public float CalculateDisplayPercent ()
	{
		return curHealth / (float)maxHealth;
	}
}
