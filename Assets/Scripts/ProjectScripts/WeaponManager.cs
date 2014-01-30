using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponManager : Singleton<WeaponManager>
{
	public List<WeaponData> weaponList = new List<WeaponData> ();
	public GoogleFu.Weapons weaponsDb;
	
	// Use this for initialization
	void Awake ()
	{
		weaponsDb = GetComponent<GoogleFu.Weapons> ();
		if (weaponsDb == null) {
			Debug.LogWarning ("Weapon DB not found. Make sure you have run GoogleFU import.");
		}
		ImportWeapons ();
	}

	public void ClearWeapons ()
	{
		weaponList.Clear ();
	}
	
	/*
	 * Search through our list of weapons and return the requested Weapon scriptable object.
	 * Must supply the GoogleFU rowId for the desired weapon.
	 */
	public WeaponData GetWeaponData (GoogleFu.Weapons.rowIds weaponId)
	{
		return GetWeaponData (weaponId.ToString ());
	}
	
	/*
	 * Search through our list of weapons and returns the requested Weapon.
	 * Must supply the GoogleFU rowID as a string value.
	 */
	public WeaponData GetWeaponData (string weaponId)
	{
		WeaponData ret = null;
		foreach (WeaponData weapon in weaponList) {
			if (string.Equals (weapon.weaponName, weaponsDb.GetRow (weaponId)._NAME)) {
				ret = weapon;
			}
		}
		return ret;
	}

	public GameObject SpawnWeapon (GoogleFu.Weapons.rowIds weaponId, GameObject parent)
	{
		return SpawnWeapon (weaponId.ToString (), parent);
	}

	public GameObject SpawnWeapon (string weaponName, GameObject parent)
	{
		GameObject newWeaponObj = (GameObject)Instantiate (Resources.Load (weaponName, typeof(GameObject)));
		Weapon newWeapon = newWeaponObj.GetComponent<Weapon> ();
		newWeapon.weaponData = GetWeaponData (weaponName);
		newWeaponObj.transform.parent = parent.transform;
		newWeaponObj.transform.position = newWeaponObj.transform.position + parent.transform.position;
		if (newWeapon.weaponData.lightAttack.projectilePrefab != string.Empty) {
			newWeapon.lightProjectile = (GameObject)Resources.Load (newWeapon.weaponData.lightAttack.projectilePrefab, typeof(GameObject));
			//newWeapon.lightProjectile.name = newWeapon.weaponData.lightAttack.projectilePrefab + "_light";
		}
		if (newWeapon.weaponData.heavyAttack.projectilePrefab != string.Empty) {
			newWeapon.heavyProjectile = (GameObject)Resources.Load (newWeapon.weaponData.heavyAttack.projectilePrefab, typeof(GameObject));
			//newWeapon.heavyProjectile.name = newWeapon.weaponData.heavyAttack.projectilePrefab + "_heavy";
		}
		return newWeaponObj;
	}

	void ImportWeapons ()
	{
		ClearWeapons ();
		
		foreach (GoogleFu.WeaponsRow row in weaponsDb.Rows) {
			WeaponData newWeaponData = (WeaponData)ScriptableObject.CreateInstance<WeaponData> ();
			newWeaponData.name = row._NAME;
			newWeaponData.weaponName = row._NAME;
			newWeaponData.lightAttack = AttackManager.Instance.GetAttack (row._LIGHTATTACKID);
			newWeaponData.heavyAttack = AttackManager.Instance.GetAttack (row._HEAVYATTACKID);
			weaponList.Add (newWeaponData);
		}
	}
}
