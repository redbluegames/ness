using UnityEngine;
using System.Collections;

public class UIHealthBar : MonoBehaviour
{
	public Health health;
	public GUIText healthDisplay;
	
	// Update is called once per frame
	void Update ()
	{
		if (healthDisplay != null) {
			healthDisplay.text = "Health: " + health.curHealth;
			//bar.sliderValue = health.CalculateDisplayPercent();
		} else {
			Debug.LogWarning ("Health Text not set in editor. Attach health text object to HealthBar script in HUD.");
		}
	}
}