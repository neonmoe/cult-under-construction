using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Config {
	public static Vector2 MouseSensitivity = new Vector2(1, 1);
	public static bool InvertMouseX = false;
	public static bool InvertMouseY = true;
	public static float CameraBobbingIntensity = 1.0f;

	public static Vector2 GetMouseLook() {
		Vector2 Result = MouseSensitivity;
		Result.x *= Input.GetAxis("Mouse X") * (InvertMouseX ? -1 : 1);
		Result.y *= Input.GetAxis("Mouse Y") * (InvertMouseY ? -1 : 1);
		return Result;
	}
}
