using UnityEngine;
using System.Collections;

public class RBInput
{

	const string PLAYER_PREFIX = "_P";
	const string DEVICE_PREFIX = "_";

	public static bool GetButtonDownForPlayer (string buttonName, int playerIndex)
	{
		bool isAnyButtonDown = false;
		foreach (InputDevice d in InputDevices.GetAllInputDevices()) {
			isAnyButtonDown |= Input.GetButtonDown (ConcatPlayerIndex (buttonName, playerIndex, d));
		}
		return isAnyButtonDown;
	}
	
	public static bool GetButtonUpForPlayer (string buttonName, int playerIndex)
	{
		
		bool isAnyButtonDown = false;
		foreach (InputDevice d in InputDevices.GetAllInputDevices()) {
			isAnyButtonDown |= Input.GetButtonUp (ConcatPlayerIndex (buttonName, playerIndex, d));
		}
		return isAnyButtonDown;
	}

	public static bool GetButtonForPlayer (string buttonName, int playerIndex)
	{
		bool isAnyButtonDown = false;
		foreach (InputDevice d in InputDevices.GetAllInputDevices()) {
			isAnyButtonDown |= Input.GetButtonUp (ConcatPlayerIndex (buttonName, playerIndex, d));
		}
		return isAnyButtonDown;
	}

	public static float GetAxisRawForPlayer (string axisName, int playerIndex)
	{
		float axisTotal = 0.0f;
		foreach (InputDevice d in InputDevices.GetAllInputDevices()) {
			axisTotal += Input.GetAxisRaw (ConcatPlayerIndex (axisName, playerIndex, d));
		}
		return axisTotal;
	}

	public static float GetAxisForPlayer (string axisName, int playerIndex)
	{
		float axisTotal = 0.0f;
		foreach (InputDevice d in InputDevices.GetAllInputDevices()) {
			axisTotal += Input.GetAxis (ConcatPlayerIndex (axisName, playerIndex, d));
		}
		return axisTotal;
	}

	public static string ConcatPlayerIndex (string buttonName, int playerIndex, InputDevice device)
	{
		return buttonName + DEVICE_PREFIX + device.DeviceName + PLAYER_PREFIX + playerIndex;
	}
}
