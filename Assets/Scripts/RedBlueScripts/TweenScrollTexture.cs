using UnityEngine;
using System.Collections;

/// <summary>
/// Move a texture across a plane at a given linear speed.
/// </summary>
public class TweenScrollTexture : MonoBehaviour
{
	public Vector2 speed;
	
	void Update ()
	{
		renderer.material.mainTextureOffset = Time.time * speed;
	}

}
