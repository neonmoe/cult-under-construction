using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobber : MonoBehaviour {
	[Header("Ensure the bobber starts at local y position 0!")]
	public bool Bob = false;
	public float BobFrequency = 2f;
	public float BobLength = 0.1f;

	private float StartingTime = -1;
	private bool LastBob = false;

	private void Update() {
		if (!LastBob && Bob) {
			// Started bobbing
			StartingTime = Time.time;
		}
		float BobIntensity = 0;
		if (Bob) {
			// Am bobbing
			BobIntensity = Mathf.Sin((Time.time - StartingTime) * Mathf.PI * 2f * BobFrequency) * BobLength * Config.CameraBobbingIntensity;
		}
		Vector3 NewPosition = transform.localPosition;
		NewPosition.y = Mathf.Lerp(NewPosition.y, BobIntensity, 20f * Time.deltaTime);
		transform.localPosition = NewPosition;
		LastBob = Bob;
	}
}
