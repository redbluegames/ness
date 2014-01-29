using UnityEngine;
using System.Collections;

[System.Serializable]
public class WeaponData : ScriptableObject
{
	public string weaponName;
	public AttackData lightAttack;
	public AttackData heavyAttack;
}