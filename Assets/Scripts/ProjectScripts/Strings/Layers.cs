using UnityEngine;
using System.Collections;

public class Layers
{
	public static int DEFAULT = 0;
	public static int IGNORE_RAYCAST = 1;
	public static int GUI = 8;
	public static int PLAYER = 9;
	public static int ENEMY = 10;
	public static int WALL = 11;
	public static int TEMP_WALL = 12;
	public static int RAGDOLL = 13;

	/// <summary>
	/// Determines if the object is on the specified layer mask.
	/// </summary>
	/// <returns><c>true</c> if the object is on the specified layer mask; otherwise, <c>false</c>.</returns>
	/// <param name="obj">Game Object to check.</param>
	/// <param name="mask">Mask to check against.</param>
	public static bool IsObjectOnLayerMask(GameObject obj, LayerMask mask)
	{
		return ((mask.value & (1 << obj.layer)) > 0);
	}
}
