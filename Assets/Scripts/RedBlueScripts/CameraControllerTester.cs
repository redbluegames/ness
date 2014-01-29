using UnityEngine;
using System.Collections;

public class CameraControllerTester : MonoBehaviour
{
	public CameraController cameraController;

	// Test shake variables
	public float shakeSpeed;
	public float shakeMagnitude;
	public float shakeDuration;
	public bool testShake;                  // Force camera shake using provided properties

	// Test zoom variables
	public float zoomRatio;
	public float zoomDuration;
	public float resetZoomDuration;
	public bool testZoom;                   // Force camera zoom using provided properties
	public bool testResetZoom;              // Force reset camera zoom

	// Update is called once per frame
	void Update ()
	{
		// Try our test methods
		TestZoom ();
		TestShake ();
		TestResetZoom ();
	}

	/*
	 * Test method to help us test the behavior of various zooms. Set testZoom to true
	 * while in Play mode.
	 */
	void TestZoom ()
	{
		if (testZoom) {
			testZoom = false;
			cameraController.Zoom (zoomRatio, zoomDuration);
		}
	}

	/*
	 * Test method to help check the behavior of various shakes. Set testShake to true
	 * while in Play mode.
	 */
	void TestShake ()
	{
		if (testShake) {
			testShake = false;
			cameraController.Shake (shakeSpeed, shakeDuration, shakeMagnitude);
		}
	}

	/*
	 * Reset zoom using either default or 
	 */
	void TestResetZoom ()
	{
		if (testResetZoom) {
			testResetZoom = false;
			if (resetZoomDuration > 0) {
				cameraController.ResetZoom (resetZoomDuration);
			} else {
				cameraController.ResetZoom ();
			}
		}
	}
}
