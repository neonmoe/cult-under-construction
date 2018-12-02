using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable {
	public Transform DoorHinge;
	public bool IsOpen = false;
	public float OpenRotation = 90;
	public float ClosedRotation = 0;

	private float TargetRotation = 0;

	private void Update() {
		if (IsOpen) {
			TargetRotation = OpenRotation;
		} else if (!IsOpen) {
			TargetRotation = ClosedRotation;
		}
		Vector3 NewEulers = DoorHinge.localEulerAngles;
		NewEulers.z = Mathf.Lerp(NewEulers.z, TargetRotation, 10f * Time.deltaTime);
		DoorHinge.localEulerAngles = NewEulers;
	}

	public void Interact(GameObject equippedItem) {
		IsOpen = !IsOpen;
	}
}
